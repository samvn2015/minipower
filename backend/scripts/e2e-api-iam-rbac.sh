#!/usr/bin/env bash
# IAM RBAC gaps (không Lark JWKS) — IT 403 PAY · NV/LM 403 TIM import · PGD 403 phiếu người khác · payslip audit.
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}
auth_hdr() { echo "Authorization: Bearer $1"; }

HR_TOKEN=$(token local-dev)
IT_TOKEN=$(token it-dev)
LM_TOKEN=$(token local-lm)
LM_ACCOUNT="11111111-1111-1111-1111-111111111111"

ACTIVE=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/templates/active")
VERSION=$(python3 - <<'PY' "$ACTIVE"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["versionCode"])
PY
)

seed_tim_closed() {
  local ym=$1
  PREVIEW=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
    -d "{\"periodYm\":\"$ym\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"iam-rbac.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":20,\"otUnclassified\":0}]}" \
    "$BASE/v1/tim/imports")
  BATCH=$(python3 - <<'PY' "$PREVIEW"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH/commit" >/dev/null
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/$ym/close" >/dev/null
}

YM=2031-03
echo "========== Seed Closed TIM+PAY $YM =========="
seed_tim_closed "$YM"
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/run" >/dev/null
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/close" >/dev/null

ME=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/payslips/me")
SLIP_ID=$(python3 - <<'PY' "$ME" "$YM"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
ym = sys.argv[2]
print(next(x["id"] for x in data if x["periodYm"] == ym))
PY
)

echo "========== IT → PAY run 403 =========="
HTTP=$(curl -s -o /tmp/iam-it-pay.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$IT_TOKEN")" "$BASE/v1/pay/periods/2031-04/run")
echo "IT pay run → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/iam-it-pay.json; exit 1; }

echo "========== IT → payslip người khác 403 =========="
HTTP=$(curl -s -o /tmp/iam-it-slip.json -w "%{http_code}" \
  -H "$(auth_hdr "$IT_TOKEN")" "$BASE/v1/pay/payslips/$SLIP_ID")
echo "IT payslip → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/iam-it-slip.json; exit 1; }

echo "========== LM/NV → TIM import 403 =========="
HTTP=$(curl -s -o /tmp/iam-lm-tim.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$LM_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"2031-05\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"x.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-HO\",\"workDays\":1}]}" \
  "$BASE/v1/tim/imports")
echo "LM TIM import → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/iam-lm-tim.json; exit 1; }

echo "========== LM → TIM periods 403 =========="
HTTP=$(curl -s -o /tmp/iam-lm-period.json -w "%{http_code}" \
  -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/tim/periods")
echo "LM TIM periods → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/iam-lm-period.json; exit 1; }

echo "========== Assign PGD to LM, phiếu Cty vẫn 403 =========="
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"roleCode":"IAM-ROLE-PGD"}' \
  "$BASE/v1/iam/accounts/$LM_ACCOUNT/roles" >/dev/null || true
PGD_TOKEN=$(token local-lm)
HTTP=$(curl -s -o /tmp/iam-pgd-slip.json -w "%{http_code}" \
  -H "$(auth_hdr "$PGD_TOKEN")" "$BASE/v1/pay/payslips/$SLIP_ID")
echo "PGD→MNV-DEV slip → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/iam-pgd-slip.json; exit 1; }
curl -sf -X DELETE -H "$(auth_hdr "$HR_TOKEN")" \
  "$BASE/v1/iam/accounts/$LM_ACCOUNT/roles/IAM-ROLE-PGD" >/dev/null || true

echo "========== Owner view payslip → PayslipViewed audit =========="
EMP_DEV="bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/payslips/$SLIP_ID" >/dev/null
AUDIT=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/emp/employees/$EMP_DEV/audit-logs")
python3 - <<'PY' "$AUDIT"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert any(x.get("action") == "PayslipViewed" for x in data), data
print("PayslipViewed audit OK")
PY

echo "========== IT được xem TIM template (HR|IT) =========="
curl -sf -H "$(auth_hdr "$IT_TOKEN")" "$BASE/v1/tim/templates/active" >/dev/null
echo "IT template OK"

echo ""
echo "OK — IAM RBAC (IT/LM/PGD 403 + payslip audit; no Lark JWKS)"
