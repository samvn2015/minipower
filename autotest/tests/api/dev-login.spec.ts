import { expect, test } from "@playwright/test";

const apiBase = process.env.API_BASE ?? "http://localhost:5287";

test.describe("HRM API — POST /dev/login", () => {
  test("đúng tài khoản trả accessToken", async ({ request }) => {
    const res = await request.post(`${apiBase}/dev/login`, {
      data: { username: "admin@gmail.com", password: "Admin@123" },
    });
    expect(res.ok()).toBeTruthy();

    const body = await res.json();
    const payload = body.data ?? body;
    expect(payload.accessToken).toBeTruthy();
    expect(payload.sub).toBe("local-dev");
  });

  test("sai mật khẩu trả 401", async ({ request }) => {
    const res = await request.post(`${apiBase}/dev/login`, {
      data: { username: "admin@gmail.com", password: "SaiMatKhau@1" },
    });
    expect(res.status()).toBe(401);
  });

  test("thiếu password trả 400", async ({ request }) => {
    const res = await request.post(`${apiBase}/dev/login`, {
      data: { username: "admin@gmail.com", password: "" },
    });
    expect(res.status()).toBe(400);
  });
});
