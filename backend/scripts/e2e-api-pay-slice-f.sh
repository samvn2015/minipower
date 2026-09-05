#!/usr/bin/env bash
# PAY slice F — payslip isolation (FR-010).
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

seed_tim_closed() {
  local ym=$1
  PREVIEW=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
    -d "{\"periodYm\":\"$ym\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"pay-f.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":20,\"otUnclassified\":0}]}" \
    "$BASE/v1/tim/imports")
  BATCH=$(python3 - <<'PY' "$PREVIEW"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH/commit" >/dev/null
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/$ym/close" >/dev/null
}

YM=2028-02
echo "========== Closed period $YM =========="
seed_tim_closed "$YM"
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/run" >/dev/null
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/close" >/dev/null

ME=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/payslips/me")
SLIP_ID=$(python3 - <<'PY' "$ME" "$YM"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
ym = sys.argv[2]
row = next(x for x in data if x["periodYm"] == ym)
print(row["id"])
PY
)
echo "payslip id=$SLIP_ID"

echo "========== PAY-TC-010 H — owner/HR 200 =========="
curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/payslips/$SLIP_ID" >/dev/null
echo "owner/HR OK"

echo "========== PAY-TC-010 N — LM subordinate 403 =========="
HTTP=$(curl -s -o /tmp/pay-slip-lm.json -w "%{http_code}" \
  -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/pay/payslips/$SLIP_ID")
echo "LM → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/pay-slip-lm.json; exit 1; }

echo "========== Draft not visible as payslip =========="
YM2=2028-03
seed_tim_closed "$YM2"
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM2/run" >/dev/null
PERIOD=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM2")
DRAFT_ID=$(python3 - <<'PY' "$PERIOD"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
print(data["lines"][0]["id"])
assert data["status"] == "Draft", data
PY
)
HTTP=$(curl -s -o /tmp/pay-slip-draft.json -w "%{http_code}" \
  -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/payslips/$DRAFT_ID")
echo "Draft payslip → HTTP $HTTP (expect 404)"
[[ "$HTTP" == "404" ]] || { cat /tmp/pay-slip-draft.json; exit 1; }

echo ""
echo "OK — PAY slice F (payslip isolation)"
