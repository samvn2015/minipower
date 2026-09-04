#!/usr/bin/env bash
# TIM slice F — no device punch · publish does not commit import · reject zkteco fileName.
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

echo "========== Preview import then publish template — batch stays Preview =========="
YM="2027-11"
PREVIEW=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"$YM\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"ok.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":20}]}" \
  "$BASE/v1/tim/imports")
BATCH=$(python3 - <<'PY' "$PREVIEW"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["status"] == "Preview", d
print(d["id"])
PY
)

SUFFIX=$(date +%s | tail -c 6)
CREATE=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"versionCode\":\"TIM-F-$SUFFIX\",\"name\":\"Slice F\",\"columns\":[{\"columnKey\":\"mnv\",\"displayName\":\"Mã NV\",\"sortOrder\":1,\"isRequired\":true,\"mapsTo\":\"EmployeeCode\"},{\"columnKey\":\"n_thuc\",\"displayName\":\"Ngày công\",\"sortOrder\":2,\"isRequired\":true,\"mapsTo\":\"WorkDays\"}]}" \
  "$BASE/v1/tim/templates")
DRAFT_ID=$(python3 - <<'PY' "$CREATE"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/templates/$DRAFT_ID/publish" >/dev/null

BATCH_GET=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH")
python3 - <<'PY' "$BATCH_GET"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["status"] == "Preview", d
print("batch still Preview OK")
PY

echo "========== TIM-TC-010 — POST device punch → 405 =========="
HTTP=$(curl -s -o /tmp/tim-device.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{}' "$BASE/v1/tim/devices/punch")
echo "POST /v1/tim/devices/punch → HTTP $HTTP (expect 405)"
[[ "$HTTP" == "405" ]] || { cat /tmp/tim-device.json; exit 1; }

echo "========== Preview fileName zkteco.bin → 400 =========="
HTTP2=$(curl -s -o /tmp/tim-zk.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"$YM\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"zkteco.bin\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":20}]}" \
  "$BASE/v1/tim/imports")
echo "zkteco.bin preview → HTTP $HTTP2 (expect 400)"
[[ "$HTTP2" == "400" ]] || { cat /tmp/tim-zk.json; exit 1; }
grep -qi "TIM-TC-010\|TIM-FR-010\|máy CC\|zkteco\|cấm" /tmp/tim-zk.json || { cat /tmp/tim-zk.json; exit 1; }

echo ""
echo "OK — TIM slice F (no device · Preview intact · reject zkteco)"
