using Hrm.Application.Employees.Dtos;
using Hrm.Domain.Employees.Constants;
using Jarvis.Application.Contracts.Queries;

namespace Hrm.Application.Employees.Queries;

public sealed record ListContractTypesQuery : Jarvis.Domain.Shared.Messaging.IQuery;

public sealed class ListContractTypesQueryHandler
    : IAsyncQueryHandler<ListContractTypesQuery, IReadOnlyList<EmpCatalogItemDto>>
{
    public Task<IReadOnlyList<EmpCatalogItemDto>> HandleAsync(
        ListContractTypesQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        // Master động quy chế — không hardcode list URD trên UI (EMP-FR-014).
        IReadOnlyList<EmpCatalogItemDto> items =
        [
            new(EmpContractTypes.Probation, "Thử việc"),
            new(EmpContractTypes.Official, "Chính thức")
        ];
        return Task.FromResult(items);
    }
}
