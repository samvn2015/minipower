using Hrm.Domain.Employees;

namespace Hrm.Domain.Employees.Repositories;

public sealed record LineManagerChangeCreateModel(
    Guid EmployeeId,
    Guid ProposedLineManagerEmployeeId,
    string RequestedByIdpSubject);

public interface ILineManagerChangeRepository
{
    Task<LineManagerChangeSnapshot?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LineManagerChangeSnapshot?> FindPendingByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LineManagerChangeSnapshot>> ListPendingAsync(
        CancellationToken cancellationToken = default);

    Task<Guid> CreateAsync(LineManagerChangeCreateModel model, CancellationToken cancellationToken = default);

    Task<bool> ApproveAsync(
        Guid requestId,
        Guid employeeId,
        Guid proposedLineManagerEmployeeId,
        string reviewedByIdpSubject,
        CancellationToken cancellationToken = default);

    Task<bool> RejectAsync(
        Guid requestId,
        string reviewedByIdpSubject,
        string? reviewNote,
        CancellationToken cancellationToken = default);
}
