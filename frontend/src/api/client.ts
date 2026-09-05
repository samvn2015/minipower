import type {
  ApiEnvelope,
  CurrentUser,
  EducationLevel,
  EmpCatalogItem,
  EmployeeDetail,
  EmployeeListItem,
  IdentityAccount,
  IdentityAccountAdminResult,
  LeaveBalance,
  LeaveRequestActionResult,
  LeaveRequestItem,
  LeaveRequestPendingC1Item,
  LeaveType,
  LineManagerChangeItem,
  LineManagerChangeResult,
  TimesheetTemplate,
  TimesheetImportBatch,
  TimesheetCommitResult,
  TimesheetPeriod,
  TimesheetCloseResult,
  TimesheetUnlockResult,
  PayPeriod,
  PayRunResult,
  PayAllowanceCatalogItem,
  PayMonthlyAllowance,
  PayMonthlyAllowanceResult,
  PayExportResult,
  PayPayslip,
  ProbationCase,
  ProbationMilestone,
  ProbationReminder,
  ProbationReminderRunResult,
  ProbationMasterItem,
  ProbationEvaluation,
  LifOffboarding,
  LifOffChecklistBoard,
  LifNPlus3LockRunResult,
  LifOnboarding,
} from "./types";

const TOKEN_KEY = "hrm.accessToken";

export function getStoredToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

export function setStoredToken(token: string): void {
  localStorage.setItem(TOKEN_KEY, token);
}

export function clearStoredToken(): void {
  localStorage.removeItem(TOKEN_KEY);
}

function extractApiErrorMessage(body: unknown, fallback: string): string {
  if (typeof body !== "object" || body === null) return fallback;
  const record = body as Record<string, unknown>;
  const nested = record.error;
  if (typeof nested === "object" && nested !== null) {
    const err = nested as Record<string, unknown>;
    if (typeof err.systemMessage === "string" && err.systemMessage.trim()) {
      return err.systemMessage;
    }
    if (typeof err.message === "string" && err.message.trim()) {
      return err.message;
    }
  }
  if (typeof record.message === "string" && record.message.trim()) {
    return record.message;
  }
  if (typeof record.systemMessage === "string" && record.systemMessage.trim()) {
    return record.systemMessage;
  }
  return fallback;
}

async function parseJson<T>(response: Response): Promise<T> {
  const text = await response.text();
  if (!text) {
    throw new Error(`HTTP ${response.status}: empty response`);
  }

  let body: unknown;
  try {
    body = JSON.parse(text);
  } catch {
    throw new Error(`HTTP ${response.status}: invalid JSON`);
  }

  if (!response.ok) {
    throw new Error(extractApiErrorMessage(body, text));
  }

  return body as T;
}

async function apiFetch<T>(path: string, init?: RequestInit): Promise<T> {
  const token = getStoredToken();
  const headers = new Headers(init?.headers);
  headers.set("Accept", "application/json");
  if (init?.body && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }
  if (token) {
    headers.set("Authorization", `Bearer ${token}`);
  }

  const response = await fetch(path, { ...init, headers });
  return parseJson<T>(response);
}

function unwrap<T>(body: T | ApiEnvelope<T>): T {
  if (typeof body === "object" && body !== null && "data" in body) {
    return (body as ApiEnvelope<T>).data;
  }
  return body as T;
}

export async function fetchDevToken(sub: string, email?: string): Promise<string> {
  const params = new URLSearchParams({ sub });
  if (email) params.set("email", email);
  const body = await apiFetch<{ accessToken: string } | ApiEnvelope<{ accessToken: string }>>(
    `/dev/token?${params}`,
  );
  const token =
    "accessToken" in body && typeof body.accessToken === "string"
      ? body.accessToken
      : unwrap(body as ApiEnvelope<{ accessToken: string }>).accessToken;
  setStoredToken(token);
  return token;
}

export async function fetchCurrentUser(): Promise<CurrentUser> {
  const body = await apiFetch<ApiEnvelope<CurrentUser>>("/v1/iam/me");
  return unwrap(body);
}

export async function fetchEmployees(): Promise<EmployeeListItem[]> {
  const body = await apiFetch<ApiEnvelope<EmployeeListItem[]>>("/v1/emp/employees");
  return unwrap(body);
}

