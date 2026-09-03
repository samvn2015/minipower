#!/usr/bin/env bash
# PAY slice A — run Draft from TIM Closed; N_tính = N_thực − N_KHL (FR-001/002).
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

echo "========== PAY-TC-001 N — TIM not closed blocks run =========="
HTTP=$(curl -s -o /tmp/pay-tim-block.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/2027-08/run")
echo "run without TIM → HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/pay-tim-block.json; exit 1; }

echo "========== seed TIM Closed 2027-08 =========="
PREVIEW=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"2027-08\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"pay.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":20,\"ot15\":1,\"otUnclassified\":0}]}" \
  "$BASE/v1/tim/imports")
BATCH=$(python3 - <<'PY' "$PREVIEW"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH/commit" >/dev/null
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/2027-08/close" >/dev/null

echo "========== PAY-TC-001 H — run Draft =========="
RUN=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/2027-08/run")
python3 - <<'PY' "$RUN"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "Draft", data
assert data["lineCount"] >= 1, data
print("run:", data)
PY

PERIOD=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/2027-08")
python3 - <<'PY' "$PERIOD"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
line = data["lines"][0]
# N_tính = N_thực − N_KHL (no paid leave added again)
assert float(line["nTinh"]) == float(line["workDays"]) - float(line["leaveDaysUnpaid"]), line
print("N_tính OK:", line)
PY

echo "========== LM cannot run =========="
HTTP=$(curl -s -o /tmp/pay-lm.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/pay/periods/2027-08/run")
echo "LM run → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/pay-lm.json; exit 1; }

echo ""
echo "OK — PAY slice A (run Draft + N_tính)"
