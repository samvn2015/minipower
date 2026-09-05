namespace Hrm.Domain.Employees.Repositories;

public sealed record EducationLevelSnapshot(string Code, string Name);

public interface IEducationLevelReadRepository
{
    Task<bool> IsActiveAsync(string code, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EducationLevelSnapshot>> ListActiveAsync(
        CancellationToken cancellationToken = default);
}