export async function fetchEmployee(id: string): Promise<EmployeeDetail> {
  const body = await apiFetch<ApiEnvelope<EmployeeDetail>>(`/v1/emp/employees/${id}`);
  return unwrap(body);
}

export type CreateEmployeePayload = {
  employeeCode: string;
  fullName?: string;
  emailCty?: string;
  orgUnitCode: string;
  educationLevelCode?: string;
  contract?: {
    contractType: string;
    startDate: string;
    endDate?: string;
    isProbation: boolean;
  };
};

export async function createEmployee(payload: CreateEmployeePayload): Promise<{ id: string }> {
  const body = await apiFetch<{ id: string } | ApiEnvelope<{ id: string }>>(
    "/v1/emp/employees",
    { method: "POST", body: JSON.stringify(payload) },
  );
  if ("id" in body && typeof body.id === "string") return { id: body.id };
  return unwrap(body as ApiEnvelope<{ id: string }>);
}

export type UpdateEmployeePayload = {
  fullName?: string;
  emailCty?: string;
  cccd?: string;
  taxId?: string;
  orgUnitCode?: string;
  educationLevelCode?: string;
};

export async function fetchEducationLevels(): Promise<EducationLevel[]> {
  const body = await apiFetch<ApiEnvelope<EducationLevel[]>>("/v1/emp/education-levels");
  return unwrap(body);
}

export async function fetchContractTypes(): Promise<EmpCatalogItem[]> {
  const body = await apiFetch<EmpCatalogItem[] | ApiEnvelope<EmpCatalogItem[]>>("/v1/emp/contract-types");
  if (Array.isArray(body)) return body;
  return unwrap(body as ApiEnvelope<EmpCatalogItem[]>);
}

export async function fetchMyEmployee(): Promise<EmployeeDetail> {
  const body = await apiFetch<ApiEnvelope<EmployeeDetail>>("/v1/emp/employees/me");
  return unwrap(body);
}

export async function updateEmployee(
  id: string,
  payload: UpdateEmployeePayload,
): Promise<void> {
  await apiFetch(`/v1/emp/employees/${id}`, {
    method: "PATCH",
    body: JSON.stringify(payload),
  });
}

export async function submitLineManagerChange(
  employeeId: string,
  proposedLineManagerEmployeeId: string,
): Promise<LineManagerChangeResult> {
  const body = await apiFetch<
    LineManagerChangeResult | ApiEnvelope<LineManagerChangeResult>
  >(`/v1/emp/employees/${employeeId}/line-manager-change-requests`, {
    method: "POST",
    body: JSON.stringify({ proposedLineManagerEmployeeId }),
  });
  if ("requestId" in body && typeof body.requestId === "string") return body;
  return unwrap(body as ApiEnvelope<LineManagerChangeResult>);
}

export async function fetchPendingLineManagerChanges(): Promise<LineManagerChangeItem[]> {
  const body = await apiFetch<ApiEnvelope<LineManagerChangeItem[]>>(
    "/v1/emp/line-manager-change-requests",
  );
  return unwrap(body);
}

export async function approveLineManagerChange(id: string): Promise<LineManagerChangeResult> {
  const body = await apiFetch<
    LineManagerChangeResult | ApiEnvelope<LineManagerChangeResult>
  >(`/v1/emp/line-manager-change-requests/${id}/approve`, { method: "POST" });
  if ("requestId" in body && typeof body.requestId === "string") return body;
  return unwrap(body as ApiEnvelope<LineManagerChangeResult>);
}

export async function rejectLineManagerChange(
  id: string,
  reviewNote?: string,
): Promise<LineManagerChangeResult> {
  const body = await apiFetch<
    LineManagerChangeResult | ApiEnvelope<LineManagerChangeResult>
  >(`/v1/emp/line-manager-change-requests/${id}/reject`, {
    method: "POST",
    body: JSON.stringify({ reviewNote: reviewNote ?? null }),
  });
  if ("requestId" in body && typeof body.requestId === "string") return body;
  return unwrap(body as ApiEnvelope<LineManagerChangeResult>);
}

