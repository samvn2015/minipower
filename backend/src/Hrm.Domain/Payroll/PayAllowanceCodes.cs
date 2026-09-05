namespace Hrm.Domain.Payroll;

/// <summary>Mã PC seed — danh mục mở (PAY-BR-005).</summary>
public static class PayAllowanceCodes
{
    public const string Meal = "PC-ANTRUA";

    public const string Fuel = "PC-XANG";

    public const string Responsibility = "PC-TRACHNHIEM";

    public const string Phone = "PC-DIENTHOAI";

    /// <summary>Tạm ứng — trừ thực lĩnh, không cộng gross.</summary>
    public const string Advance = "PC-TAMUNG";

    public static bool IsAdvance(string code) =>
        string.Equals(code, Advance, StringComparison.OrdinalIgnoreCase);

    public static bool IsMealTaxExempt(string code) =>
        string.Equals(code, Meal, StringComparison.OrdinalIgnoreCase);
}
