using Hrm.Domain.Employees.Entities;
using Hrm.Domain.Employees.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hrm.Infrastructure.Persistence.Repositories;

internal sealed class EmpAuditLogRepository(AppDbContext db) : IEmpAuditLogRepository
{
    public async Task AppendAsync(EmpAuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        db.EmpAuditLogs.Add(new EmpAuditLog
        {
            Id = Guid.NewGuid(),
            Action = entry.Action,
            EmployeeId = entry.EmployeeId,
            RelatedId = entry.RelatedId,
            ActorIdpSubject = entry.ActorIdpSubject,
            OccurredAtUtc = DateTime.UtcNow,
            Detail = entry.Detail
        });
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<EmpAuditLogSnapshot>> ListByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default) =>
        await db.EmpAuditLogs
            .AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.OccurredAtUtc)
            .Select(x => new EmpAuditLogSnapshot(
                x.Id,
                x.Action,
                x.EmployeeId,
                x.RelatedId,
                x.ActorIdpSubject,
                x.OccurredAtUtc,
                x.Detail))
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<EmpAuditLogSnapshot>> ListByActionAsync(
        string action,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var limit = take <= 0 ? 50 : Math.Min(take, 200);
        return await db.EmpAuditLogs
            .AsNoTracking()
            .Where(x => x.Action == action)
            .OrderByDescending(x => x.OccurredAtUtc)
            .Take(limit)
            .Select(x => new EmpAuditLogSnapshot(
                x.Id,
                x.Action,
                x.EmployeeId,
                x.RelatedId,
                x.ActorIdpSubject,
                x.OccurredAtUtc,
                x.Detail))
            .ToArrayAsync(cancellationToken);
    }
}
