#!/usr/bin/env bash
# PRB slice C — propose/decide RBAC + master outcomes (FR-004/009/012/006 gate).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}
auth_hdr() { echo "Authorization: Bearer $1"; }

HR_TOKEN=$(token local-dev)
LM_TOKEN=$(token local-lm)

CODE="PRB-C-$(date +%s | tail -c 6)"
EMAIL="${CODE}@test.local"

echo "========== Masters =========="
OUT=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/prb/masters/outcomes")
python3 - <<'PY' "$OUT"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
codes = {x["code"] for x in data}
assert codes == {"PASS", "EXTEND", "FAIL"}, codes
print("outcomes OK")
PY

echo "========== Create TV =========="
EMP=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeCode\":\"$CODE\",\"fullName\":\"PRB C\",\"emailCty\":\"$EMAIL\",\"orgUnitCode\":\"ORG-HQ\",\"contract\":{\"contractType\":\"PROBATION\",\"startDate\":\"2026-01-01\",\"endDate\":\"2026-06-30\",\"isProbation\":true}}" \
  "$BASE/v1/emp/employees")
EMP_ID=$(python3 - <<'PY' "$EMP"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
print(d.get("id") or d.get("employeeId"))
PY
)
echo "emp id=$EMP_ID"

echo "========== FR-004 invalid outcome =========="
HTTP=$(curl -s -o /tmp/prb-c-bad.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"outcomeCode":"CONDITIONAL","note":"x"}' \
  "$BASE/v1/prb/evaluations/$EMP_ID/decide")
echo "invalid → HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/prb-c-bad.json; exit 1; }

echo "========== FR-009 LM decide 403 =========="
HTTP=$(curl -s -o /tmp/prb-c-lm.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$LM_TOKEN")" -H "Content-Type: application/json" \
  -d '{"outcomeCode":"PASS"}' \
  "$BASE/v1/prb/evaluations/$EMP_ID/decide")
echo "LM decide → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/prb-c-lm.json; exit 1; }

echo "========== FR-009/017 HR decide PASS =========="
DEC=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"outcomeCode":"PASS","note":"chốt đạt","scores":[{"criterionCode":"CRIT-WORK","comment":"ok"}]}' \
  "$BASE/v1/prb/evaluations/$EMP_ID/decide")
python3 - <<'PY' "$DEC"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["status"] == "Decided"
assert d["decidedOutcomeCode"] == "PASS"
assert d["decidedByIdpSubject"]
assert d["decidedAtUtc"]
print("decide OK", d["decidedByIdpSubject"])
PY

echo ""
echo "OK — PRB slice C (decide RBAC)"
