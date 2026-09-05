namespace Hrm.Domain.Payroll;

/// <summary>Cấm CC / gửi nhầm khi xuất phiếu — PAY-FR-012.</summary>
public static class PayExportRecipientGuard
{
    public static void EnsureNoCc(IReadOnlyList<string>? ccAddresses)
    {
        if (ccAddresses is { Count: > 0 }
            && ccAddresses.Any(static a => !string.IsNullOrWhiteSpace(a)))
        {
            throw new InvalidOperationException("CC_NOT_ALLOWED");
        }
    }

    public static void EnsureToMatchesEmployee(string? employeeEmail, string toAddress)
    {
        if (string.IsNullOrWhiteSpace(employeeEmail)
            || !string.Equals(employeeEmail.Trim(), toAddress.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("TO_MISMATCH");
        }
    }
}
