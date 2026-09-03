#!/usr/bin/env bash
# PAY slice C — block close when N_tính > standard workdays (FR-007).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}
auth_hdr() { echo "Authorization: Bearer $1"; }

HR_TOKEN=$(token local-dev)

ACTIVE=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/templates/active")
VERSION=$(python3 - <<'PY' "$ACTIVE"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["versionCode"])
PY
)

seed_tim_closed() {
  local ym=$1 days=$2
  PREVIEW=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
    -d "{\"periodYm\":\"$ym\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"c.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":$days,\"otUnclassified\":0}]}" \
    "$BASE/v1/tim/imports")
  BATCH=$(python3 - <<'PY' "$PREVIEW"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH/commit" >/dev/null
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/$ym/close" >/dev/null
}

echo "========== PAY-TC-007 N — N_tính > chuẩn blocks close =========="
seed_tim_closed "2027-10" 22
curl -sf -X PUT -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"standardWorkDays":21}' "$BASE/v1/pay/calendar/2027-10" >/dev/null
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/2027-10/run" >/dev/null
PERIOD=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/2027-10")
python3 - <<'PY' "$PERIOD"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["hasNTinhOverCap"] is True, data
print("preview warning OK:", data["overCapEmployeeCodes"], "std=", data["standardWorkDays"])
PY
HTTP=$(curl -s -o /tmp/pay-cap.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/2027-10/close")
echo "close over cap → HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/pay-cap.json; exit 1; }

echo "========== PAY-TC-007 H — N_tính ≤ chuẩn closes =========="
seed_tim_closed "2027-11" 20
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/2027-11/run" >/dev/null
CLOSE=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/2027-11/close")
python3 - <<'PY' "$CLOSE"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "Closed", data
print("closed OK:", data)
PY

echo ""
echo "OK — PAY slice C (workday cap on close)"
