namespace Hrm.Domain.Leave;

/// <summary>Kênh thông báo phép — cấm CRM sales (LEV-FR-009 / BR-011).</summary>
public static class LeaveNotificationChannels
{
    public const string Email = "Email";
    public const string InApp = "InApp";

    public static bool IsAllowed(string channel) =>
        string.Equals(channel, Email, StringComparison.OrdinalIgnoreCase)
        || string.Equals(channel, InApp, StringComparison.OrdinalIgnoreCase);

    public static bool IsCrmSales(string channel) =>
        channel.Contains("CRM", StringComparison.OrdinalIgnoreCase)
        || channel.Contains("Sales", StringComparison.OrdinalIgnoreCase);
}

public static class LeaveNotificationEvents
{
    public const string Submitted = "Submitted";
    public const string C1Approved = "C1Approved";
    public const string C1Rejected = "C1Rejected";
    public const string C2Approved = "C2Approved";
    public const string C2Rejected = "C2Rejected";
    public const string Cancelled = "Cancelled";
}
