#!/usr/bin/env bash
# TIM slice C — period close (FR-006) + OT unclassified guard (FR-007).
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

echo "========== TIM-TC-007 — OT unclassified blocks close =========="
PREVIEW_BAD=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"2026-11\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"ot-bad.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":22,\"otUnclassified\":2}]}" \
  "$BASE/v1/tim/imports")
BATCH_BAD=$(python3 - <<'PY' "$PREVIEW_BAD"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH_BAD/commit" >/dev/null
HTTP=$(curl -s -o /tmp/tim-close-ot.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/2026-11/close")
echo "close with OT unclassified → HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/tim-close-ot.json; exit 1; }

echo "========== TIM-TC-006 — clean OT classified → close OK =========="
PREVIEW_OK=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"2026-12\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"ot-ok.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":22,\"ot15\":1,\"otUnclassified\":0}]}" \
  "$BASE/v1/tim/imports")
BATCH_OK=$(python3 - <<'PY' "$PREVIEW_OK"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH_OK/commit" >/dev/null
CLOSE=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/2026-12/close")
python3 - <<'PY' "$CLOSE"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "Closed", data
assert data["periodYm"] == "2026-12", data
print("closed:", data)
PY

echo "========== TIM-TC-011 — LM cannot close =========="
HTTP=$(curl -s -o /tmp/tim-lm-close.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/tim/periods/2026-12/close")
echo "LM close → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/tim-lm-close.json; exit 1; }

echo ""
echo "OK — TIM slice C (period close + OT guard)"
