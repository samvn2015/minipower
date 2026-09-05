import { defineConfig, devices } from "@playwright/test";

const apiBase = process.env.API_BASE ?? "http://localhost:5287";
const webBase = process.env.WEB_BASE ?? "http://127.0.0.1:5283";

export default defineConfig({
  testDir: "./tests",
  fullyParallel: false,
  forbidOnly: !!process.env.CI,
  retries: 0,
  workers: 1,
  reporter: [["list"]],
  use: {
    trace: "on-first-retry",
    channel: "chrome",
  },
  projects: [
    {
      name: "api",
      testMatch: /api\/.*\.spec\.ts/,
      use: { baseURL: apiBase },
    },
    {
      name: "ui",
      testMatch: /ui\/.*\.spec\.ts/,
      use: {
        ...devices["Desktop Chrome"],
        channel: "chrome",
        baseURL: webBase,
      },
    },
  ],
});
