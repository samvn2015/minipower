import { expect, test } from "@playwright/test";
import {
  API_BASE,
  createEmployeeViaApi,
  fetchDevToken,
  loginAsHr,
  unwrapApi,
} from "./helpers";

test.describe("HRM web E2E", () => {
  test.beforeEach(async ({ page }) => {
    await page.goto("/login");
    await page.evaluate(() => localStorage.clear());
  });

  test("login → profile (EMP-SCR-003)", async ({ page }) => {
    await loginAsHr(page);

    await expect(page).toHaveURL(/\/profile$/);
    await expect(page.getByRole("heading", { name: "Hồ sơ của tôi" })).toBeVisible();
    await expect(page.getByText("EMP-SCR-003")).toBeVisible();
    await expect(page.getByText("Mã NV:")).toBeVisible();
  });

  test("HR → IAM list (IAM-SCR-003)", async ({ page }) => {
    await loginAsHr(page);

    await page.getByRole("link", { name: "IAM" }).click();
    await expect(page).toHaveURL(/\/iam\/accounts$/);
    await expect(page.getByRole("heading", { name: "Tài khoản IAM" })).toBeVisible();
    await expect(page.getByRole("table")).toBeVisible();
    await expect(page.getByRole("link", { name: "Quản lý" }).first()).toBeVisible();
  });

  test("HR → employee list (EMP-SCR-001)", async ({ page }) => {
    await loginAsHr(page);

    await page.getByRole("link", { name: "Nhân viên" }).click();
    await expect(page).toHaveURL(/\/employees$/);
    await expect(page.getByRole("heading", { name: "Danh sách nhân viên" })).toBeVisible();
    await expect(page.getByRole("link", { name: "+ Tạo nhân viên" })).toBeVisible();
  });

  test("HR → LM queue (EMP-SCR-006)", async ({ page }) => {
    await loginAsHr(page);

    await page.getByRole("link", { name: "Duyệt LM" }).click();
    await expect(page).toHaveURL(/\/line-manager-changes$/);
    await expect(page.getByRole("heading", { name: "Duyệt đổi Line Manager" })).toBeVisible();
  });

  test("HR submit + approve LM change via web (SCR-005/006)", async ({ page, request }) => {
    const hrToken = await fetchDevToken(request, "local-dev", "dev@company.local");
    const ts = Date.now();
    const employee = await createEmployeeViaApi(request, hrToken, {
      employeeCode: `MNV-PW-${ts}`,
      fullName: "Playwright NV",
      emailCty: `pw-nv-${ts}@test.local`,
    });
    const lmCandidate = await createEmployeeViaApi(request, hrToken, {
      employeeCode: `MNV-PW-LM-${ts}`,
      fullName: "Playwright LM",
      emailCty: `pw-lm-${ts}@test.local`,
    });

    await loginAsHr(page);

    await page.goto(`/employees/${employee.id}`);
    await expect(page.getByRole("heading", { name: "Sửa hồ sơ" })).toBeVisible();

    await page.getByLabel("Line Manager mới").selectOption(lmCandidate.id);
    await page.getByRole("button", { name: "Gửi đề xuất đổi LM" }).click();
    await expect(page.getByText(/Đã gửi đề xuất/)).toBeVisible();

    await page.getByRole("link", { name: "Duyệt LM" }).click();
    const row = page.getByRole("row").filter({ hasText: employee.employeeCode });
    await expect(row).toBeVisible();
    await row.getByRole("button", { name: "Duyệt" }).click();
    await expect(row).toHaveCount(0, { timeout: 15_000 });

    const detailResponse = await request.get(`${API_BASE}/v1/emp/employees/${employee.id}`, {
      headers: { Authorization: `Bearer ${hrToken}` },
    });
    expect(detailResponse.ok()).toBeTruthy();
    const detail = unwrapApi(
      (await detailResponse.json()) as { lineManagerEmployeeId: string | null },
    );
    expect(detail.lineManagerEmployeeId).toBe(lmCandidate.id);
  });
});
