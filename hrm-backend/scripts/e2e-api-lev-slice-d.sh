#!/usr/bin/env bash
# LEV slice D — C2 approve + atomic balance deduct (FR-012 / TC-012).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}

auth_hdr() { echo "Authorization: Bearer $1"; }

NV_TOKEN=$(token local-dev)
LM_TOKEN=$(token local-lm)
HR_TOKEN=$(token local-dev)
HANDOVER_EMP="dddddddd-dddd-dddd-dddd-dddddddddddd"
YEAR=2026

echo "========== balance before flow =========="
BAL0=$(curl -sf -H "$(auth_hdr "$NV_TOKEN")" "$BASE/v1/lev/leave-balances/me?year=$YEAR")
REMAIN0=$(python3 - <<'PY' "$BAL0"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["remainingDays"])
PY
)
echo "remaining: $REMAIN0"

echo "========== submit → C1 → C2 happy path =========="
CREATE=$(curl -sf -X POST -H "$(auth_hdr "$NV_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"leaveTypeCode\":\"LEV-ANNUAL\",\"fromDate\":\"2026-12-15\",\"toDate\":\"2026-12-15\",\"dayPart\":\"FullDay\",\"reason\":\"Slice D C2 test\",\"handoverEmployeeId\":\"$HANDOVER_EMP\"}" \
  "$BASE/v1/lev/leave-requests")
REQ_ID=$(python3 - <<'PY' "$CREATE"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/lev/leave-requests/$REQ_ID/c1/approve" >/dev/null

echo "========== LEV-TC-017 — LM cannot C2 =========="
HTTP=$(curl -s -o /tmp/lev-lm-c2.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/lev/leave-requests/$REQ_ID/c2/approve")
echo "LM C2 → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/lev-lm-c2.json; exit 1; }

echo "========== LEV-TC-012 — HR C2 approve + deduct balance =========="
APPROVE=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" \
  "$BASE/v1/lev/leave-requests/$REQ_ID/c2/approve")
python3 - <<'PY' "$APPROVE"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "Approved", data
print("c2 approved:", data)
PY

BAL1=$(curl -sf -H "$(auth_hdr "$NV_TOKEN")" "$BASE/v1/lev/leave-balances/me?year=$YEAR")
REMAIN1=$(python3 - <<'PY' "$BAL1"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["remainingDays"])
PY
)
echo "remaining after C2: $REMAIN1 (expect $((REMAIN0 - 1)))"
python3 - <<PY
assert float("$REMAIN1") == float("$REMAIN0") - 1, ("$REMAIN1", "$REMAIN0")
PY

echo ""
echo "OK — LEV slice D (C2 approve + atomic balance deduct)"