export async function fetchIdentityAccounts(): Promise<IdentityAccount[]> {
  const body = await apiFetch<ApiEnvelope<IdentityAccount[]>>("/v1/iam/accounts");
  return unwrap(body);
}

export async function fetchIdentityAccount(id: string): Promise<IdentityAccount> {
  const body = await apiFetch<ApiEnvelope<IdentityAccount>>(`/v1/iam/accounts/${id}`);
  return unwrap(body);
}

export async function assignAccountRole(
  accountId: string,
  roleCode: string,
): Promise<IdentityAccountAdminResult> {
  const body = await apiFetch<
    IdentityAccountAdminResult | ApiEnvelope<IdentityAccountAdminResult>
  >(`/v1/iam/accounts/${accountId}/roles`, {
    method: "POST",
    body: JSON.stringify({ roleCode }),
  });
  return unwrap(body as ApiEnvelope<IdentityAccountAdminResult>);
}

export async function removeAccountRole(
  accountId: string,
  roleCode: string,
): Promise<IdentityAccountAdminResult> {
  const body = await apiFetch<
    IdentityAccountAdminResult | ApiEnvelope<IdentityAccountAdminResult>
  >(`/v1/iam/accounts/${accountId}/roles/${encodeURIComponent(roleCode)}`, {
    method: "DELETE",
  });
  return unwrap(body as ApiEnvelope<IdentityAccountAdminResult>);
}

export async function disableIdentityAccount(
  accountId: string,
): Promise<IdentityAccountAdminResult> {
  const body = await apiFetch<
    IdentityAccountAdminResult | ApiEnvelope<IdentityAccountAdminResult>
  >(`/v1/iam/accounts/${accountId}/disable`, { method: "POST" });
  return unwrap(body as ApiEnvelope<IdentityAccountAdminResult>);
}

export async function fetchLeaveTypes(): Promise<LeaveType[]> {
  const body = await apiFetch<ApiEnvelope<LeaveType[]>>("/v1/lev/leave-types");
  return unwrap(body);
}

export async function fetchMyLeaveBalance(year?: number): Promise<LeaveBalance> {
  const query = year ? `?year=${year}` : "";
  const body = await apiFetch<ApiEnvelope<LeaveBalance>>(`/v1/lev/leave-balances/me${query}`);
  return unwrap(body);
}

export async function fetchMyLeaveRequests(): Promise<LeaveRequestItem[]> {
  const body = await apiFetch<ApiEnvelope<LeaveRequestItem[]>>("/v1/lev/leave-requests/me");
  return unwrap(body);
}

export type CreateLeaveRequestPayload = {
  leaveTypeCode: string;
  fromDate: string;
  toDate: string;
  dayPart: string;
  reason: string;
  handoverEmployeeId: string;
  isEmergency?: boolean;
};

export async function createLeaveRequest(
  payload: CreateLeaveRequestPayload,
): Promise<{ id: string; status: string; totalDays: number }> {
  const body = await apiFetch<
    { id: string; status: string; totalDays: number } | ApiEnvelope<{ id: string; status: string; totalDays: number }>
  >("/v1/lev/leave-requests", { method: "POST", body: JSON.stringify(payload) });
  if ("id" in body && typeof body.id === "string") return body;
  return unwrap(body as ApiEnvelope<{ id: string; status: string; totalDays: number }>);
}

export async function fetchPendingLeaveRequestsC1(): Promise<LeaveRequestPendingC1Item[]> {
  const body = await apiFetch<ApiEnvelope<LeaveRequestPendingC1Item[]>>(
    "/v1/lev/leave-requests/pending-c1",
  );
  return unwrap(body);
}

export async function approveLeaveRequestC1(id: string): Promise<LeaveRequestActionResult> {
  const body = await apiFetch<
    LeaveRequestActionResult | ApiEnvelope<LeaveRequestActionResult>
  >(`/v1/lev/leave-requests/${id}/c1/approve`, { method: "POST" });
  if ("id" in body && typeof body.id === "string") return body;
  return unwrap(body as ApiEnvelope<LeaveRequestActionResult>);
}

