namespace Hrm.Domain.Payroll;

/// <summary>Trần N_tính vs ngày công chuẩn tháng — PAY-FR-007 · PAY-BR-002.</summary>
public static class PayrollWorkdayCap
{
    public static bool ExceedsCap(decimal nTinh, decimal standardWorkDays) =>
        nTinh > standardWorkDays;
}
