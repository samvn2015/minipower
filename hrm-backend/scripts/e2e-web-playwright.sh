#!/usr/bin/env bash
# Playwright E2E — hrm-web against local backend + Vite dev server.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
WEB="${WEB_URL:-http://127.0.0.1:5173}"
BASE="${BASE_URL:-http://localhost:5167}"

echo "========== Playwright preflight =========="
curl -sf -o /dev/null -w "backend: %{http_code}\n" "$BASE/api/ping"
curl -sf -o /dev/null -w "web: %{http_code}\n" "$WEB/"

cd "$ROOT/hrm-web"
export WEB_URL="$WEB"
export BASE_URL="$BASE"
npm run test:e2e

echo ""
echo "OK — Playwright E2E web passed"
