using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Entities;
using Hrm.Domain.Employees.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class LineManagerChangeRepository(AppDbContext db)
    : ILineManagerChangeRepository
{
    public Task<LineManagerChangeSnapshot?> FindByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        ProjectQuery(db.LineManagerChangeRequests.AsNoTracking().Where(r => r.Id == id))
            .FirstOrDefaultAsync(cancellationToken);

    public Task<LineManagerChangeSnapshot?> FindPendingByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default) =>
        ProjectQuery(db.LineManagerChangeRequests.AsNoTracking().Where(r =>
            r.EmployeeId == employeeId && r.Status == LineManagerChangeStatus.Pending))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<LineManagerChangeSnapshot>> ListPendingAsync(
        CancellationToken cancellationToken = default) =>
        await ProjectQuery(
                db.LineManagerChangeRequests.AsNoTracking()
                    .Where(r => r.Status == LineManagerChangeStatus.Pending)
                    .OrderBy(r => r.RequestedAtUtc))
            .ToListAsync(cancellationToken);

    public async Task<Guid> CreateAsync(
        LineManagerChangeCreateModel model,
        CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        db.LineManagerChangeRequests.Add(new LineManagerChangeRequest
        {
            Id = id,
            EmployeeId = model.EmployeeId,
            ProposedLineManagerEmployeeId = model.ProposedLineManagerEmployeeId,
            Status = LineManagerChangeStatus.Pending,
            RequestedByIdpSubject = model.RequestedByIdpSubject.Trim(),
            RequestedAtUtc = DateTime.UtcNow
        });

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return id;
    }

    public async Task<bool> ApproveAsync(
        Guid requestId,
        Guid employeeId,
        Guid proposedLineManagerEmployeeId,
        string reviewedByIdpSubject,
        CancellationToken cancellationToken = default)
    {
        var request = await db.LineManagerChangeRequests
            .FirstOrDefaultAsync(
                r => r.Id == requestId && r.Status == LineManagerChangeStatus.Pending,
                cancellationToken);
        if (request is null)
            return false;

        var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);
        if (employee is null)
            return false;

        employee.LineManagerEmployeeId = proposedLineManagerEmployeeId;
        request.Status = LineManagerChangeStatus.Approved;
        request.ReviewedByIdpSubject = reviewedByIdpSubject.Trim();
        request.ReviewedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    public async Task<bool> RejectAsync(
        Guid requestId,
        string reviewedByIdpSubject,
        string? reviewNote,
        CancellationToken cancellationToken = default)
    {
        var request = await db.LineManagerChangeRequests
            .FirstOrDefaultAsync(
                r => r.Id == requestId && r.Status == LineManagerChangeStatus.Pending,
                cancellationToken);
        if (request is null)
            return false;

        request.Status = LineManagerChangeStatus.Rejected;
        request.ReviewedByIdpSubject = reviewedByIdpSubject.Trim();
        request.ReviewedAtUtc = DateTime.UtcNow;
        request.ReviewNote = reviewNote?.Trim();

        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    private IQueryable<LineManagerChangeSnapshot> ProjectQuery(
        IQueryable<LineManagerChangeRequest> requests) =>
        from r in requests
        join e in db.Employees.AsNoTracking() on r.EmployeeId equals e.Id
        join lm in db.Employees.AsNoTracking() on r.ProposedLineManagerEmployeeId equals lm.Id
        select new LineManagerChangeSnapshot(
            r.Id,
            r.EmployeeId,
            e.EmployeeCode,
            e.FullName,
            r.ProposedLineManagerEmployeeId,
            lm.EmployeeCode,
            lm.FullName,
            r.Status,
            r.RequestedByIdpSubject,
            r.RequestedAtUtc,
            r.ReviewedByIdpSubject,
            r.ReviewedAtUtc,
            r.ReviewNote);
}
