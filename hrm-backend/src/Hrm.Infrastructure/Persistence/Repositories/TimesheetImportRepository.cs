using Hrm.Domain.Timekeeping;
using Hrm.Domain.Timekeeping.Entities;
using Hrm.Domain.Timekeeping.Repositories;
using Hrm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class TimesheetImportRepository(AppDbContext db) : ITimesheetImportRepository
{
    public async Task<Guid> CreatePreviewAsync(
        TimesheetImportBatchCreateModel model,
        CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        var errorRows = model.Rows.Count(r => !r.IsOk);
        var entity = new TimesheetImportBatch
        {
            Id = id,
            PeriodYm = model.PeriodYm,
            TemplateVersionId = model.TemplateVersionId,
            TemplateVersionCode = model.TemplateVersionCode,
            Status = TimesheetImportBatchStatus.Preview,
            UploadedByIdpSubject = model.UploadedByIdpSubject,
            UploadedAtUtc = DateTime.UtcNow,
            FileName = model.FileName,
            TotalRows = model.Rows.Count,
            ErrorRows = errorRows,
            HasMustErrors = errorRows > 0,
            Rows = model.Rows.Select(r => new TimesheetImportRow
            {
                Id = Guid.NewGuid(),
                BatchId = id,
                RowNumber = r.RowNumber,
                EmployeeCode = r.EmployeeCode,
                EmployeeId = r.EmployeeId,
                WorkDays = r.WorkDays,
                Ot15 = r.Ot15,
                Ot20 = r.Ot20,
                Ot30 = r.Ot30,
                OtUnclassified = r.OtUnclassified,
                IsOk = r.IsOk,
                ErrorCode = r.ErrorCode,
                ErrorMessage = r.ErrorMessage
            }).ToList()
        };

        db.TimesheetImportBatches.Add(entity);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return id;
    }

    public async Task<TimesheetImportBatchSnapshot?> FindBatchByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.TimesheetImportBatches.AsNoTracking()
            .Include(x => x.Rows)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            .ConfigureAwait(false);
        return entity is null ? null : MapBatch(entity);
    }

    public async Task<TimesheetPeriodSnapshot?> FindPeriodByYmAsync(
        string periodYm,
        CancellationToken cancellationToken = default)
    {
        var entity = await db.TimesheetPeriods.AsNoTracking()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.PeriodYm == periodYm, cancellationToken)
            .ConfigureAwait(false);
        return entity is null ? null : MapPeriod(entity);
    }

    public async Task<IReadOnlyList<TimesheetPeriodSnapshot>> ListPeriodsAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await db.TimesheetPeriods.AsNoTracking()
            .Include(x => x.Lines)
            .OrderByDescending(x => x.PeriodYm)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        return rows.Select(MapPeriod).ToList();
    }

    public async Task<TimesheetPeriodSnapshot?> CommitAsync(
        Guid batchId,
        string committedByIdpSubject,
        CancellationToken cancellationToken = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        var batch = await db.TimesheetImportBatches
            .Include(x => x.Rows)
            .FirstOrDefaultAsync(x => x.Id == batchId, cancellationToken)
            .ConfigureAwait(false);
        if (batch is null
            || batch.Status != TimesheetImportBatchStatus.Preview
            || batch.HasMustErrors
            || batch.Rows.Any(r => !r.IsOk))
        {
            return null;
        }

        var period = await db.TimesheetPeriods
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.PeriodYm == batch.PeriodYm, cancellationToken)
            .ConfigureAwait(false);

        if (period is { Status: TimesheetPeriodStatus.Closed })
            return null;

        if (period is null)
        {
            period = new TimesheetPeriod
            {
                Id = Guid.NewGuid(),
                PeriodYm = batch.PeriodYm,
                Status = TimesheetPeriodStatus.Draft
            };
            db.TimesheetPeriods.Add(period);
        }
        else
        {
            db.TimesheetLines.RemoveRange(period.Lines);
            period.Lines.Clear();
        }

        foreach (var row in batch.Rows.Where(r => r.IsOk && r.EmployeeId.HasValue))
        {
            period.Lines.Add(new TimesheetLine
            {
                Id = Guid.NewGuid(),
                PeriodId = period.Id,
                EmployeeId = row.EmployeeId!.Value,
                EmployeeCode = row.EmployeeCode!,
                WorkDays = row.WorkDays ?? 0,
                Ot15 = row.Ot15 ?? 0,
                Ot20 = row.Ot20 ?? 0,
                Ot30 = row.Ot30 ?? 0,
                OtUnclassified = row.OtUnclassified ?? 0
            });
        }

        period.Status = TimesheetPeriodStatus.Draft;
        period.SourceImportBatchId = batch.Id;
        period.CommittedAtUtc = DateTime.UtcNow;
        period.CommittedByIdpSubject = committedByIdpSubject;
        period.ClosedAtUtc = null;
        period.ClosedByIdpSubject = null;

        batch.Status = TimesheetImportBatchStatus.Committed;

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await tx.CommitAsync(cancellationToken).ConfigureAwait(false);

        return MapPeriod(period);
    }

    public async Task<TimesheetPeriodSnapshot?> ClosePeriodAsync(
        string periodYm,
        string closedByIdpSubject,
        IReadOnlyList<TimesheetLeaveMergeLine> leaveMerge,
        CancellationToken cancellationToken = default)
    {
        var period = await db.TimesheetPeriods
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.PeriodYm == periodYm, cancellationToken)
            .ConfigureAwait(false);

        if (period is null || period.Status != TimesheetPeriodStatus.Draft)
            return null;

        var mergeByEmployee = leaveMerge.ToDictionary(x => x.EmployeeId);
        foreach (var line in period.Lines)
        {
            // Idempotent: reset prior merge then apply (Draft only).
            line.WorkDays -= line.LeaveDaysPaid;
            line.LeaveDaysPaid = 0;
            line.LeaveDaysUnpaid = 0;
            line.LeaveDaysOther = 0;

            if (!mergeByEmployee.TryGetValue(line.EmployeeId, out var merge))
                continue;

            line.LeaveDaysPaid = merge.LeaveDaysPaid;
            line.LeaveDaysUnpaid = merge.LeaveDaysUnpaid;
            line.LeaveDaysOther = merge.LeaveDaysOther;
            line.WorkDays += merge.LeaveDaysPaid;
        }

        period.Status = TimesheetPeriodStatus.Closed;
        period.ClosedAtUtc = DateTime.UtcNow;
        period.ClosedByIdpSubject = closedByIdpSubject;

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return MapPeriod(period);
    }

    private static TimesheetPeriodSnapshot MapPeriod(TimesheetPeriod entity) =>
        new(
            entity.Id,
            entity.PeriodYm,
            entity.Status,
            entity.SourceImportBatchId,
            entity.Lines.Count,
            entity.Lines.Select(l => new TimesheetLineSnapshot(
                l.Id,
                l.EmployeeId,
                l.EmployeeCode,
                l.WorkDays,
                l.Ot15,
                l.Ot20,
                l.Ot30,
                l.OtUnclassified,
                l.LeaveDaysPaid,
                l.LeaveDaysUnpaid,
                l.LeaveDaysOther)).ToList());

    private static TimesheetImportBatchSnapshot MapBatch(TimesheetImportBatch entity) =>
        new(
            entity.Id,
            entity.PeriodYm,
            entity.TemplateVersionId,
            entity.TemplateVersionCode,
            entity.Status,
            entity.UploadedByIdpSubject,
            entity.UploadedAtUtc,
            entity.FileName,
            entity.TotalRows,
            entity.ErrorRows,
            entity.HasMustErrors,
            entity.Rows.OrderBy(r => r.RowNumber).Select(r => new TimesheetImportRowSnapshot(
                r.Id,
                r.RowNumber,
                r.EmployeeCode,
                r.EmployeeId,
                r.WorkDays,
                r.Ot15,
                r.Ot20,
                r.Ot30,
                r.OtUnclassified,
                r.IsOk,
                r.ErrorCode,
                r.ErrorMessage)).ToList());
}
