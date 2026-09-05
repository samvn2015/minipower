#!/usr/bin/env bash
# LIF slice D — on checklist Must + provision Email/Git/CRM SP/chat at on (FR-001/002).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}
auth_hdr() { echo "Authorization: Bearer $1"; }

HR_TOKEN=$(token local-dev)
IT_TOKEN=$(token it-dev)

CODE="LIF-D-$(date +%s | tail -c 6)"
EMP=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeCode\":\"$CODE\",\"fullName\":\"LIF D\",\"emailCty\":\"$CODE@test.local\",\"orgUnitCode\":\"ORG-HQ\",\"contract\":{\"contractType\":\"OFFICIAL\",\"startDate\":\"2025-01-01\",\"endDate\":null,\"isProbation\":false}}" \
  "$BASE/v1/emp/employees")
EMP_ID=$(python3 -c 'import json,sys; d=json.load(sys.stdin); d=d.get("data",d); print(d["id"])' <<<"$EMP")

CASE=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeId\":\"$EMP_ID\",\"note\":\"on\"}" \
  "$BASE/v1/lif/onboarding")
CASE_ID=$(python3 -c 'import json,sys; d=json.load(sys.stdin); d=d.get("data",d); print(d["id"])' <<<"$CASE")

echo "========== FR-001 close without Must → 400 =========="
HTTP=$(curl -s -o /tmp/lif-d-close.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/lif/onboarding/$CASE_ID/close")
[[ "$HTTP" == "400" ]] || { cat /tmp/lif-d-close.json; exit 1; }

echo "========== Tick Must =========="
for code in ON-PAPERWORK ON-ORIENTATION; do
  curl -sf -X PUT -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
    -d '{"isChecked":true}' \
    "$BASE/v1/lif/onboarding/$CASE_ID/checklist/$code" >/dev/null
done

echo "========== FR-002 defer Git to N+3 → 400 =========="
HTTP=$(curl -s -o /tmp/lif-d-defer.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$IT_TOKEN")" -H "Content-Type: application/json" \
  -d '{"deferGitToNPlus3":true}' \
  "$BASE/v1/lif/onboarding/$CASE_ID/provisions/Git")
[[ "$HTTP" == "400" ]] || { cat /tmp/lif-d-defer.json; exit 1; }

echo "========== Provision all at on =========="
for sys in EmailCty Git CrmSp Chat; do
  curl -sf -X POST -H "$(auth_hdr "$IT_TOKEN")" -H "Content-Type: application/json" \
    -d '{"deferGitToNPlus3":false}' \
    "$BASE/v1/lif/onboarding/$CASE_ID/provisions/$sys" >/dev/null
done

BOARD=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/lif/onboarding/$CASE_ID/checklist")
python3 - <<'PY' "$BOARD"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["canClose"] is True, d
print("canClose OK")
PY

CLOSED=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/lif/onboarding/$CASE_ID/close")
python3 - <<'PY' "$CLOSED"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["status"] == "Closed"
assert d["allProvisioned"] is True
assert d["gitProvisioned"] is True
print("closed OK")
PY

echo ""
echo "OK — LIF slice D (onboarding)"
