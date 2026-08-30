#!/usr/bin/env bash
# LEV slice E — overlap block (FR-003) + NV cancel before C2 (FR-013).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}

auth_hdr() { echo "Authorization: Bearer $1"; }

NV_TOKEN=$(token local-dev)
HANDOVER_EMP="dddddddd-dddd-dddd-dddd-dddddddddddd"

payload() {
  python3 - <<PY
import json
print(json.dumps({
  "leaveTypeCode": "LEV-ANNUAL",
  "fromDate": "$1",
  "toDate": "$1",
  "dayPart": "FullDay",
  "reason": "$2",
  "handoverEmployeeId": "$HANDOVER_EMP"
}))
PY
}

echo "========== LEV-TC-013 — submit for cancel test =========="
CREATE=$(curl -sf -X POST -H "$(auth_hdr "$NV_TOKEN")" -H "Content-Type: application/json" \
  -d "$(payload 2026-11-20 "Cancel me")" "$BASE/v1/lev/leave-requests")
REQ_ID=$(python3 - <<'PY' "$CREATE"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
echo "request: $REQ_ID"

echo "========== LEV-TC-003 — overlap blocked =========="
HTTP=$(curl -s -o /tmp/lev-overlap.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$NV_TOKEN")" -H "Content-Type: application/json" \
  -d "$(payload 2026-11-20 "Overlap")" "$BASE/v1/lev/leave-requests")
echo "overlap submit → HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/lev-overlap.json; exit 1; }

echo "========== LEV-TC-013 — NV cancel before C2 =========="
CANCEL=$(curl -sf -X POST -H "$(auth_hdr "$NV_TOKEN")" \
  "$BASE/v1/lev/leave-requests/$REQ_ID/cancel")
python3 - <<'PY' "$CANCEL"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "Cancelled", data
print("cancelled:", data)
PY

echo "========== LEV-TC-013 — resubmit same day after cancel =========="
RESUB=$(curl -sf -X POST -H "$(auth_hdr "$NV_TOKEN")" -H "Content-Type: application/json" \
  -d "$(payload 2026-11-20 "Resubmit OK")" "$BASE/v1/lev/leave-requests")
python3 - <<'PY' "$RESUB"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "PendingC1", data
print("resubmitted:", data)
PY

echo ""
echo "OK — LEV slice E (overlap + cancel)"
