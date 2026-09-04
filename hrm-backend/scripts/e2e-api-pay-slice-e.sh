#!/usr/bin/env bash
# PAY slice E — BH/TNCN from C&B master (FR-006/018).
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
  -d "{\"periodYm\":\"$YM\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"pay-e.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":26,\"otUnclassified\":0}]}" \
  "$BASE/v1/tim/imports")
BATCH=$(python3 - <<'PY' "$PREVIEW"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH/commit" >/dev/null
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/$YM/close" >/dev/null

curl -sf -X PUT -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"standardWorkDays":26}' "$BASE/v1/pay/calendar/$YM" >/dev/null

echo "========== PAY-TC-006 — BH/TNCN C&B master =========="
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/run" >/dev/null
PERIOD=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM")
python3 - <<'PY' "$PERIOD"
import json, sys, decimal
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
line = data["lines"][0]
# MNV-DEV seed: E=10M, PC meal contract 730k; F=26, G=26
# H=10M; L=10_730_000; P=1_050_000; T=max(0,L-J-P-Q-S)=0 → U=0; W=9_680_000
bh_rate = decimal.Decimal(str(line["bhRate"]))
assert bh_rate == decimal.Decimal("0.105"), line
assert decimal.Decimal(str(line["bhAmount"])) == decimal.Decimal("1050000"), line
assert decimal.Decimal(str(line["tncnAmount"])) == decimal.Decimal("0"), line
assert decimal.Decimal(str(line["netPay"])) == decimal.Decimal("9680000"), line
print("C&B master OK net=", line["netPay"])
PY

echo ""
echo "OK — PAY slice E (BH/TNCN C&B master rates)"
