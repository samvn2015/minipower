import type { APIRequestContext, Page } from "@playwright/test";

export const API_BASE = process.env.BASE_URL ?? "http://localhost:5167";

type ApiEnvelope<T> = { data: T };

export function unwrapApi<T>(body: T | ApiEnvelope<T>): T {
  if (typeof body === "object" && body !== null && "data" in body) {
    return (body as ApiEnvelope<T>).data;
  }
  return body as T;
}

export async function fetchDevToken(
  request: APIRequestContext,
  sub: string,
  email?: string,
): Promise<string> {
  const params = new URLSearchParams({ sub });
  if (email) params.set("email", email);

  const response = await request.get(`${API_BASE}/dev/token?${params.toString()}`);
  if (!response.ok()) {
    throw new Error(`dev/token failed: HTTP ${response.status()}`);
  }

  const body = (await response.json()) as { accessToken: string };
  return body.accessToken;
}

export async function loginAsHr(page: Page): Promise<void> {
  await page.goto("/login");
  await page
    .locator(".login-option")
    .filter({ hasText: "HR / C&B" })
    .getByRole("button", { name: "Vào ứng dụng" })
    .click();
  await page.getByRole("heading", { name: "Hồ sơ của tôi" }).waitFor();
}

export async function createEmployeeViaApi(
  request: APIRequestContext,
  hrToken: string,
  payload: {
    employeeCode: string;
    fullName: string;
    emailCty?: string;
    orgUnitCode?: string;
  },
): Promise<{ id: string; employeeCode: string }> {
  const response = await request.post(`${API_BASE}/v1/emp/employees`, {
    headers: {
      Authorization: `Bearer ${hrToken}`,
      "Content-Type": "application/json",
    },
    data: {
      orgUnitCode: "ORG-HQ",
      ...payload,
    },
  });

  if (!response.ok()) {
    throw new Error(`create employee failed: HTTP ${response.status()} ${await response.text()}`);
  }

  const body = unwrapApi(await response.json());
  return body;
}
