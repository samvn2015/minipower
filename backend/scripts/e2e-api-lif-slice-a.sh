#!/usr/bin/env bash
# LIF slice A — confirm N = last working day; N+3 display; NV 403 (FR-003/004/013/015).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}
auth_hdr() { echo "Authorization: Bearer $1"; }

HR_TOKEN=$(token local-dev)
LM_TOKEN=$(token local-lm)

CODE="LIF-A-$(date +%s | tail -c 6)"
EMP=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeCode\":\"$CODE\",\"fullName\":\"LIF A\",\"emailCty\":\"$CODE@test.local\",\"orgUnitCode\":\"ORG-HQ\",\"contract\":{\"contractType\":\"OFFICIAL\",\"startDate\":\"2025-01-01\",\"endDate\":null,\"isProbation\":false}}" \
  "$BASE/v1/emp/employees")
EMP_ID=$(python3 - <<'PY' "$EMP"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
print(d["id"])
PY
)

echo "========== Create offboarding with resignation signed date =========="
CASE=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeId\":\"$EMP_ID\",\"resignationSignedDate\":\"2026-09-01\",\"note\":\"đơn\"}" \
  "$BASE/v1/lif/offboarding")
CASE_ID=$(python3 - <<'PY' "$CASE"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d.get("jobNPlus3Eligible") in (False, None) or d["jobNPlus3Eligible"] is False
assert not d.get("nPlus3Expected")
print(d["id"])
PY
)

echo "========== FR-003 reject N = signed date =========="
HTTP=$(curl -s -o /tmp/lif-a-bad.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"lastWorkingDayN":"2026-09-01"}' \
  "$BASE/v1/lif/offboarding/$CASE_ID/confirm-n")
echo "signed-as-N → HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/lif-a-bad.json; exit 1; }

echo "========== FR-015 LM/NV confirm 403 =========="
HTTP=$(curl -s -o /tmp/lif-a-lm.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$LM_TOKEN")" -H "Content-Type: application/json" \
  -d '{"lastWorkingDayN":"2026-09-30"}' \
  "$BASE/v1/lif/offboarding/$CASE_ID/confirm-n")
echo "LM confirm → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/lif-a-lm.json; exit 1; }

echo "========== FR-003/013 HR confirm N + N+3 =========="
OK=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"lastWorkingDayN":"2026-09-30"}' \
  "$BASE/v1/lif/offboarding/$CASE_ID/confirm-n")
python3 - <<'PY' "$OK"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["status"] == "ConfirmedN"
assert d["lastWorkingDayN"].startswith("2026-09-30")
assert d["nPlus3Expected"].startswith("2026-10-03"), d
assert d["jobNPlus3Eligible"] is True
assert d["confirmedByIdpSubject"]
print("confirm N OK", d["nPlus3Expected"])
PY

echo ""
echo "OK — LIF slice A (confirm N)"
