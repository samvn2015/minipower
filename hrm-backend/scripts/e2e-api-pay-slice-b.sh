#!/usr/bin/env bash
# PAY slice B — probation time-wage factor from master (FR-003) + OT from TIM (FR-004).
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

YM=2027-09
echo "========== seed TIM Closed $YM =========="
PREVIEW=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"$YM\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"pay-b.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":22,\"ot15\":2,\"ot20\":1,\"otUnclassified\":0}]}" \
  "$BASE/v1/tim/imports")
BATCH=$(python3 - <<'PY' "$PREVIEW"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH/commit" >/dev/null
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/$YM/close" >/dev/null

echo "========== PAY-TC-003/004 — factor + OT from TIM =========="
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/run" >/dev/null
PERIOD=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM")
python3 - <<'PY' "$PERIOD"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
line = data["lines"][0]
factor = float(line["timeWageFactor"])
assert factor in (0.85, 1.0), line  # master 0.85 if TV else 1.0
assert float(line["ot15"]) == 2 and float(line["ot20"]) == 1, line
print("factor=", factor, "OT from TIM OK:", line)
PY

echo ""
echo "OK — PAY slice B (probation factor + OT from TIM)"
