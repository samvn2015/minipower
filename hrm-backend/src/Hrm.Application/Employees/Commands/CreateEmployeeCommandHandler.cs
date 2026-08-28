using Hrm.Application.Common;
using Hrm.Application.Employees.Commands;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Employees.Commands;

public sealed class CreateEmployeeCommandHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    IEmployeeWriteRepository employeeWrites)
    : IAsyncCommandHandler<CreateEmployeeCommand, EmployeeCreateResult>
{
    public async Task<EmployeeCreateResult> HandleAsync(
        CreateEmployeeCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);

        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        if (actor is null || actor.Status != Domain.Identity.IdentityAccountStatus.Active)
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Tài khoản không hiệu lực.");

        if (!IamAccessGuard.IsHrOrIt(actor))
            throw new ForbiddenException(HrmErrorCodes.Forbidden, "Chỉ HR/IT tạo NV (EMP-FR-001).");

        if (string.IsNullOrWhiteSpace(command.EmployeeCode))
            throw new BadRequestException(HrmErrorCodes.BadRequest, "MNV bắt buộc.");

        var code = command.EmployeeCode.Trim();
        await EmployeeUniqueGuard.EnsureUniqueAsync(
            employees,
            code,
            command.Cccd,
            command.EmailCty,
            command.TaxId,
            excludeEmployeeId: null,
            cancellationToken).ConfigureAwait(false);

        var id = await employeeWrites.CreateAsync(
            new EmployeeCreateModel(code, command.FullName, command.Cccd, command.EmailCty, command.TaxId),
            cancellationToken).ConfigureAwait(false);

        return new EmployeeCreateResult(id, code, nameof(Domain.Employees.EmployeeStatus.Active));
    }
}
