using Hrm.Application.Common;
using Hrm.Application.Employees.Commands;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Employees.Commands;

public sealed class UpdateEmployeeCommandHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    IEmployeeWriteRepository employeeWrites)
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
        }

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
            new EmployeePatch(command.FullName, command.EmailCty, command.Cccd, command.TaxId),
            cancellationToken).ConfigureAwait(false);

        if (!updated)
            throw new NotFoundException(HrmErrorCodes.NotFound, $"Employee {command.EmployeeId} không tồn tại.");

        var refreshed = await employees.FindByIdAsync(command.EmployeeId, cancellationToken).ConfigureAwait(false);
        return new EmployeeUpdateResult(command.EmployeeId, refreshed!.Status.ToString());
    }
}
