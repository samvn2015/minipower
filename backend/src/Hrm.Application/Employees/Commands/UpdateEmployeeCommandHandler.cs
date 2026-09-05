using Hrm.Application.Employees.Commands;
using Hrm.Application.Common;
using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Employees.Commands;

public sealed class UpdateEmployeeCommandHandler(
    IIdentityAccountReadRepository accounts,
    IOrgUnitReadRepository orgUnits,
    IEducationLevelReadRepository educationLevels,
    IEmployeeReadRepository employees,
    IEmployeeWriteRepository employeeWrites,
    IEmpAuditLogRepository auditLogs)
    : IAsyncCommandHandler<UpdateEmployeeCommand, EmployeeUpdateResult>
{
    public async Task<EmployeeUpdateResult> HandleAsync(
        UpdateEmployeeCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        if (actor is null || actor.Status != Domain.Identity.IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");

        var employee = await employees.FindByIdAsync(command.EmployeeId, cancellationToken).ConfigureAwait(false)
            ?? throw new NotFoundException(HrmErrorCodes.NotFound, $"Employee {command.EmployeeId} không tồn tại.");

        var isHrOrIt = IamAccessGuard.IsHrOrIt(actor);
        if (!isHrOrIt)
        {
            if (string.IsNullOrWhiteSpace(actor.EmployeeCode)
                || !string.Equals(actor.EmployeeCode, employee.EmployeeCode, StringComparison.OrdinalIgnoreCase))
            {
                throw new ForbiddenException(HrmErrorCodes.Forbidden, "Chỉ sửa hồ sơ của chính mình (EMP-FR-011).");
            }

            if (command.OrgUnitCode is not null
                || command.Contract is not null
                || command.EducationLevelCode is not null
                || command.SeniorityStartDate is not null)
            {
                throw new ForbiddenException(HrmErrorCodes.Forbidden, "NV không sửa org/HĐ/học vấn (EMP-FR-007).");
            }
        }

        if (command.OrgUnitCode is not null)
        {
            await EmpOrgGuard.RequireActiveOrgAsync(orgUnits, command.OrgUnitCode, cancellationToken)
                .ConfigureAwait(false);
        }

        if (command.EducationLevelCode is not null)
        {
            await EmpEducationGuard.RequireActiveEducationLevelAsync(
                educationLevels,
                command.EducationLevelCode,
                cancellationToken).ConfigureAwait(false);
        }

        var contract = EmpContractGuard.Normalize(command.Contract);

        var nextCccd = command.Cccd ?? employee.Cccd;
        var nextEmail = command.EmailCty ?? employee.EmailCty;
        var nextTaxId = command.TaxId ?? employee.TaxId;

        await EmployeeUniqueGuard.EnsureUniqueAsync(
            employees,
            employee.EmployeeCode,
            nextCccd,
            nextEmail,
            nextTaxId,
            command.EmployeeId,
            cancellationToken).ConfigureAwait(false);

        var updated = await employeeWrites.UpdateAsync(
            command.EmployeeId,
            new EmployeePatch(
                command.FullName,
                command.EmailCty,
                command.Cccd,
                command.TaxId,
                command.OrgUnitCode,
                command.EducationLevelCode,
                command.SeniorityStartDate,
                contract),
            cancellationToken).ConfigureAwait(false);

        if (!updated)
            throw new NotFoundException(HrmErrorCodes.NotFound, $"Employee {command.EmployeeId} không tồn tại.");

        await auditLogs.AppendAsync(
            new EmpAuditLogEntry(
                EmpAuditActions.EmployeeUpdated,
                command.EmployeeId,
                null,
                command.ActorIdpSubject!,
                null),
            cancellationToken).ConfigureAwait(false);

        var refreshed = await employees.FindByIdAsync(command.EmployeeId, cancellationToken).ConfigureAwait(false);
        return new EmployeeUpdateResult(command.EmployeeId, refreshed!.Status.ToString());
    }
}
