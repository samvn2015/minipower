using Hrm.Domain.Probation;
using Hrm.Domain.Probation.Entities;
using Hrm.Domain.Probation.Repositories;
using Hrm.Infrastructure.Persistence;
using Hrm.Domain.Shared.Paging;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class ProbationReminderRepository(PrbDbContext db) : IProbationReminderRepository
{
    public Task<bool> ExistsAsync(
        Guid employeeId,
        ProbationReminderKind kind,
        DateOnly probationEndDate,
        CancellationToken cancellationToken = default) =>
        db.ProbationReminders.AnyAsync(
            x => x.EmployeeId == employeeId && x.Kind == kind && x.ProbationEndDate == probationEndDate,
            cancellationToken);

    public async Task AddManyAsync(
        IReadOnlyList<ProbationReminderCreateModel> rows,
        CancellationToken cancellationToken = default)
    {
        foreach (var row in rows)
        {
            db.ProbationReminders.Add(new ProbationReminder
            {
                Id = Guid.NewGuid(),
                Kind = row.Kind,
                EmployeeId = row.EmployeeId,
                EmployeeCode = row.EmployeeCode,
                ProbationEndDate = row.ProbationEndDate,
                DueDate = row.DueDate,
                AsOfDate = row.AsOfDate,
                AssigneeEmployeeId = row.AssigneeEmployeeId,
                AssigneeEmployeeCode = row.AssigneeEmployeeCode,
                InAppMessage = row.InAppMessage,
                EmailTo = row.EmailTo,
                Channel = row.Channel,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByIdpSubject = row.CreatedByIdpSubject
            });
        }

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<PagedResult<ProbationReminderSnapshot>> ListPagedAsync(
        ProbationReminderKind? kind,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(page);

        var q = db.ProbationReminders.AsNoTracking().AsQueryable();
        if (kind is { } k)
            q = q.Where(x => x.Kind == k);

        // Tổng đếm SAU khi lọc kind, TRƯỚC khi cắt trang — tổng phải khớp bộ lọc.
        var total = await q.CountAsync(cancellationToken).ConfigureAwait(false);

        var rows = await q.OrderByDescending(x => x.CreatedAtUtc)
            .Skip(page.Skip)
            .Take(page.Size)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        var items = rows.Select(x => new ProbationReminderSnapshot(
            x.Id,
            x.Kind,
            x.EmployeeId,
            x.EmployeeCode,
            x.ProbationEndDate,
            x.DueDate,
            x.AsOfDate,
            x.AssigneeEmployeeId,
            x.AssigneeEmployeeCode,
            x.InAppMessage,
            x.EmailTo,
            x.Channel,
            x.CreatedAtUtc)).ToList();

        return new PagedResult<ProbationReminderSnapshot>(items, total);
    }
}
