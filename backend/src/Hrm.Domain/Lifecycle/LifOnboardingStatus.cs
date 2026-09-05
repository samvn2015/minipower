namespace Hrm.Domain.Lifecycle;

public enum LifOnboardingStatus
{
    Open = 0,
    Closed = 1
}

/// <summary>Hệ thống cấp lúc on — LIF-FR-002 (cấm trì hoãn Git đến N+3).</summary>
public static class LifProvisionSystems
{
    public const string EmailCty = "EmailCty";
    public const string Git = "Git";
    public const string CrmSp = "CrmSp";
    public const string Chat = "Chat";

    public static readonly IReadOnlyList<string> All =
        [EmailCty, Git, CrmSp, Chat];

    public static bool IsKnown(string code) =>
        All.Any(x => string.Equals(x, code, StringComparison.OrdinalIgnoreCase));
}