export async function rejectLeaveRequestC1(
  id: string,
  reviewNote?: string,
): Promise<LeaveRequestActionResult> {
  const body = await apiFetch<
    LeaveRequestActionResult | ApiEnvelope<LeaveRequestActionResult>
  >(`/v1/lev/leave-requests/${id}/c1/reject`, {
    method: "POST",
    body: JSON.stringify({ reviewNote: reviewNote ?? null }),
  });
  if ("id" in body && typeof body.id === "string") return body;
  return unwrap(body as ApiEnvelope<LeaveRequestActionResult>);
}

export async function cancelLeaveRequest(id: string): Promise<LeaveRequestActionResult> {
  const body = await apiFetch<
    LeaveRequestActionResult | ApiEnvelope<LeaveRequestActionResult>
  >(`/v1/lev/leave-requests/${id}/cancel`, { method: "POST" });
  if ("id" in body && typeof body.id === "string") return body;
  return unwrap(body as ApiEnvelope<LeaveRequestActionResult>);
}

export async function fetchPendingLeaveRequestsC2(): Promise<LeaveRequestPendingC1Item[]> {
  const body = await apiFetch<ApiEnvelope<LeaveRequestPendingC1Item[]>>(
    "/v1/lev/leave-requests/pending-c2",
  );
  return unwrap(body);
}

export async function approveLeaveRequestC2(id: string): Promise<LeaveRequestActionResult> {
  const body = await apiFetch<
    LeaveRequestActionResult | ApiEnvelope<LeaveRequestActionResult>
  >(`/v1/lev/leave-requests/${id}/c2/approve`, { method: "POST" });
  if ("id" in body && typeof body.id === "string") return body;
  return unwrap(body as ApiEnvelope<LeaveRequestActionResult>);
}

export async function rejectLeaveRequestC2(
  id: string,
  reviewNote?: string,
): Promise<LeaveRequestActionResult> {
  const body = await apiFetch<
    LeaveRequestActionResult | ApiEnvelope<LeaveRequestActionResult>
  >(`/v1/lev/leave-requests/${id}/c2/reject`, {
    method: "POST",
    body: JSON.stringify({ reviewNote: reviewNote ?? null }),
  });
  if ("id" in body && typeof body.id === "string") return body;
  return unwrap(body as ApiEnvelope<LeaveRequestActionResult>);
}

export async function fetchActiveTimesheetTemplate(): Promise<TimesheetTemplate> {
  const body = await apiFetch<ApiEnvelope<TimesheetTemplate>>("/v1/tim/templates/active");
  return unwrap(body);
}

export async function fetchTimesheetTemplates(): Promise<TimesheetTemplate[]> {
  const body = await apiFetch<ApiEnvelope<TimesheetTemplate[]>>("/v1/tim/templates");
  return unwrap(body);
}

export async function createTimesheetTemplate(payload: {
  versionCode: string;
  name: string;
  columns: {
    columnKey: string;
    displayName: string;
    sortOrder: number;
    isRequired: boolean;
    mapsTo: string;
  }[];
}): Promise<{ id: string; versionCode: string; status: string }> {
  const body = await apiFetch<
    | { id: string; versionCode: string; status: string }
    | ApiEnvelope<{ id: string; versionCode: string; status: string }>
  >("/v1/tim/templates", { method: "POST", body: JSON.stringify(payload) });
  if ("id" in body && typeof body.id === "string") return body;
  return unwrap(body as ApiEnvelope<{ id: string; versionCode: string; status: string }>);
}

export async function publishTimesheetTemplate(
  id: string,
): Promise<{ id: string; versionCode: string; status: string }> {
  const body = await apiFetch<
    | { id: string; versionCode: string; status: string }
    | ApiEnvelope<{ id: string; versionCode: string; status: string }>
  >(`/v1/tim/templates/${id}/publish`, { method: "POST" });
  if ("id" in body && typeof body.id === "string") return body;
  return unwrap(body as ApiEnvelope<{ id: string; versionCode: string; status: string }>);
}

