#!/usr/bin/env bash
# TIM slice B — import preview (FR-003/004) + commit Draft (FR-005).
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
echo "active version: $VERSION"

echo "========== TIM-TC-003 — wrong version rejected =========="
HTTP=$(curl -s -o /tmp/tim-bad-ver.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"2026-09\",\"templateVersionCode\":\"NOT-ACTIVE\",\"fileName\":\"bad.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":22}]}" \
  "$BASE/v1/tim/imports")
echo "wrong version → HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/tim-bad-ver.json; exit 1; }

echo "========== TIM-TC-004 — preview with missing NV (Must error) =========="
PREVIEW_ERR=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"2026-09\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"err.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"NOPE\",\"workDays\":22}]}" \
  "$BASE/v1/tim/imports")
BATCH_ERR=$(python3 - <<'PY' "$PREVIEW_ERR"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["hasMustErrors"] is True, data
print(data["id"])
PY
)
HTTP=$(curl -s -o /tmp/tim-commit-block.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH_ERR/commit")
echo "commit with Must errors → HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/tim-commit-block.json; exit 1; }

echo "========== TIM-TC-011 — LM cannot import =========="
HTTP=$(curl -s -o /tmp/tim-lm-imp.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$LM_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"2026-09\",\"templateVersionCode\":\"$VERSION\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":22}]}" \
  "$BASE/v1/tim/imports")
echo "LM import → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/tim-lm-imp.json; exit 1; }

echo "========== TIM-TC-005 — clean preview + commit Draft =========="
PREVIEW_OK=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"2026-10\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"ok.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":22,\"ot15\":1}]}" \
  "$BASE/v1/tim/imports")
BATCH_OK=$(python3 - <<'PY' "$PREVIEW_OK"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["hasMustErrors"] is False, data
print(data["id"])
PY
)
COMMIT=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH_OK/commit")
python3 - <<'PY' "$COMMIT"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "Draft", data
assert data["lineCount"] >= 1, data
print("committed period:", data)
PY

echo ""
echo "OK — TIM slice B (import preview + commit)"
