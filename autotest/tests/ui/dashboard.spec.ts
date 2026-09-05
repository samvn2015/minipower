import { expect, test } from "@playwright/test";

test.describe("HRM UI smoke", () => {
  test("login screen loads", async ({ page }) => {
    await page.goto("/login");
    await expect(page.getByRole("heading", { name: "Đăng nhập (Development)" })).toBeVisible();
  });
});
