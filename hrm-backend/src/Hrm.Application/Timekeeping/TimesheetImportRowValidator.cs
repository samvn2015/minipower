using Hrm.Domain.Employees;
using Hrm.Domain.Employees.Repositories;
using Hrm.Domain.Timekeeping.Repositories;

namespace Hrm.Application.Timekeeping;

/// <summary>Validate import rows against master columns + EMP — TIM-FR-004.</summary>
public static class TimesheetImportRowValidator
{
    public sealed record RawImportRow(
        int RowNumber,
        string? EmployeeCode,
        decimal? WorkDays,
        decimal? Ot15,
        decimal? Ot20,
        decimal? Ot30,
        decimal? OtUnclassified);

    public static async Task<IReadOnlyList<TimesheetImportRowCreateModel>> ValidateAsync(
        IReadOnlyList<RawImportRow> rows,
        IEmployeeReadRepository employees,
        CancellationToken cancellationToken)
    {
        var result = new List<TimesheetImportRowCreateModel>(rows.Count);
        foreach (var row in rows)
        {
            var code = row.EmployeeCode?.Trim();
            if (string.IsNullOrWhiteSpace(code))
            {
                result.Add(Error(row, "MISSING_EMPLOYEE", "Thiếu MNV (TIM-FR-004 Must)."));
                continue;
            }

            var employee = await employees.FindByEmployeeCodeAsync(code, cancellationToken).ConfigureAwait(false);
            if (employee is null)
            {
                result.Add(Error(row, "UNKNOWN_EMPLOYEE", $"MNV {code} không tồn tại (TIM-FR-004 Must)."));
                continue;
            }

            if (employee.Status != EmployeeStatus.Active)
            {
                result.Add(Error(row, "INACTIVE_EMPLOYEE", $"MNV {code} không Active (TIM-FR-004 Must)."));
                continue;
            }

            if (row.WorkDays is null)
            {
                result.Add(Error(row, "MISSING_WORK_DAYS", "Thiếu ngày công thực (TIM-FR-004 Must)."));
                continue;
            }

            result.Add(new TimesheetImportRowCreateModel(
                row.RowNumber,
                code,
                employee.Id,
                row.WorkDays,
                row.Ot15,
                row.Ot20,
                row.Ot30,
                row.OtUnclassified,
                IsOk: true,
                ErrorCode: null,
                ErrorMessage: null));
        }

        return result;
    }

    private static TimesheetImportRowCreateModel Error(RawImportRow row, string code, string message) =>
        new(
            row.RowNumber,
            row.EmployeeCode?.Trim(),
            null,
            row.WorkDays,
            row.Ot15,
            row.Ot20,
            row.Ot30,
            row.OtUnclassified,
            IsOk: false,
            ErrorCode: code,
            ErrorMessage: message);
}
