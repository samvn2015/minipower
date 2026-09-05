#!/usr/bin/env bash
# TIM slice D — merge Approved leave into N_thực on close (FR-008/009).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}
auth_hdr() { echo "Authorization: Bearer $1"; }

NV_TOKEN=$(token local-dev)
LM_TOKEN=$(token local-lm)
HR_TOKEN=$(token local-dev)
HANDOVER_EMP="dddddddd-dddd-dddd-dddd-dddddddddddd"

ACTIVE=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/templates/active")
VERSION=$(python3 - <<'PY' "$ACTIVE"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["versionCode"])
PY
)

echo "========== TIM-TC-008 N — PendingC1 leave not merged =========="
CREATE_P=$(curl -sf -X POST -H "$(auth_hdr "$NV_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"leaveTypeCode\":\"LEV-ANNUAL\",\"fromDate\":\"2027-03-10\",\"toDate\":\"2027-03-10\",\"dayPart\":\"FullDay\",\"reason\":\"TIM D pending\",\"handoverEmployeeId\":\"$HANDOVER_EMP\"}" \
  "$BASE/v1/lev/leave-requests")
# leave stays PendingC1 — no C1/C2

PREVIEW_P=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"2027-03\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"p.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":20}]}" \
  "$BASE/v1/tim/imports")
BATCH_P=$(python3 - <<'PY' "$PREVIEW_P"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH_P/commit" >/dev/null
CLOSE_P=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/2027-03/close")
python3 - <<'PY' "$CLOSE_P"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "Closed", data
assert float(data.get("totalLeaveDaysPaid", 0)) == 0, data
print("pending not merged:", data)
PY

PERIOD_P=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/2027-03")
python3 - <<'PY' "$PERIOD_P"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
line = data["lines"][0]
assert float(line["workDays"]) == 20, line
assert float(line.get("leaveDaysPaid", 0)) == 0, line
print("N_thực unchanged:", line["workDays"])
PY

echo "========== TIM-TC-008/009 H — Approved leave merged into N_thực =========="
CREATE_A=$(curl -sf -X POST -H "$(auth_hdr "$NV_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"leaveTypeCode\":\"LEV-ANNUAL\",\"fromDate\":\"2027-02-08\",\"toDate\":\"2027-02-09\",\"dayPart\":\"FullDay\",\"reason\":\"TIM D approved\",\"handoverEmployeeId\":\"$HANDOVER_EMP\"}" \
  "$BASE/v1/lev/leave-requests")
REQ_A=$(python3 - <<'PY' "$CREATE_A"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/lev/leave-requests/$REQ_A/c1/approve" >/dev/null
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/lev/leave-requests/$REQ_A/c2/approve" >/dev/null

PREVIEW_A=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"2027-02\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"a.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":20,\"ot15\":0}]}" \
  "$BASE/v1/tim/imports")
BATCH_A=$(python3 - <<'PY' "$PREVIEW_A"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH_A/commit" >/dev/null
CLOSE_A=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/2027-02/close")
python3 - <<'PY' "$CLOSE_A"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "Closed", data
assert float(data["totalLeaveDaysPaid"]) == 2, data
print("closed with leave:", data)
PY

PERIOD_A=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/2027-02")
python3 - <<'PY' "$PERIOD_A"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
line = data["lines"][0]
assert float(line["leaveDaysPaid"]) == 2, line
assert float(line["workDays"]) == 22, line  # 20 import + 2 paid leave (TIM-FR-009)
print("N_thực gồm phép hưởng:", line)
PY

echo ""
echo "OK — TIM slice D (approved leave merge into N_thực)"
