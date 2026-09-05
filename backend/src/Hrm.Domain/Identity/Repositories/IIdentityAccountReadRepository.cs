namespace Hrm.Domain.Identity.Repositories;

public interface IIdentityAccountReadRepository
{
    Task<IdentityAccountSnapshot?> FindByIdpSubjectAsync(
        string idpSubject,
        CancellationToken cancellationToken = default);

    Task<IdentityAccountSnapshot?> FindByEmployeeCodeAsync(
        string employeeCode,
        CancellationToken cancellationToken = default);
}
