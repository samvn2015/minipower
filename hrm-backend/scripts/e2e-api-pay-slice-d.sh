#!/usr/bin/env bash
# PAY slice D — PC HĐ + tháng, mã ∈ master (FR-005/015).
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

YM=2027-12
echo "========== seed TIM Closed $YM =========="
PREVIEW=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"$YM\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"pay-d.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":20,\"otUnclassified\":0}]}" \
  "$BASE/v1/tim/imports")
BATCH=$(python3 - <<'PY' "$PREVIEW"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH/commit" >/dev/null
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/$YM/close" >/dev/null

echo "========== PAY-TC-005 N — unknown code blocked =========="
HTTP=$(curl -s -o /tmp/pay-pc-bad.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"$YM\",\"employeeCode\":\"MNV-DEV\",\"code\":\"PC-LA\",\"amount\":1000}" \
  "$BASE/v1/pay/monthly-allowances")
echo "unknown code → HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/pay-pc-bad.json; exit 1; }

echo "========== PAY-FR-009 — LM 403 =========="
HTTP=$(curl -s -o /tmp/pay-pc-lm.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$LM_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"$YM\",\"employeeCode\":\"MNV-DEV\",\"code\":\"PC-XANG\",\"amount\":1}" \
  "$BASE/v1/pay/monthly-allowances")
echo "LM upsert → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/pay-pc-lm.json; exit 1; }

echo "========== PAY-TC-005 H — HĐ + tháng on preview =========="
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"$YM\",\"employeeCode\":\"MNV-DEV\",\"code\":\"PC-XANG\",\"amount\":200000}" \
  "$BASE/v1/pay/monthly-allowances" >/dev/null
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/run" >/dev/null
PERIOD=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM")
python3 - <<'PY' "$PERIOD"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
line = data["lines"][0]
assert float(line["contractAllowance"]) == 730000, line
assert float(line["monthlyAllowance"]) == 200000, line
print("two channels OK:", line["contractAllowance"], line["monthlyAllowance"])
PY

echo ""
echo "OK — PAY slice D (allowance two channels)"