export async function previewTimesheetImport(payload: {
  periodYm: string;
  templateVersionCode: string;
  fileName?: string;
  rows: {
    rowNumber: number;
    employeeCode?: string;
    workDays?: number;
    ot15?: number;
    ot20?: number;
    ot30?: number;
    otUnclassified?: number;
  }[];
}): Promise<TimesheetImportBatch> {
  const body = await apiFetch<TimesheetImportBatch | ApiEnvelope<TimesheetImportBatch>>(
    "/v1/tim/imports",
    { method: "POST", body: JSON.stringify(payload) },
  );
  if ("id" in body && typeof body.id === "string") return body;
  return unwrap(body as ApiEnvelope<TimesheetImportBatch>);
}

export async function commitTimesheetImport(id: string): Promise<TimesheetCommitResult> {
  const body = await apiFetch<TimesheetCommitResult | ApiEnvelope<TimesheetCommitResult>>(
    `/v1/tim/imports/${id}/commit`,
    { method: "POST" },
  );
  if ("periodId" in body && typeof body.periodId === "string") return body;
  return unwrap(body as ApiEnvelope<TimesheetCommitResult>);
}

export async function fetchTimesheetPeriods(): Promise<TimesheetPeriod[]> {
  const body = await apiFetch<TimesheetPeriod[] | ApiEnvelope<TimesheetPeriod[]>>("/v1/tim/periods");
  if (Array.isArray(body)) return body;
  return unwrap(body as ApiEnvelope<TimesheetPeriod[]>);
}

export async function closeTimesheetPeriod(ym: string): Promise<TimesheetCloseResult> {
  const body = await apiFetch<TimesheetCloseResult | ApiEnvelope<TimesheetCloseResult>>(
    `/v1/tim/periods/${ym}/close`,
    { method: "POST" },
  );
  if ("periodId" in body && typeof body.periodId === "string") return body;
  return unwrap(body as ApiEnvelope<TimesheetCloseResult>);
}

export async function unlockTimesheetPeriod(ym: string): Promise<TimesheetUnlockResult> {
  const body = await apiFetch<TimesheetUnlockResult | ApiEnvelope<TimesheetUnlockResult>>(
    `/v1/tim/periods/${ym}/unlock`,
    { method: "POST" },
  );
  if ("periodId" in body && typeof body.periodId === "string") return body;
  return unwrap(body as ApiEnvelope<TimesheetUnlockResult>);
}

export async function fetchPayrollPeriod(ym: string): Promise<PayPeriod> {
  const body = await apiFetch<PayPeriod | ApiEnvelope<PayPeriod>>(`/v1/pay/periods/${ym}`);
  if ("id" in body && typeof body.id === "string") return body;
  return unwrap(body as ApiEnvelope<PayPeriod>);
}

export async function runPayrollPeriod(ym: string): Promise<PayRunResult> {
  const body = await apiFetch<PayRunResult | ApiEnvelope<PayRunResult>>(
    `/v1/pay/periods/${ym}/run`,
    { method: "POST" },
  );
  if ("periodId" in body && typeof body.periodId === "string") return body;
  return unwrap(body as ApiEnvelope<PayRunResult>);
}

export async function closePayrollPeriod(ym: string): Promise<PayRunResult> {
  const body = await apiFetch<PayRunResult | ApiEnvelope<PayRunResult>>(
    `/v1/pay/periods/${ym}/close`,
    { method: "POST" },
  );
  if ("periodId" in body && typeof body.periodId === "string") return body;
  return unwrap(body as ApiEnvelope<PayRunResult>);
}

export async function fetchPayAllowanceCatalog(): Promise<PayAllowanceCatalogItem[]> {
  const body = await apiFetch<PayAllowanceCatalogItem[] | ApiEnvelope<PayAllowanceCatalogItem[]>>(
    "/v1/pay/allowance-catalog",
  );
  if (Array.isArray(body)) return body;
  return unwrap(body as ApiEnvelope<PayAllowanceCatalogItem[]>);
}

export async function fetchPayMonthlyAllowances(ym: string): Promise<PayMonthlyAllowance[]> {
  const body = await apiFetch<PayMonthlyAllowance[] | ApiEnvelope<PayMonthlyAllowance[]>>(
    `/v1/pay/monthly-allowances/${ym}`,
  );
  if (Array.isArray(body)) return body;
  return unwrap(body as ApiEnvelope<PayMonthlyAllowance[]>);
}

