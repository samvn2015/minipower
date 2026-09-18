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

    // LEV — doc-review pass 3 B1 (DEC-ARC-033): C1/C2/huỷ phép phải có dòng audit bất biến (NFR-005).
    public const string LeaveRequestC1Approved = "LeaveRequestC1Approved";
    public const string LeaveRequestC1Rejected = "LeaveRequestC1Rejected";
    public const string LeaveRequestC2Approved = "LeaveRequestC2Approved";
    public const string LeaveRequestC2Rejected = "LeaveRequestC2Rejected";
    public const string LeaveRequestCancelled = "LeaveRequestCancelled";

    // IAM — cùng slice B1; nền cho audit đăng nhập/khoá sau ADR-012.
    public const string IamRoleAssigned = "IamRoleAssigned";
    public const string IamRoleRemoved = "IamRoleRemoved";
    public const string IamAccountDisabled = "IamAccountDisabled";
    public const string LifOffboardingNConfirmed = "LifOffboardingNConfirmed";
    public const string LifOffboardingAccessLocked = "LifOffboardingAccessLocked";
    public const string TimesheetTemplatePublished = "TimesheetTemplatePublished";
    public const string TimesheetImportCommitted = "TimesheetImportCommitted";
    public const string TimesheetPeriodClosed = "TimesheetPeriodClosed";
    public const string TimesheetPeriodUnlocked = "TimesheetPeriodUnlocked";

    // ADR-005 RK-06 — dấu vết MỖI lần job chạy, kể cả khi không sinh gì.
    // Không có dòng này thì "job chạy mà không có việc" và "job không hề chạy"
    // là không phân biệt được → NFR-009 hỏng trong im lặng.
    public const string ProbationRemindersJobRan = "ProbationRemindersJobRan";

    /// <inheritdoc cref="ProbationRemindersJobRan"/>
    public const string LifNPlus3LocksJobRan = "LifNPlus3LocksJobRan";
}
