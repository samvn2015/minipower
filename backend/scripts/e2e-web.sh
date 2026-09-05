#!/usr/bin/env bash
# E2E web smoke — API paths used by hrm-web + frontend up check.
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"
WEB="${WEB_URL:-http://127.0.0.1:5173}"

token() {
  local sub="$1" email="${2:-}"
  local url="$BASE/dev/token?sub=$sub"
  [[ -n "$email" ]] && url="$url&email=$email"
  curl -sf "$url" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}

auth_hdr() { echo "Authorization: Bearer $1"; }

echo "========== 0. Frontend up =========="
curl -sf -o /dev/null -w "web: %{http_code}\n" "$WEB/"

echo "========== 1. NV self-service (EMP-SCR-003) =========="
DEV_TOKEN=$(token "local-dev" "dev@company.local")
echo "GET /v1/emp/employees/me"
curl -sf -H "$(auth_hdr "$DEV_TOKEN")" "$BASE/v1/emp/employees/me" | python3 -m json.tool
echo "PATCH self-service field"
curl -sf -X PATCH -H "$(auth_hdr "$DEV_TOKEN")" -H "Content-Type: application/json" \
  -d '{"fullName":"Dev IAM"}' \
  "$BASE/v1/emp/employees/bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb" | python3 -m json.tool

echo "========== 2. NV 403 list employees (EMP-TC-012) =========="
TS=$(date +%s)
NV_CODE="MNV-WEB-$TS"
NV_EMAIL="$NV_CODE@test.local"
HR_TOKEN=$(token "local-dev")
curl -sf -H "Authorization: Bearer $HR_TOKEN" -H "Content-Type: application/json" \
  -d "{\"employeeCode\":\"$NV_CODE\",\"fullName\":\"Web NV\",\"emailCty\":\"$NV_EMAIL\",\"orgUnitCode\":\"ORG-HQ\"}" \
  "$BASE/v1/emp/employees" > /dev/null
NV_TOKEN=$(token "web-nv-$TS" "$NV_EMAIL")
curl -sf -H "Authorization: Bearer $NV_TOKEN" "$BASE/v1/iam/me" > /dev/null
HTTP=$(curl -s -o /tmp/nv-list.json -w "%{http_code}" \
  -H "Authorization: Bearer $NV_TOKEN" "$BASE/v1/emp/employees")
echo "HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/nv-list.json; exit 1; }
echo "GET /v1/emp/employees/me (NV)"
curl -sf -H "Authorization: Bearer $NV_TOKEN" "$BASE/v1/emp/employees/me" | python3 -m json.tool

echo "========== 3. IAM admin API (SCR-003) =========="
HR_TOKEN=$(token "local-dev")
curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/iam/accounts" | python3 -m json.tool | head -30

echo "========== 4. Full API regression =========="
bash "$(dirname "$0")/e2e-full.sh"
bash "$(dirname "$0")/e2e-api-unhappy.sh"

echo "========== 5. EMP slice A (education, seniority, audit) =========="
bash "$(dirname "$0")/e2e-api-emp-slice-a.sh"

echo "========== 6. Playwright UI (hrm-web) =========="
bash "$(dirname "$0")/e2e-web-playwright.sh"

echo ""
echo "OK — E2E WEB smoke passed"
