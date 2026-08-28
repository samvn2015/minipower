#!/usr/bin/env bash
# E2E smoke local — cần Host Development + Postgres (User Secrets).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"
SUB="${DEV_SUB:-local-dev}"

echo "== dev token ($SUB) =="
TOKEN_JSON=$(curl -sf "$BASE/dev/token?sub=$SUB")
TOKEN=$(python3 - <<'PY' "$TOKEN_JSON"
import json, sys
print(json.loads(sys.argv[1])["accessToken"])
PY
)
AUTH=(-H "Authorization: Bearer $TOKEN")

echo "== GET /v1/iam/me =="
curl -sf "${AUTH[@]}" "$BASE/v1/iam/me" | python3 -m json.tool

echo "== GET /v1/emp/employees (HR list) =="
curl -sf "${AUTH[@]}" "$BASE/v1/emp/employees" | python3 -m json.tool

DEV_EMP_ID="${DEV_EMP_ID:-bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb}"
echo "== GET /v1/emp/employees/$DEV_EMP_ID =="
curl -sf "${AUTH[@]}" "$BASE/v1/emp/employees/$DEV_EMP_ID" | python3 -m json.tool

NEW_CODE="MNV-E2E-$(date +%s)"
echo "== POST /v1/emp/employees ($NEW_CODE) =="
curl -sf "${AUTH[@]}" -H "Content-Type: application/json" \
  -d "{\"employeeCode\":\"$NEW_CODE\",\"fullName\":\"E2E NV\",\"emailCty\":\"$NEW_CODE@test.local\"}" \
  "$BASE/v1/emp/employees" | python3 -m json.tool

echo "OK — E2E smoke passed"
