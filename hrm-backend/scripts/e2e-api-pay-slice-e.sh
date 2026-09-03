#!/usr/bin/env bash
# PAY slice E — BH/TNCN from period master rates (FR-006).
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

YM=2028-01
echo "========== seed TIM Closed $YM =========="
PREVIEW=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"$YM\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"pay-e.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":20,\"otUnclassified\":0}]}" \
  "$BASE/v1/tim/imports")
BATCH=$(python3 - <<'PY' "$PREVIEW"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH/commit" >/dev/null
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/$YM/close" >/dev/null

echo "========== PAY-TC-006 — BH/TNCN from master kỳ =========="
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/run" >/dev/null
PERIOD=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM")
python3 - <<'PY' "$PERIOD"
import json, sys, decimal
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
line = data["lines"][0]
bh_rate = decimal.Decimal(str(line["bhRate"]))
tncn_rate = decimal.Decimal(str(line["tncnRate"]))
assert bh_rate == decimal.Decimal("0.10"), line
assert tncn_rate == decimal.Decimal("0.05"), line
salary = decimal.Decimal("10000000")
pc = decimal.Decimal(str(line["contractAllowance"])) + decimal.Decimal(str(line["monthlyAllowance"]))
factor = decimal.Decimal(str(line["timeWageFactor"]))
gross = (salary * factor) + pc
bh = (gross * bh_rate).quantize(decimal.Decimal("0.01"), rounding=decimal.ROUND_HALF_UP)
tncn_base = gross - bh
tncn = (tncn_base * tncn_rate).quantize(decimal.Decimal("0.01"), rounding=decimal.ROUND_HALF_UP)
net = gross - bh - tncn
assert decimal.Decimal(str(line["bhAmount"])) == bh, (line, bh)
assert decimal.Decimal(str(line["tncnAmount"])) == tncn, (line, tncn)
assert decimal.Decimal(str(line["netPay"])) == net, (line, net)
print("master rates OK bh=", bh, "tncn=", tncn, "net=", net)
PY

echo ""
echo "OK — PAY slice E (BH/TNCN master rates)"
