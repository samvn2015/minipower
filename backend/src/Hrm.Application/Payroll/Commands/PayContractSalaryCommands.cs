using Hrm.Application.Common;
using Hrm.Application.Payroll.Dtos;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Identity.Repositories;
using Hrm.Domain.Payroll.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Application.Contracts.Commands;
using Jarvis.Domain.Shared.ExceptionHandling;
using Jarvis.Domain.Shared.Messaging;

namespace Hrm.Application.Payroll.Commands;

public sealed record UpsertPayContractSalaryCommand(
    string? ActorIdpSubject,
    string EmployeeCode,
    decimal Amount,
    int DependentCount) : ICommand;

public sealed class UpsertPayContractSalaryCommandHandler(
    IIdentityAccountReadRepository accounts,
    IEmployeeReadRepository employees,
    IPayContractSalaryRepository salaries)
    : IAsyncCommandHandler<UpsertPayContractSalaryCommand, PayContractSalaryResult>
{
    public async Task<PayContractSalaryResult> HandleAsync(
        UpsertPayContractSalaryCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        IamAccessGuard.RequireAuthenticated(command.ActorIdpSubject);
        var actor = await accounts.FindByIdpSubjectAsync(command.ActorIdpSubject!, cancellationToken)
            .ConfigureAwait(false);
        PayHrGuard.RequireHr(actor);

        var employeeCode = (command.EmployeeCode ?? "").Trim();
        if (string.IsNullOrWhiteSpace(employeeCode))
            throw new BadRequestException(HrmErrorCodes.BadRequest, "EmployeeCode bắt buộc.");
        if (command.Amount < 0m)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "Amount không được âm.");
        if (command.DependentCount < 0)
            throw new BadRequestException(HrmErrorCodes.BadRequest, "DependentCount không được âm.");

        var emp = await employees.FindByEmployeeCodeAsync(employeeCode, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new BadRequestException(HrmErrorCodes.BadRequest, $"Không tìm thấy NV {employeeCode}.");

        await salaries
            .UpsertAsync(emp.Id, emp.EmployeeCode, command.Amount, command.DependentCount, cancellationToken)
            .ConfigureAwait(false);

        return new PayContractSalaryResult(emp.EmployeeCode, command.Amount, command.DependentCount);
    }
}
