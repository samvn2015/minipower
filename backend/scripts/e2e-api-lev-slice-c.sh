#!/usr/bin/env bash
# LEV slice C — C1 approve/reject (FR-010) + balance unchanged after C1 (TC-018).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}

auth_hdr() { echo "Authorization: Bearer $1"; }

NV_TOKEN=$(token local-dev)
LM_TOKEN=$(token local-lm)
HANDOVER_EMP="dddddddd-dddd-dddd-dddd-dddddddddddd"
YEAR=2026

echo "========== LEV-TC-015 — balance before C1 =========="
BAL_BEFORE=$(curl -sf -H "$(auth_hdr "$NV_TOKEN")" "$BASE/v1/lev/leave-balances/me?year=$YEAR")
REMAIN_BEFORE=$(python3 - <<'PY' "$BAL_BEFORE"
import json, sys
body = json.loads(sys.argv[1])
data = body.get("data", body)
print(data["remainingDays"])
PY
)
echo "remaining before: $REMAIN_BEFORE"

echo "========== submit leave for C1 flow =========="
CREATE=$(curl -sf -X POST -H "$(auth_hdr "$NV_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"leaveTypeCode\":\"LEV-ANNUAL\",\"fromDate\":\"2026-12-01\",\"toDate\":\"2026-12-01\",\"dayPart\":\"FullDay\",\"reason\":\"Slice C test\",\"handoverEmployeeId\":\"$HANDOVER_EMP\"}" \
  "$BASE/v1/lev/leave-requests")
REQ_ID=$(python3 - <<'PY' "$CREATE"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "PendingC1", data
print(data["id"])
PY
)
echo "request id: $REQ_ID"

echo "========== LEV-TC-010 — NV cannot self C1 =========="
HTTP=$(curl -s -o /tmp/lev-self-c1.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$NV_TOKEN")" "$BASE/v1/lev/leave-requests/$REQ_ID/c1/approve")
echo "NV self C1 → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/lev-self-c1.json; exit 1; }

echo "========== LM pending C1 inbox =========="
PENDING=$(curl -sf -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/lev/leave-requests/pending-c1")
python3 - <<'PY' "$PENDING" "$REQ_ID"
import json, sys
rows = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
req_id = sys.argv[2]
ids = {r["id"] for r in rows}
assert req_id in ids, (req_id, ids)
print("pending c1 count:", len(rows))
PY

echo "========== LEV-TC-011 path — LM C1 approve → PendingC2 =========="
APPROVE=$(curl -sf -X POST -H "$(auth_hdr "$LM_TOKEN")" \
  "$BASE/v1/lev/leave-requests/$REQ_ID/c1/approve")
python3 - <<'PY' "$APPROVE"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "PendingC2", data
print("approved:", data)
PY

echo "========== LEV-TC-018 — balance unchanged after C1 =========="
BAL_AFTER=$(curl -sf -H "$(auth_hdr "$NV_TOKEN")" "$BASE/v1/lev/leave-balances/me?year=$YEAR")
REMAIN_AFTER=$(python3 - <<'PY' "$BAL_AFTER"
import json, sys
body = json.loads(sys.argv[1])
data = body.get("data", body)
print(data["remainingDays"])
PY
)
echo "remaining after C1: $REMAIN_AFTER (expect $REMAIN_BEFORE)"
[[ "$REMAIN_AFTER" == "$REMAIN_BEFORE" ]] || exit 1

echo ""
echo "OK — LEV slice C (C1 approve + web API)"
