#!/usr/bin/env bash
# EMP slice A — education (FR-017), seniority (FR-010), audit (NFR-002).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=local-dev" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}

auth_hdr() { echo "Authorization: Bearer $1"; }

HR_TOKEN=$(token)
TS=$(date +%s)
DEV_EMP="bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"

echo "========== EMP-TC-017 — education catalog + save =========="
curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/emp/education-levels" | python3 -m json.tool

HTTP=$(curl -s -o /tmp/edu-bad.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeCode\":\"MNV-EDU-BAD-$TS\",\"fullName\":\"Bad Edu\",\"orgUnitCode\":\"ORG-HQ\",\"educationLevelCode\":\"EDU-INACTIVE\"}" \
  "$BASE/v1/emp/employees")
echo "POST inactive education → HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/edu-bad.json; exit 1; }

CODE="MNV-EDU-$TS"
CREATE_JSON=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeCode\":\"$CODE\",\"fullName\":\"Edu NV\",\"emailCty\":\"$CODE@test.local\",\"orgUnitCode\":\"ORG-HQ\",\"educationLevelCode\":\"EDU-DH\",\"contract\":{\"contractType\":\"PROBATION\",\"startDate\":\"2020-03-01\",\"endDate\":null,\"isProbation\":false}}" \
  "$BASE/v1/emp/employees")
echo "$CREATE_JSON" | python3 -m json.tool
NEW_ID=$(python3 -c 'import json,sys; print(json.loads(sys.argv[1])["id"])' "$CREATE_JSON")

GET_JSON=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/emp/employees/$NEW_ID")
echo "$GET_JSON" | python3 -m json.tool | rg -i "education|EDU-DH"

echo "========== EMP-TC-010 — seniority from master =========="
python3 - <<'PY' "$GET_JSON"
import json, sys
body = json.loads(sys.argv[1])
data = body.get("data", body)
sen = data.get("seniority")
assert sen and sen.get("ruleCode"), "missing seniority.ruleCode from master"
assert sen.get("years", 0) >= 5, f"expected years>=5, got {sen}"
print("seniority:", sen)
PY

echo "========== EMP-TC-NFR-002 — audit log after create/update/LM =========="
AUDIT=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/emp/employees/$NEW_ID/audit-logs")
echo "$AUDIT" | python3 -m json.tool
python3 - <<'PY' "$AUDIT"
import json, sys
rows = json.loads(sys.argv[1])
rows = rows.get("data", rows)
actions = {r["action"] for r in rows}
assert "EmployeeCreated" in actions, actions
print("audit actions:", sorted(actions))
PY

curl -sf -X PATCH -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"educationLevelCode":"EDU-THPT"}' \
  "$BASE/v1/emp/employees/$NEW_ID" > /dev/null

AUDIT2=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/emp/employees/$NEW_ID/audit-logs")
python3 - <<'PY' "$AUDIT2"
import json, sys
rows = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
actions = {r["action"] for r in rows}
assert "EmployeeUpdated" in actions, actions
print("audit after patch:", sorted(actions))
PY

echo ""
echo "OK — EMP slice A (education, seniority, audit)"
