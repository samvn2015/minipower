#!/usr/bin/env bash
# E2E full local — tất cả chức năng hrm-backend MVP.
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  local sub="$1" email="${2:-}"
  local url="$BASE/dev/token?sub=$sub"
  [[ -n "$email" ]] && url="$url&email=$email"
  curl -sf "$url" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}

auth_hdr() { echo "Authorization: Bearer $1"; }

echo "========== 1. Health / Ping =========="
curl -sf "$BASE/api/ping" | python3 -m json.tool
curl -sf -o /dev/null -w "health/live: %{http_code}\n" "$BASE/health/live"

echo "========== 2. IAM — dev token + /me =========="
HR_TOKEN=$(token "local-dev")
echo "GET /v1/iam/me (local-dev HR+NV)"
curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/iam/me" | python3 -m json.tool

IT_TOKEN=$(token "it-dev")
echo "GET /v1/iam/me (it-dev IT)"
curl -sf -H "$(auth_hdr "$IT_TOKEN")" "$BASE/v1/iam/me" | python3 -m json.tool

echo "========== 3. IAM admin (SCR-003) =========="
DEV_ACCOUNT="aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"
echo "GET /v1/iam/accounts"
curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/iam/accounts" | python3 -m json.tool
echo "GET /v1/iam/accounts/$DEV_ACCOUNT"
curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/iam/accounts/$DEV_ACCOUNT" | python3 -m json.tool
echo "POST assign IAM-ROLE-LM (temporary)"
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"roleCode":"IAM-ROLE-LM"}' \
  "$BASE/v1/iam/accounts/$DEV_ACCOUNT/roles" | python3 -m json.tool
echo "DELETE remove IAM-ROLE-LM"
curl -sf -X DELETE -H "$(auth_hdr "$HR_TOKEN")" \
  "$BASE/v1/iam/accounts/$DEV_ACCOUNT/roles/IAM-ROLE-LM" | python3 -m json.tool

echo "========== 4. EMP — list / get seed =========="
DEV_EMP="bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/emp/employees" | python3 -m json.tool
curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/emp/employees/$DEV_EMP" | python3 -m json.tool

echo "========== 5. EMP — create (org + contract) =========="
TS=$(date +%s)
NEW_CODE="MNV-E2E-$TS"
NEW_EMAIL="$NEW_CODE@test.local"
CREATE_JSON=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeCode\":\"$NEW_CODE\",\"fullName\":\"E2E NV\",\"emailCty\":\"$NEW_EMAIL\",\"orgUnitCode\":\"ORG-HQ\",\"contract\":{\"contractType\":\"PROBATION\",\"startDate\":\"2026-01-01\",\"endDate\":\"2026-06-30\",\"isProbation\":true}}" \
  "$BASE/v1/emp/employees")
echo "$CREATE_JSON" | python3 -m json.tool
NEW_EMP_ID=$(python3 -c 'import json,sys; print(json.loads(sys.argv[1])["id"])' "$CREATE_JSON")

echo "========== 6. EMP — PATCH (HR) =========="
curl -sf -X PATCH -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"fullName":"E2E NV Updated"}' \
  "$BASE/v1/emp/employees/$NEW_EMP_ID" | python3 -m json.tool

echo "========== 7. EMP — PATCH cấm đổi LM (400) =========="
HTTP=$(curl -s -o /tmp/emp-patch-lm.json -w "%{http_code}" -X PATCH \
  -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"lineManagerEmployeeId\":\"$DEV_EMP\"}" \
  "$BASE/v1/emp/employees/$NEW_EMP_ID")
echo "HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/emp-patch-lm.json; exit 1; }

echo "========== 8. IAM auto-provision (IAM-FR-017) =========="
PROV_SUB="provision-$TS"
PROV_TOKEN=$(token "$PROV_SUB" "$NEW_EMAIL")
curl -sf -H "$(auth_hdr "$PROV_TOKEN")" "$BASE/v1/iam/me" | python3 -m json.tool

echo "========== 9. EMP — tạo LM candidate =========="
LM_CODE="MNV-LM-$TS"
LM_JSON=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeCode\":\"$LM_CODE\",\"fullName\":\"E2E LM\",\"emailCty\":\"$LM_CODE@test.local\",\"orgUnitCode\":\"ORG-HQ\"}" \
  "$BASE/v1/emp/employees")
echo "$LM_JSON" | python3 -m json.tool
LM_EMP_ID=$(python3 -c 'import json,sys; print(json.loads(sys.argv[1])["id"])' "$LM_JSON")

echo "========== 10. SCR-005 — gửi đổi LM =========="
LM_REQ_JSON=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"proposedLineManagerEmployeeId\":\"$LM_EMP_ID\"}" \
  "$BASE/v1/emp/employees/$NEW_EMP_ID/line-manager-change-requests")
echo "$LM_REQ_JSON" | python3 -m json.tool
LM_REQ_ID=$(python3 -c 'import json,sys; print(json.loads(sys.argv[1])["requestId"])' "$LM_REQ_JSON")

echo "========== 11. SCR-006 — hàng chờ + duyệt =========="
curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/emp/line-manager-change-requests" | python3 -m json.tool
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" \
  "$BASE/v1/emp/line-manager-change-requests/$LM_REQ_ID/approve" | python3 -m json.tool

echo "========== 12. Verify LM đã ghi =========="
curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/emp/employees/$NEW_EMP_ID" | python3 -m json.tool

echo ""
echo "OK — E2E FULL passed (IAM + EMP + auto-provision + LM workflow)"
