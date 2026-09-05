import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

const apiTarget = process.env.API_PROXY ?? "https://localhost:7006";

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5283,
    host: "127.0.0.1",
    proxy: {
      "/api": { target: apiTarget, changeOrigin: true, secure: false },
      "/health": { target: apiTarget, changeOrigin: true, secure: false },
      "/swagger": { target: apiTarget, changeOrigin: true, secure: false },
      "/v1": { target: apiTarget, changeOrigin: true, secure: false },
      "/dev": { target: apiTarget, changeOrigin: true, secure: false },
    },
  },
});
