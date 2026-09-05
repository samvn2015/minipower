#!/usr/bin/env bash
# PAY slice J — mobile parity (FR-011): same APIs/IAM as FR-010 (SCR-005 ≡ SCR-006).
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
    -d "{\"periodYm\":\"$ym\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"pay-j.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":20,\"otUnclassified\":0}]}" \
    "$BASE/v1/tim/imports")
  BATCH=$(python3 - <<'PY' "$PREVIEW"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH/commit" >/dev/null
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/$ym/close" >/dev/null
}

YM=2030-01
echo "========== Closed period $YM =========="
seed_tim_closed "$YM"
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/run" >/dev/null
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/close" >/dev/null

echo "========== FR-011 — owner me + detail (same endpoints as SCR-005) =========="
ME=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/payslips/me")
SLIP_ID=$(python3 - <<'PY' "$ME" "$YM"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
ym = sys.argv[2]
row = next(x for x in data if x["periodYm"] == ym)
# Fields SCR-006 must surface (parity with SCR-005)
for k in ("id", "periodYm", "employeeCode", "status", "nTinh", "timeWageFactor",
          "contractAllowance", "monthlyAllowance", "bhAmount", "tncnAmount", "netPay"):
    assert k in row, k
print(row["id"])
PY
)
DETAIL=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/payslips/$SLIP_ID")
python3 - <<'PY' "$DETAIL" "$SLIP_ID"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["id"] == sys.argv[2]
assert d["status"] == "Closed"
print("owner detail fields OK")
PY

echo "========== FR-011 — LM subordinate still 403 (no wider rights) =========="
HTTP=$(curl -s -o /tmp/pay-j-lm.json -w "%{http_code}" \
  -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/pay/payslips/$SLIP_ID")
echo "LM → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/pay-j-lm.json; exit 1; }
python3 - <<'PY'
import json
body = json.load(open("/tmp/pay-j-lm.json"))
msg = json.dumps(body)
assert "FR-010" in msg or "FR-011" in msg or "phiếu" in msg.lower(), body
print("403 message OK")
PY

echo ""
echo "OK — PAY slice J (mobile parity FR-011; APIs identical to FR-010)"
