#!/usr/bin/env bash
# PAY slice H — re-run Draft (FR-016) + hide HR screens from NV/LM (FR-017).
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
    -d "{\"periodYm\":\"$ym\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"pay-h.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":20,\"otUnclassified\":0}]}" \
    "$BASE/v1/tim/imports")
  BATCH=$(python3 - <<'PY' "$PREVIEW"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH/commit" >/dev/null
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/$ym/close" >/dev/null
}

YM=2029-02
echo "========== Seed TIM Closed $YM =========="
seed_tim_closed "$YM"

echo "========== PAY-TC-016 H — overwrite Draft =========="
RUN1=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/run")
ID1=$(python3 - <<'PY' "$RUN1"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "Draft", data
print(data["periodId"])
PY
)
RUN2=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/run")
python3 - <<'PY' "$RUN2" "$ID1"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "Draft", data
print("overwrite Draft OK", data["periodId"])
PY

echo "========== PAY-TC-016 N — Closed run 409 =========="
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/close" >/dev/null
HTTP=$(curl -s -o /tmp/pay-h-closed.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/run")
echo "Closed re-run → HTTP $HTTP (expect 409)"
[[ "$HTTP" == "409" ]] || { cat /tmp/pay-h-closed.json; exit 1; }
grep -q "PAY-FR-016" /tmp/pay-h-closed.json || { cat /tmp/pay-h-closed.json; exit 1; }

echo "========== PAY-TC-017 — LM list/get periods 403 =========="
HTTP=$(curl -s -o /tmp/pay-h-lm-list.json -w "%{http_code}" \
  -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/pay/periods")
echo "LM list → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/pay-h-lm-list.json; exit 1; }

HTTP=$(curl -s -o /tmp/pay-h-lm-get.json -w "%{http_code}" \
  -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/pay/periods/$YM")
echo "LM get period → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/pay-h-lm-get.json; exit 1; }

echo "========== NV still payslips me =========="
curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/payslips/me" >/dev/null
echo "owner payslips OK"

echo ""
echo "OK — PAY slice H (FR-016 + FR-017)"
