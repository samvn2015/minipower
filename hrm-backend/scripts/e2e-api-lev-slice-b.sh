#!/usr/bin/env bash
# LEV slice B — balance (FR-015) + submit request (FR-001 MVP).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=local-dev" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}

auth_hdr() { echo "Authorization: Bearer $1"; }

NV_TOKEN=$(token)
HANDOVER_EMP="dddddddd-dddd-dddd-dddd-dddddddddddd"
YEAR=2026

echo "========== LEV-TC-015 — my leave balance =========="
BAL=$(curl -sf -H "$(auth_hdr "$NV_TOKEN")" "$BASE/v1/lev/leave-balances/me?year=$YEAR")
echo "$BAL" | python3 -m json.tool
python3 - <<'PY' "$BAL"
import json, sys
body = json.loads(sys.argv[1])
data = body.get("data", body)
assert data["year"] == 2026, data
assert data["remainingDays"] >= 12, data
print("balance ok:", data)
PY

echo "========== LEV catalog — 6 leave types =========="
TYPES=$(curl -sf -H "$(auth_hdr "$NV_TOKEN")" "$BASE/v1/lev/leave-types")
echo "$TYPES" | python3 -m json.tool
python3 - <<'PY' "$TYPES"
import json, sys
rows = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
codes = {r["code"] for r in rows}
assert "LEV-ANNUAL" in codes and len(rows) == 6, codes
print("leave types:", sorted(codes))
PY

echo "========== LEV-TC-001 — submit annual leave =========="
CREATE=$(curl -sf -X POST -H "$(auth_hdr "$NV_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"leaveTypeCode\":\"LEV-ANNUAL\",\"fromDate\":\"2026-10-01\",\"toDate\":\"2026-10-01\",\"dayPart\":\"FullDay\",\"reason\":\"Nghỉ cá nhân\",\"handoverEmployeeId\":\"$HANDOVER_EMP\"}" \
  "$BASE/v1/lev/leave-requests")
echo "$CREATE" | python3 -m json.tool
python3 - <<'PY' "$CREATE"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "PendingC1", data
assert data["totalDays"] == 1, data
print("created:", data)
PY

echo "========== LEV-TC-001n — handover = self blocked =========="
HTTP=$(curl -s -o /tmp/lev-self-ho.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$NV_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"leaveTypeCode\":\"LEV-ANNUAL\",\"fromDate\":\"2026-11-01\",\"toDate\":\"2026-11-01\",\"dayPart\":\"FullDay\",\"reason\":\"Bad\",\"handoverEmployeeId\":\"bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb\"}" \
  "$BASE/v1/lev/leave-requests")
echo "self handover → HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/lev-self-ho.json; exit 1; }

echo ""
echo "OK — LEV slice B (balance + submit MVP)"
