using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Shared.Constants;
using Jarvis.Domain.Shared.ExceptionHandling;

namespace Hrm.Application.Employees;

internal static class EmployeeUniqueGuard
{
    public static async Task EnsureUniqueAsync(
        IEmployeeReadRepository employees,
        string employeeCode,
        string? cccd,
        string? emailCty,
        string? taxId,
        Guid? excludeEmployeeId,
        CancellationToken cancellationToken)
    {
        var duplicate = await employees.FindDuplicateAsync(
            employeeCode,
            cccd,
            emailCty,
            taxId,
            excludeEmployeeId,
            cancellationToken).ConfigureAwait(false);

        if (duplicate is null)
            return;

        var message = duplicate.Value switch
        {
            EmployeeUniqueField.EmployeeCode => "Trùng MNV (EMP-FR-002).",
            EmployeeUniqueField.Cccd => "Trùng CCCD (EMP-FR-002).",
            EmployeeUniqueField.EmailCty => "Trùng email công ty (EMP-FR-003).",
            EmployeeUniqueField.TaxId => "Trùng MST (EMP-FR-003).",
            _ => "Trùng định danh nhân viên."
        };

        throw new ConflictException(HrmErrorCodes.Conflict, message);
    }
}
