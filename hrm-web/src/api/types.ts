export type ApiEnvelope<T> = {
  data: T;
  traceId?: string;
  code?: string;
};

export type CurrentUser = {
  sub: string;
  name: string | null;
  roles: string[];
};

export type EmployeeListItem = {
  id: string;
  employeeCode: string;
  fullName: string | null;
  emailCty: string | null;
  orgUnitCode: string | null;
  hasContract: boolean;
  status: string;
};

export type EmployeeContract = {
  contractType: string;
  startDate: string;
  endDate: string | null;
  isProbation: boolean;
};

export type Seniority = {
  years: number;
  months: number;
  displayText: string;
  ruleCode: string;
};

export type EducationLevel = {
  code: string;
  name: string;
};

export type EmployeeDetail = {
  id: string;
  employeeCode: string;
  fullName: string | null;
  cccd: string | null;
  emailCty: string | null;
  taxId: string | null;
  orgUnitCode: string | null;
  educationLevelCode: string | null;
  educationLevelName: string | null;
  seniority: Seniority | null;
  contract: EmployeeContract | null;
  lineManagerEmployeeId: string | null;
  status: string;
};

export type DevPersona = {
  id: string;
  label: string;
  sub: string;
  email?: string;
};

export const DEV_PERSONAS: DevPersona[] = [
  { id: "hr", label: "HR / C&B (local-dev)", sub: "local-dev", email: "dev@company.local" },
  { id: "lm", label: "Line Manager (local-lm)", sub: "local-lm", email: "handover@company.local" },
  { id: "it", label: "IT (it-dev)", sub: "it-dev", email: "it@company.local" },
];

export type LineManagerChangeItem = {
  id: string;
  employeeId: string;
  employeeCode: string;
  employeeFullName: string | null;
  proposedLineManagerEmployeeId: string;
  proposedLineManagerCode: string;
  proposedLineManagerName: string | null;
  status: string;
  requestedByIdpSubject: string;
  requestedAtUtc: string;
  reviewedByIdpSubject?: string | null;
  reviewedAtUtc?: string | null;
  reviewNote?: string | null;
};

export type LineManagerChangeResult = {
  requestId: string;
  status: string;
};

export type IdentityAccount = {
  id: string;
  idpSubject: string;
  displayName: string | null;
  emailCty: string | null;
  employeeCode: string | null;
  status: string;
  roles: string[];
};

export type IdentityAccountAdminResult = {
  accountId: string;
  status: string;
  roles: string[];
};

export const IAM_ASSIGNABLE_ROLES = [
  "IAM-ROLE-NV",
  "IAM-ROLE-LM",
  "IAM-ROLE-HR",
  "IAM-ROLE-IT",
  "IAM-ROLE-PGD",
] as const;

export type LeaveType = {
  code: string;
  name: string;
  deductsAnnualBalance: boolean;
};

export type LeaveBalance = {
  year: number;
  entitledDays: number;
  usedDays: number;
  remainingDays: number;
};

export type LeaveRequestItem = {
  id: string;
  leaveTypeCode: string;
  leaveTypeName: string | null;
  fromDate: string;
  toDate: string;
  dayPart: string;
  totalDays: number;
  reason: string;
  handoverEmployeeId: string;
  status: string;
  isEmergency: boolean;
};

export type LeaveRequestPendingC1Item = {
  id: string;
  employeeCode: string;
  employeeFullName: string | null;
  leaveTypeCode: string;
  leaveTypeName: string | null;
  fromDate: string;
  toDate: string;
  dayPart: string;
  totalDays: number;
  reason: string;
  handoverEmployeeId: string;
  isEmergency: boolean;
  submittedAtUtc: string;
};

export type LeaveRequestActionResult = {
  id: string;
  status: string;
};

export type TimesheetTemplateColumn = {
  columnKey: string;
  displayName: string;
  sortOrder: number;
  isRequired: boolean;
  mapsTo: string;
};

export type TimesheetTemplate = {
  id: string;
  versionCode: string;
  name: string;
  status: string;
  publishedAtUtc: string | null;
  publishedByIdpSubject: string | null;
  columns: TimesheetTemplateColumn[];
};

export type TimesheetImportRow = {
  rowNumber: number;
  employeeCode: string | null;
  workDays: number | null;
  ot15: number | null;
  ot20: number | null;
  ot30: number | null;
  isOk: boolean;
  errorCode: string | null;
  errorMessage: string | null;
};

export type TimesheetImportBatch = {
  id: string;
  periodYm: string;
  templateVersionCode: string;
  status: string;
  totalRows: number;
  errorRows: number;
  hasMustErrors: boolean;
  fileName: string | null;
  rows: TimesheetImportRow[];
};

export type TimesheetCommitResult = {
  periodId: string;
  periodYm: string;
  status: string;
  lineCount: number;
};
