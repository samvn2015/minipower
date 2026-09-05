using Hrm.Domain.Employees.Constants;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Employees;

public static class EmpOrgGuard
{
    public static async Task RequireActiveOrgAsync(
        IOrgUnitReadRepository orgUnits,
        string? orgUnitCode,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(orgUnitCode))
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Org bắt buộc (EMP-FR-004).");

        if (!await orgUnits.IsActiveAsync(orgUnitCode, cancellationToken).ConfigureAwait(false))
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Org không hiệu lực (EMP-FR-004).");
    }
}

public static class EmpContractGuard
{
    public static EmployeeContractUpsert? Normalize(EmployeeContractUpsert? contract)
    {
        if (contract is null)
            return null;

        if (!EmpContractTypes.All.Contains(contract.ContractType))
        {
            throw new BadRequestException(
                HrmErrorCodes.BadRequest,
                "Loại HĐ không thuộc master (EMP-FR-014).");
        }

        return contract with { ContractType = contract.ContractType.Trim().ToUpperInvariant() };
    }

    public static string? MissingContractWarning(EmployeeContractUpsert? contract) =>
        contract is null ? "Thiếu HĐ hiệu lực — cảnh báo EMP-FR-005." : null;
}