export async function upsertPayMonthlyAllowance(payload: {
  periodYm: string;
  employeeCode: string;
  code: string;
  amount: number;
}): Promise<PayMonthlyAllowanceResult> {
  const body = await apiFetch<PayMonthlyAllowanceResult | ApiEnvelope<PayMonthlyAllowanceResult>>(
    "/v1/pay/monthly-allowances",
    { method: "POST", body: JSON.stringify(payload) },
  );
  if ("periodYm" in body && typeof body.periodYm === "string") return body;
  return unwrap(body as ApiEnvelope<PayMonthlyAllowanceResult>);
}

export async function fetchMyPayslips(): Promise<PayPayslip[]> {
  const body = await apiFetch<PayPayslip[] | ApiEnvelope<PayPayslip[]>>("/v1/pay/payslips/me");
  if (Array.isArray(body)) return body;
  return unwrap(body as ApiEnvelope<PayPayslip[]>);
}

export async function fetchPayslip(id: string): Promise<PayPayslip> {
  const body = await apiFetch<PayPayslip | ApiEnvelope<PayPayslip>>(`/v1/pay/payslips/${id}`);
  if ("id" in body && typeof body.id === "string") return body;
  return unwrap(body as ApiEnvelope<PayPayslip>);
}

export async function exportPayrollPeriod(
  ym: string,
  payload: { includePdf: boolean; includeEmail: boolean; ccAddresses?: string[] },
): Promise<PayExportResult> {
  const body = await apiFetch<PayExportResult | ApiEnvelope<PayExportResult>>(
    `/v1/pay/periods/${ym}/export`,
    { method: "POST", body: JSON.stringify(payload) },
  );
  if ("periodYm" in body && typeof body.periodYm === "string") return body;
  return unwrap(body as ApiEnvelope<PayExportResult>);
}

export async function fetchProbationCases(): Promise<ProbationCase[]> {
  const body = await apiFetch<ProbationCase[] | ApiEnvelope<ProbationCase[]>>("/v1/prb/cases");
  if (Array.isArray(body)) return body;
  return unwrap(body as ApiEnvelope<ProbationCase[]>);
}

export async function fetchMyProbationMilestones(): Promise<ProbationMilestone> {
  const body = await apiFetch<ProbationMilestone | ApiEnvelope<ProbationMilestone>>(
    "/v1/prb/milestones/me",
  );
  if ("employeeCode" in body && typeof body.employeeCode === "string") return body;
  return unwrap(body as ApiEnvelope<ProbationMilestone>);
}

export async function runProbationReminders(asOfDate?: string): Promise<ProbationReminderRunResult> {
  const body = await apiFetch<ProbationReminderRunResult | ApiEnvelope<ProbationReminderRunResult>>(
    "/v1/prb/jobs/reminders/run",
    { method: "POST", body: JSON.stringify({ asOfDate: asOfDate ?? null }) },
  );
  if ("asOfDate" in body && typeof body.asOfDate === "string") return body;
  return unwrap(body as ApiEnvelope<ProbationReminderRunResult>);
}

export async function fetchProbationReminders(kind?: string): Promise<ProbationReminder[]> {
  const q = kind ? `?kind=${encodeURIComponent(kind)}` : "";
  const body = await apiFetch<ProbationReminder[] | ApiEnvelope<ProbationReminder[]>>(
    `/v1/prb/reminders${q}`,
  );
  if (Array.isArray(body)) return body;
  return unwrap(body as ApiEnvelope<ProbationReminder[]>);
}

export async function fetchProbationOutcomes(): Promise<ProbationMasterItem[]> {
  const body = await apiFetch<ProbationMasterItem[] | ApiEnvelope<ProbationMasterItem[]>>(
    "/v1/prb/masters/outcomes",
  );
  if (Array.isArray(body)) return body;
  return unwrap(body as ApiEnvelope<ProbationMasterItem[]>);
}

export async function fetchProbationCriteria(): Promise<ProbationMasterItem[]> {
  const body = await apiFetch<ProbationMasterItem[] | ApiEnvelope<ProbationMasterItem[]>>(
    "/v1/prb/masters/criteria",
  );
  if (Array.isArray(body)) return body;
  return unwrap(body as ApiEnvelope<ProbationMasterItem[]>);
}

