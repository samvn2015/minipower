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

export type EmployeeDetail = {
  id: string;
  employeeCode: string;
  fullName: string | null;
  cccd: string | null;
  emailCty: string | null;
  taxId: string | null;
  orgUnitCode: string | null;
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
