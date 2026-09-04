#!/usr/bin/env bash
# LIF slice B — off checklist master; cannot close missing Must (FR-009).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}
auth_hdr() { echo "Authorization: Bearer $1"; }

HR_TOKEN=$(token local-dev)
CODE="LIF-B-$(date +%s | tail -c 6)"
EMP=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeCode\":\"$CODE\",\"fullName\":\"LIF B\",\"emailCty\":\"$CODE@test.local\",\"orgUnitCode\":\"ORG-HQ\",\"contract\":{\"contractType\":\"OFFICIAL\",\"startDate\":\"2025-01-01\",\"endDate\":null,\"isProbation\":false}}" \
  "$BASE/v1/emp/employees")
EMP_ID=$(python3 -c 'import json,sys; d=json.load(sys.stdin); d=d.get("data",d); print(d["id"])' <<<"$EMP")

CASE=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeId\":\"$EMP_ID\",\"resignationSignedDate\":\"2026-08-01\"}" \
  "$BASE/v1/lif/offboarding")
CASE_ID=$(python3 -c 'import json,sys; d=json.load(sys.stdin); d=d.get("data",d); print(d["id"])' <<<"$CASE")

echo "========== Checklist master on board =========="
BOARD=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/lif/offboarding/$CASE_ID/checklist")
python3 - <<'PY' "$BOARD"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
codes = {i["code"] for i in d["items"]}
assert "OFF-RETURN-LAPTOP" in codes and "OFF-RETURN-BADGE" in codes
assert any(i["isMust"] for i in d["items"])
assert d["canClose"] is False
print("board OK", len(d["items"]))
PY

echo "========== FR-009 close without Must → 400 =========="
HTTP=$(curl -s -o /tmp/lif-b-close.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/lif/offboarding/$CASE_ID/close")
echo "close incomplete → HTTP $HTTP"
[[ "$HTTP" == "400" ]] || { cat /tmp/lif-b-close.json; exit 1; }

echo "========== Tick all Must =========="
for code in OFF-RETURN-LAPTOP OFF-RETURN-BADGE OFF-HANDOVER; do
  curl -sf -X PUT -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
    -d '{"isChecked":true}' \
    "$BASE/v1/lif/offboarding/$CASE_ID/checklist/$code" >/dev/null
done
BOARD2=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/lif/offboarding/$CASE_ID/checklist")
python3 - <<'PY' "$BOARD2"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["canClose"] is True, d
print("canClose OK")
PY

echo "========== Close OK =========="
CLOSED=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/lif/offboarding/$CASE_ID/close")
python3 - <<'PY' "$CLOSED"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["status"] == "Closed"
print("closed OK")
PY

echo ""
echo "OK — LIF slice B (off checklist)"