export async function fetchProbationExtendDurations(): Promise<ProbationMasterItem[]> {
  const body = await apiFetch<ProbationMasterItem[] | ApiEnvelope<ProbationMasterItem[]>>(
    "/v1/prb/masters/extend-durations",
  );
  if (Array.isArray(body)) return body;
  return unwrap(body as ApiEnvelope<ProbationMasterItem[]>);
}

export async function proposeProbationEvaluation(
  employeeId: string,
  payload: { outcomeCode: string; note?: string },
): Promise<ProbationEvaluation> {
  const body = await apiFetch<ProbationEvaluation | ApiEnvelope<ProbationEvaluation>>(
    `/v1/prb/evaluations/${employeeId}/propose`,
    { method: "POST", body: JSON.stringify(payload) },
  );
  if ("employeeCode" in body && typeof body.employeeCode === "string") return body;
  return unwrap(body as ApiEnvelope<ProbationEvaluation>);
}

export async function decideProbationEvaluation(
  employeeId: string,
  payload: { outcomeCode: string; note?: string; extendDurationCode?: string },
): Promise<ProbationEvaluation> {
  const body = await apiFetch<ProbationEvaluation | ApiEnvelope<ProbationEvaluation>>(
    `/v1/prb/evaluations/${employeeId}/decide`,
    { method: "POST", body: JSON.stringify(payload) },
  );
  if ("employeeCode" in body && typeof body.employeeCode === "string") return body;
  return unwrap(body as ApiEnvelope<ProbationEvaluation>);
}

export async function fetchLifOffboardings(): Promise<LifOffboarding[]> {
  const body = await apiFetch<LifOffboarding[] | ApiEnvelope<LifOffboarding[]>>("/v1/lif/offboarding");
  if (Array.isArray(body)) return body;
  return unwrap(body as ApiEnvelope<LifOffboarding[]>);
}

export async function confirmLifOffboardingN(
  caseId: string,
  lastWorkingDayN: string,
): Promise<LifOffboarding> {
  const body = await apiFetch<LifOffboarding | ApiEnvelope<LifOffboarding>>(
    `/v1/lif/offboarding/${caseId}/confirm-n`,
    { method: "POST", body: JSON.stringify({ lastWorkingDayN }) },
  );
  if ("employeeCode" in body && typeof body.employeeCode === "string") return body;
  return unwrap(body as ApiEnvelope<LifOffboarding>);
}

export async function fetchLifOffboardingChecklist(caseId: string): Promise<LifOffChecklistBoard> {
  const body = await apiFetch<LifOffChecklistBoard | ApiEnvelope<LifOffChecklistBoard>>(
    `/v1/lif/offboarding/${caseId}/checklist`,
  );
  if ("caseId" in body && typeof body.caseId === "string") return body;
  return unwrap(body as ApiEnvelope<LifOffChecklistBoard>);
}

export async function upsertLifOffChecklistTick(
  caseId: string,
  itemCode: string,
  isChecked: boolean,
): Promise<LifOffChecklistBoard> {
  const body = await apiFetch<LifOffChecklistBoard | ApiEnvelope<LifOffChecklistBoard>>(
    `/v1/lif/offboarding/${caseId}/checklist/${encodeURIComponent(itemCode)}`,
    { method: "PUT", body: JSON.stringify({ isChecked }) },
  );
  if ("caseId" in body && typeof body.caseId === "string") return body;
  return unwrap(body as ApiEnvelope<LifOffChecklistBoard>);
}

export async function closeLifOffboarding(caseId: string): Promise<LifOffboarding> {
  const body = await apiFetch<LifOffboarding | ApiEnvelope<LifOffboarding>>(
    `/v1/lif/offboarding/${caseId}/close`,
    { method: "POST", body: "{}" },
  );
  if ("employeeCode" in body && typeof body.employeeCode === "string") return body;
  return unwrap(body as ApiEnvelope<LifOffboarding>);
}

