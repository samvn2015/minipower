namespace Hrm.Domain.Employees;

public static class EmpAuditActions
{
    public const string EmployeeCreated = "EmployeeCreated";
    public const string EmployeeUpdated = "EmployeeUpdated";
    public const string LmChangeSubmitted = "LmChangeSubmitted";
    public const string LmChangeApproved = "LmChangeApproved";
    public const string LmChangeRejected = "LmChangeRejected";
    public const string ProbationDecided = "ProbationDecided";
    public const string PayslipViewed = "PayslipViewed";
    public const string LifOffboardingNConfirmed = "LifOffboardingNConfirmed";
    public const string LifOffboardingAccessLocked = "LifOffboardingAccessLocked";
}
