import { expect, test } from "@playwright/test";

const apiBase = process.env.API_BASE ?? "http://localhost:5287";

test.describe("HRM API smoke", () => {
  test("GET /api/ping returns ok", async ({ request }) => {
    const res = await request.get(`${apiBase}/api/ping`);
    expect(res.ok()).toBeTruthy();
    const body = await res.json();
    const payload = body.data ?? body;
    expect(payload.status).toBe("ok");
    expect(payload.product).toBe("Hrm");
  });

  test("GET /health/live is healthy", async ({ request }) => {
    const res = await request.get(`${apiBase}/health/live`);
    expect(res.ok()).toBeTruthy();
  });

  test("Swagger UI is reachable", async ({ request }) => {
    const res = await request.get(`${apiBase}/swagger/index.html`);
    expect(res.ok()).toBeTruthy();
    const html = await res.text();
    expect(html.toLowerCase()).toContain("swagger");
  });
});