/** IT/PGD early CR lock Git+CRM SP — không CRM sales (LIF-FR-007/008). */
export async function applyLifOffboardingLocks(
  caseId: string,
  payload?: { asOfDate?: string; earlyCrReason?: string },
): Promise<LifOffboarding> {
  const body = await apiFetch<LifOffboarding | ApiEnvelope<LifOffboarding>>(
    `/v1/lif/offboarding/${caseId}/locks`,
    {
      method: "POST",
      body: JSON.stringify({
        asOfDate: payload?.asOfDate ?? null,
        earlyCrReason: payload?.earlyCrReason ?? null,
      }),
    },
  );
  if ("employeeCode" in body && typeof body.employeeCode === "string") return body;
  return unwrap(body as ApiEnvelope<LifOffboarding>);
}

export async function runLifNPlus3Locks(asOfDate?: string): Promise<LifNPlus3LockRunResult> {
  const body = await apiFetch<LifNPlus3LockRunResult | ApiEnvelope<LifNPlus3LockRunResult>>(
    "/v1/lif/offboarding/jobs/nplus3-locks",
    { method: "POST", body: JSON.stringify({ asOfDate: asOfDate ?? null }) },
  );
  if ("locked" in body && typeof body.locked === "number") return body;
  return unwrap(body as ApiEnvelope<LifNPlus3LockRunResult>);
}

export async function fetchLifOnboardings(): Promise<LifOnboarding[]> {
  const body = await apiFetch<LifOnboarding[] | ApiEnvelope<LifOnboarding[]>>("/v1/lif/onboarding");
  if (Array.isArray(body)) return body;
  return unwrap(body as ApiEnvelope<LifOnboarding[]>);
}

export async function createLifOnboarding(employeeId: string, note?: string): Promise<LifOnboarding> {
  const body = await apiFetch<LifOnboarding | ApiEnvelope<LifOnboarding>>("/v1/lif/onboarding", {
    method: "POST",
    body: JSON.stringify({ employeeId, note: note ?? null }),
  });
  if ("employeeCode" in body && typeof body.employeeCode === "string") return body;
  return unwrap(body as ApiEnvelope<LifOnboarding>);
}

export async function fetchLifOnboardingChecklist(caseId: string): Promise<LifOffChecklistBoard> {
  const body = await apiFetch<LifOffChecklistBoard | ApiEnvelope<LifOffChecklistBoard>>(
    `/v1/lif/onboarding/${caseId}/checklist`,
  );
  if ("caseId" in body && typeof body.caseId === "string") return body;
  return unwrap(body as ApiEnvelope<LifOffChecklistBoard>);
}

export async function upsertLifOnChecklistTick(
  caseId: string,
  itemCode: string,
  isChecked: boolean,
): Promise<LifOffChecklistBoard> {
  const body = await apiFetch<LifOffChecklistBoard | ApiEnvelope<LifOffChecklistBoard>>(
    `/v1/lif/onboarding/${caseId}/checklist/${encodeURIComponent(itemCode)}`,
    { method: "PUT", body: JSON.stringify({ isChecked }) },
  );
  if ("caseId" in body && typeof body.caseId === "string") return body;
  return unwrap(body as ApiEnvelope<LifOffChecklistBoard>);
}

export async function markLifOnboardingProvisioned(
  caseId: string,
  systemCode: string,
  deferGitToNPlus3 = false,
): Promise<LifOnboarding> {
  const body = await apiFetch<LifOnboarding | ApiEnvelope<LifOnboarding>>(
    `/v1/lif/onboarding/${caseId}/provisions/${encodeURIComponent(systemCode)}`,
    { method: "POST", body: JSON.stringify({ deferGitToNPlus3 }) },
  );
  if ("employeeCode" in body && typeof body.employeeCode === "string") return body;
  return unwrap(body as ApiEnvelope<LifOnboarding>);
}

export async function closeLifOnboarding(caseId: string): Promise<LifOnboarding> {
  const body = await apiFetch<LifOnboarding | ApiEnvelope<LifOnboarding>>(
    `/v1/lif/onboarding/${caseId}/close`,
    { method: "POST", body: "{}" },
  );
  if ("employeeCode" in body && typeof body.employeeCode === "string") return body;
  return unwrap(body as ApiEnvelope<LifOnboarding>);
}
