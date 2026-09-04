#!/usr/bin/env bash
# LEV slice F — advance notice + attachment + notify + FR-004/005/014.
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
YEAR=2026
TS=$(date +%s)
# Ngày unique theo giây để re-run không overlap đơn cũ
D1=$(python3 - <<PY
from datetime import date, timedelta
base = date(2026, 8, 3) + timedelta(days=int("$TS") % 20)
print(base.isoformat())
PY
)
D_SICK=$(python3 - <<PY
from datetime import date, timedelta
base = date(2026, 8, 10) + timedelta(days=int("$TS") % 15)
print(base.isoformat())
PY
)

echo "========== LEV-TC-004 — vượt quỹ năm =========="
BAL=$(curl -sf -H "$(auth_hdr "$NV_TOKEN")" "$BASE/v1/lev/leave-balances/me?year=$YEAR")
REMAIN=$(python3 - <<'PY' "$BAL"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["remainingDays"])
PY
)
# Khoảng tương lai đủ hạn 3 NLĐ nhưng > quỹ còn
OVER=$(python3 <<PY
from datetime import date, timedelta
today = date.today()
start = today + timedelta(days=21)
# đảm bảo không Chủ nhật
while start.weekday() >= 5:
    start += timedelta(days=1)
end = start + timedelta(days=45)  # >> remaining
print(f"{start.isoformat()}|{end.isoformat()}")
PY
)
OF=${OVER%%|*}; OT=${OVER##*|}
HTTP=$(curl -s -o /tmp/lev-f-over.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$NV_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"leaveTypeCode\":\"LEV-ANNUAL\",\"fromDate\":\"$OF\",\"toDate\":\"$OT\",\"dayPart\":\"FullDay\",\"reason\":\"Over balance\",\"handoverEmployeeId\":\"$HANDOVER_EMP\"}" \
  "$BASE/v1/lev/leave-requests")
echo "over balance remain=$REMAIN $OF..$OT → HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/lev-f-over.json; exit 1; }
grep -qi "FR-004\\|quỹ" /tmp/lev-f-over.json

echo "========== LEV-TC-005 — loại không trừ quỹ (UNPAID) =========="
CREATE=$(curl -sf -X POST -H "$(auth_hdr "$NV_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"leaveTypeCode\":\"LEV-UNPAID\",\"fromDate\":\"$D1\",\"toDate\":\"$D1\",\"dayPart\":\"FullDay\",\"reason\":\"Unpaid OK\",\"handoverEmployeeId\":\"$HANDOVER_EMP\"}" \
  "$BASE/v1/lev/leave-requests")
UNPAID_ID=$(python3 - <<'PY' "$CREATE"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/lev/leave-requests/$UNPAID_ID/c1/approve" >/dev/null
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/lev/leave-requests/$UNPAID_ID/c2/approve" >/dev/null
BAL1=$(curl -sf -H "$(auth_hdr "$NV_TOKEN")" "$BASE/v1/lev/leave-balances/me?year=$YEAR")
python3 - <<PY "$BAL1" "$REMAIN"
import json, sys
got = float(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["remainingDays"])
assert got == float(sys.argv[2]), (got, sys.argv[2])
print("unpaid C2 balance unchanged:", got)
PY

echo "========== LEV-TC-006 — ≥3 ngày công, nộp trễ, không đột xuất =========="
# Wed–Fri 2026-10-07..09; submit “as of” gần ngày bắt đầu → API dùng UtcNow nên chọn khoảng trong quá khứ gần?
# Host dùng DateTime.UtcNow — chọn fromDate = today+1 với 3 ngày công (khó deterministic).
# Dùng khoảng tương lai ngắn: nếu hôm nay gần start → 400.
# Fixed: 3 ngày công bắt đầu sau 1 NLĐ từ “tomorrow-ish” — dùng python tính.
RANGE=$(python3 <<'PY'
from datetime import date, timedelta
def is_work(d):
    return d.weekday() < 5
today = date.today()
d = today + timedelta(days=1)
for _ in range(60):
    if d.weekday() == 2:  # Wed
        fri = d + timedelta(days=2)
        n = 0
        x = today + timedelta(days=1)
        while x < d:
            if is_work(x): n += 1
            x += timedelta(days=1)
        if n < 3:
            print(f"{d.isoformat()}|{fri.isoformat()}")
            break
    d += timedelta(days=1)
else:
    raise SystemExit("no late Wed-Fri window in 60d")
PY
)
FROM=${RANGE%%|*}; TO=${RANGE##*|}
HTTP=$(curl -s -o /tmp/lev-f-late.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$NV_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"leaveTypeCode\":\"LEV-ANNUAL\",\"fromDate\":\"$FROM\",\"toDate\":\"$TO\",\"dayPart\":\"FullDay\",\"reason\":\"Late\",\"handoverEmployeeId\":\"$HANDOVER_EMP\",\"isEmergency\":false}" \
  "$BASE/v1/lev/leave-requests")
echo "late $FROM..$TO → HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/lev-f-late.json; exit 1; }
grep -q "LEV-FR-006" /tmp/lev-f-late.json || grep -qi "đột xuất" /tmp/lev-f-late.json

echo "========== LEV-TC-007 — đột xuất được submit; C1 không trừ quỹ =========="
# Cùng cửa sổ trễ + offset tuần để tránh overlap đơn cũ trên DB shared
EM_RANGE=$(python3 <<PY
from datetime import date, timedelta
f = date.fromisoformat("$FROM") + timedelta(days=28)
t = date.fromisoformat("$TO") + timedelta(days=28)
print(f"{f.isoformat()}|{t.isoformat()}")
PY
)
EM_FROM=${EM_RANGE%%|*}; EM_TO=${EM_RANGE##*|}
CREATE=$(curl -sf -X POST -H "$(auth_hdr "$NV_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"leaveTypeCode\":\"LEV-UNPAID\",\"fromDate\":\"$EM_FROM\",\"toDate\":\"$EM_TO\",\"dayPart\":\"FullDay\",\"reason\":\"Emergency\",\"handoverEmployeeId\":\"$HANDOVER_EMP\",\"isEmergency\":true}" \
  "$BASE/v1/lev/leave-requests")
EM_ID=$(python3 - <<'PY' "$CREATE"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/lev/leave-requests/$EM_ID/c1/approve" >/dev/null
BAL2=$(curl -sf -H "$(auth_hdr "$NV_TOKEN")" "$BASE/v1/lev/leave-balances/me?year=$YEAR")
python3 - <<PY "$BAL2" "$REMAIN"
import json, sys
got = float(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["remainingDays"])
assert got == float(sys.argv[2]), (got, sys.argv[2])
print("after emergency C1 balance still:", got, "range", "$EM_FROM", "$EM_TO")
PY

echo "========== LEV-TC-008 — ốm thiếu file mẫu =========="
HTTP=$(curl -s -o /tmp/lev-f-sick.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$NV_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"leaveTypeCode\":\"LEV-SICK\",\"fromDate\":\"$D_SICK\",\"toDate\":\"$D_SICK\",\"dayPart\":\"FullDay\",\"reason\":\"Sick\",\"handoverEmployeeId\":\"$HANDOVER_EMP\"}" \
  "$BASE/v1/lev/leave-requests")
echo "sick no file → HTTP $HTTP (expect 400)"
[[ "$HTTP" == "400" ]] || { cat /tmp/lev-f-sick.json; exit 1; }

CREATE=$(curl -sf -X POST -H "$(auth_hdr "$NV_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"leaveTypeCode\":\"LEV-SICK\",\"fromDate\":\"$D_SICK\",\"toDate\":\"$D_SICK\",\"dayPart\":\"FullDay\",\"reason\":\"Sick\",\"handoverEmployeeId\":\"$HANDOVER_EMP\",\"attachmentFileName\":\"mau-cty-om.pdf\",\"attachmentMatchesCompanyTemplate\":true}" \
  "$BASE/v1/lev/leave-requests")
python3 - <<'PY' "$CREATE"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "PendingC1", data
print("sick with template OK", data["id"])
PY

echo "========== LEV-TC-009 — notify Email/InApp, không CRM =========="
NOTIF=$(curl -sf -H "$(auth_hdr "$NV_TOKEN")" "$BASE/v1/lev/notifications/me")
python3 - <<'PY' "$NOTIF"
import json, sys
rows = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert rows, "expected notifications"
channels = {r["channel"] for r in rows}
assert "Email" in channels and "InApp" in channels, channels
assert not any("CRM" in c.upper() or "SALES" in c.upper() for c in channels), channels
print("notifications OK", len(rows), sorted(channels))
PY

echo "========== LEV-TC-014 — hủy sau C2 =========="
# Dùng unpaid đã Approved ở trên
HTTP=$(curl -s -o /tmp/lev-f-cancel.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$NV_TOKEN")" "$BASE/v1/lev/leave-requests/$UNPAID_ID/cancel")
echo "cancel after C2 → HTTP $HTTP (expect 409)"
[[ "$HTTP" == "409" ]] || { cat /tmp/lev-f-cancel.json; exit 1; }
grep -q "LEV-FR-014" /tmp/lev-f-cancel.json

echo ""
echo "OK — LEV slice F (advance notice + attach + notify + FR-004/005/014)"
