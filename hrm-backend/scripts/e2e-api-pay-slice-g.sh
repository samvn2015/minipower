#!/usr/bin/env bash
# PAY slice G — immutable workdays (FR-008) + bulk export (FR-012).
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
    -d "{\"periodYm\":\"$ym\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"pay-g.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":20,\"otUnclassified\":0}]}" \
    "$BASE/v1/tim/imports")
  BATCH=$(python3 - <<'PY' "$PREVIEW"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH/commit" >/dev/null
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/$ym/close" >/dev/null
}

YM=2028-04
echo "========== Closed period $YM =========="
seed_tim_closed "$YM"
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/run" >/dev/null
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/close" >/dev/null

PERIOD=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM")
LINE_ID=$(python3 - <<'PY' "$PERIOD"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
print(data["lines"][0]["id"])
PY
)

echo "========== PAY-TC-008 — reject edit 400 =========="
HTTP=$(curl -s -o /tmp/pay-g-edit.json -w "%{http_code}" -X PUT \
  -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{}' "$BASE/v1/pay/periods/$YM/lines/$LINE_ID")
echo "HR edit → HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/pay-g-edit.json; exit 1; }
grep -q "PAY-FR-008" /tmp/pay-g-edit.json || { cat /tmp/pay-g-edit.json; exit 1; }

echo "========== PAY-TC-009 — LM export 403 =========="
HTTP=$(curl -s -o /tmp/pay-g-lm.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$LM_TOKEN")" -H "Content-Type: application/json" \
  -d '{"includePdf":true,"includeEmail":false}' \
  "$BASE/v1/pay/periods/$YM/export")
echo "LM export → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/pay-g-lm.json; exit 1; }

echo "========== PAY-TC-012 N — CC forbidden =========="
HTTP=$(curl -s -o /tmp/pay-g-cc.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"includePdf":true,"includeEmail":true,"ccAddresses":["lm@company.local"]}' \
  "$BASE/v1/pay/periods/$YM/export")
echo "CC → HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/pay-g-cc.json; exit 1; }

echo "========== PAY-TC-012 H — PDF + email =========="
EXPORT=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"includePdf":true,"includeEmail":true}' \
  "$BASE/v1/pay/periods/$YM/export")
python3 - <<'PY' "$EXPORT"
import json, sys, base64
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["pdfCount"] == 1, data
assert data["emailCount"] == 1, data
item = data["items"][0]
assert item["employeeCode"] == "MNV-DEV"
assert item["toAddress"] == "dev@company.local"
assert item["pdfFileName"]
raw = base64.b64decode(item["pdfBase64"])
assert raw.startswith(b"%PDF"), raw[:20]
print("export OK", item["pdfFileName"], "to", item["toAddress"])
PY

echo ""
echo "OK — PAY slice G (FR-008 + FR-012)"
