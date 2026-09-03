#!/usr/bin/env bash
# PAY slice I — A-001 paid-leave warning (FR-013) + preview columns (FR-014 smoke).
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

# Leave balance seed covers 2026; pick a free month.
YM=2026-09
FROM="${YM}-22"
TO="${YM}-23"

echo "========== Seed Approved leave + TIM Closed $YM =========="
CREATE=$(curl -sf -X POST -H "$(auth_hdr "$NV_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"leaveTypeCode\":\"LEV-ANNUAL\",\"fromDate\":\"$FROM\",\"toDate\":\"$TO\",\"dayPart\":\"FullDay\",\"reason\":\"PAY I A-001\",\"handoverEmployeeId\":\"$HANDOVER_EMP\"}" \
  "$BASE/v1/lev/leave-requests")
REQ=$(python3 - <<'PY' "$CREATE"
import json, sys
body = json.loads(sys.argv[1])
data = body["data"] if isinstance(body.get("data"), dict) else body
print(data["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/lev/leave-requests/$REQ/c1/approve" >/dev/null
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/lev/leave-requests/$REQ/c2/approve" >/dev/null

# Unlock TIM if previously closed (idempotent best-effort)
curl -s -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/$YM/unlock" >/dev/null || true

PREVIEW=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"$YM\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"pay-i.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":18,\"otUnclassified\":0}]}" \
  "$BASE/v1/tim/imports")
BATCH=$(python3 - <<'PY' "$PREVIEW"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH/commit" >/dev/null
CLOSE=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/$YM/close")
python3 - <<'PY' "$CLOSE"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert float(data.get("totalLeaveDaysPaid", 0)) > 0, data
print("TIM closed; leaveDaysPaid=", data["totalLeaveDaysPaid"])
PY

echo "========== PAY-TC-013 — A-001 on run =========="
# If PAY already Closed for YM, skip (use unique YM in CI); else run Draft
RUN_HTTP=$(curl -s -o /tmp/pay-i-run.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/run")
if [[ "$RUN_HTTP" == "409" ]]; then
  echo "PAY already Closed for $YM — check GET warnings instead"
else
  [[ "$RUN_HTTP" == "200" ]] || { cat /tmp/pay-i-run.json; exit 1; }
  python3 - <<'PY'
import json
data = json.load(open("/tmp/pay-i-run.json")).get("data", json.load(open("/tmp/pay-i-run.json")))
# re-read file once
raw = open("/tmp/pay-i-run.json").read()
body = json.loads(raw)
data = body.get("data", body)
assert data["status"] == "Draft", data
warns = data.get("warnings") or []
assert any("A-001" in w for w in warns), warns
assert any("PAY-FR-013" in w for w in warns), warns
print("run A-001 OK", warns[0])
PY
fi

echo "========== PAY-TC-014 smoke — preview columns + nTinh =========="
PERIOD=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM")
python3 - <<'PY' "$PERIOD"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
line = data["lines"][0]
for k in ("workDays","leaveDaysUnpaid","leaveDaysPaid","nTinh","timeWageFactor","ot15","contractAllowance","monthlyAllowance","bhAmount","tncnAmount","netPay"):
  assert k in line, k
paid = float(line["leaveDaysPaid"])
n = float(line["nTinh"])
work = float(line["workDays"])
unpaid = float(line["leaveDaysUnpaid"])
assert paid > 0, line
assert abs(n - (work - unpaid)) < 0.001, (n, work, unpaid, paid)
warns = data.get("warnings") or []
assert any("A-001" in w for w in warns), warns
print("preview FR-013/014 OK; nTinh", n, "work", work, "paid", paid)
PY

echo ""
echo "OK — PAY slice I (FR-013 + FR-014 smoke)"
