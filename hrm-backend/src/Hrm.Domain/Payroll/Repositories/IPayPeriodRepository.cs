namespace Hrm.Domain.Payroll.Repositories;

public interface IPayPeriodGate
{
    /// <summary>True nếu kỳ PAY đã chốt — cấm bỏ chốt TIM (TIM-FR-012).</summary>
    Task<bool> IsClosedAsync(string periodYm, CancellationToken cancellationToken = default);
}

public interface IPayPeriodRepository : IPayPeriodGate
{
    /// <summary>Stub: đánh dấu kỳ PAY Closed (phục vụ TIM unlock gate / e2e).</summary>
    Task MarkClosedAsync(string periodYm, string closedByIdpSubject, CancellationToken cancellationToken = default);
}
