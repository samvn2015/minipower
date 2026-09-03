import type {
  ApiEnvelope,
  CurrentUser,
  EducationLevel,
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
    const message =
      typeof body === "object" &&
      body !== null &&
      "message" in body &&
      typeof (body as { message: unknown }).message === "string"
        ? (body as { message: string }).message
        : text;
    throw new Error(message);
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
