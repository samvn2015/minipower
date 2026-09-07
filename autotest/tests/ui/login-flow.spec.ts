import { expect, test } from "@playwright/test";

/**
 * Luồng đăng nhập username/password → trang chủ sau đăng nhập.
 *
 * Tài khoản khai trong `DevAuth:Accounts` (appsettings.Development.json), Host đối chiếu
 * rồi cấp JWT local qua `POST /dev/login`. KHÔNG lưu password trong DB — DOC-11 §3.1
 * giữ nguyên. Endpoint trả 404 ngoài Development nên Prod vẫn Lark SSO (ADR-007).
 *
 * App không có route `/dashboard`; sau đăng nhập điều hướng về `/profile`
 * (persona IT chưa gắn MNV thì về `/iam/accounts` — IAM-FR-017).
 */
const USERNAME = "admin@gmail.com";
const PASSWORD = "Admin@123";

test.describe("HRM UI — đăng nhập username/password", () => {
  test("mở /login → đăng nhập admin@gmail.com → vào trang chủ", async ({ page }) => {
    await page.goto("/login");
    await expect(page.getByRole("heading", { name: "Đăng nhập (Development)" })).toBeVisible();

    await page.getByLabel("Tên đăng nhập").fill(USERNAME);
    await page.getByLabel("Mật khẩu").fill(PASSWORD);
    await page.getByRole("button", { name: "Đăng nhập" }).click();

    await expect(page).toHaveURL(/\/profile$/);
    await expect(page.getByRole("heading", { name: "HRM — MVP" })).toBeVisible();
    await expect(page.getByRole("heading", { name: "Hồ sơ của tôi" })).toBeVisible();
  });

  test("sai mật khẩu thì báo lỗi và ở lại /login", async ({ page }) => {
    await page.goto("/login");

    await page.getByLabel("Tên đăng nhập").fill(USERNAME);
    await page.getByLabel("Mật khẩu").fill("SaiMatKhau@1");
    await page.getByRole("button", { name: "Đăng nhập" }).click();

    await expect(page.locator(".error-box")).toBeVisible();
    await expect(page).toHaveURL(/\/login$/);
  });

  test("chưa đăng nhập thì route được bảo vệ đẩy về /login", async ({ page }) => {
    await page.goto("/profile");
    await expect(page).toHaveURL(/\/login$/);
    await expect(page.getByRole("heading", { name: "Đăng nhập (Development)" })).toBeVisible();
  });
});
