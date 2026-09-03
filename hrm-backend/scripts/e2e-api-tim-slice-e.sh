#!/usr/bin/env bash
# TIM slice E — unlock period (FR-012); block when PAY closed.
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}
auth_hdr() { echo "Authorization: Bearer $1"; }

HR_TOKEN=$(token local-dev)
LM_TOKEN=$(token local-lm)

ACTIVE=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/templates/active")
VERSION=$(python3 - <<'PY' "$ACTIVE"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["versionCode"])
PY
)

seed_closed() {
  local ym=$1
  PREVIEW=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
    -d "{\"periodYm\":\"$ym\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"e.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":20}]}" \
    "$BASE/v1/tim/imports")
  BATCH=$(python3 - <<'PY' "$PREVIEW"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH/commit" >/dev/null
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/$ym/close" >/dev/null
}

echo "========== TIM-TC-012 H — unlock when PAY not closed =========="
seed_closed "2027-05"
UNLOCK=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/2027-05/unlock")
python3 - <<'PY' "$UNLOCK"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "Draft", data
print("unlocked:", data)
PY

echo "========== TIM-TC-011 — LM cannot unlock =========="
# re-close for LM test
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/2027-05/close" >/dev/null
HTTP=$(curl -s -o /tmp/tim-lm-unlock.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/tim/periods/2027-05/unlock")
echo "LM unlock → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/tim-lm-unlock.json; exit 1; }

echo "========== TIM-TC-012 N — PAY closed blocks TIM unlock =========="
seed_closed "2027-06"
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/2027-06/close" >/dev/null
HTTP=$(curl -s -o /tmp/tim-pay-block.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/2027-06/unlock")
echo "unlock with PAY closed → HTTP $HTTP (expect 409)"
[[ "$HTTP" == "409" ]] || { cat /tmp/tim-pay-block.json; exit 1; }

echo ""
echo "OK — TIM slice E (period unlock + PAY closed guard)"
