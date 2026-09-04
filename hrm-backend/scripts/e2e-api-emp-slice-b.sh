#!/usr/bin/env bash
# EMP slice B — contract-type master (FR-014), inactive org (FR-016), LM payslip 403 (FR-015).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}
auth_hdr() { echo "Authorization: Bearer $1"; }

HR_TOKEN=$(token local-dev)
LM_TOKEN=$(token local-lm)
TS=$(date +%s)

echo "========== EMP-TC-014 — contract-types master =========="
TYPES=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/emp/contract-types")
python3 - <<'PY' "$TYPES"
import json, sys
rows = json.loads(sys.argv[1])
rows = rows.get("data", rows)
codes = {r["code"] for r in rows}
assert "PROBATION" in codes and "OFFICIAL" in codes, codes
print("contract-types OK", sorted(codes))
PY

echo "========== FR-014 bad contract type → 400 =========="
HTTP=$(curl -s -o /tmp/emp-b-ctype.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeCode\":\"MNV-BADCT-$TS\",\"fullName\":\"Bad CT\",\"orgUnitCode\":\"ORG-HQ\",\"contract\":{\"contractType\":\"HARDCODE-URD\",\"startDate\":\"2026-01-01\",\"endDate\":null,\"isProbation\":false}}" \
  "$BASE/v1/emp/employees")
echo "bad type → HTTP $HTTP"
[[ "$HTTP" == "400" ]] || { cat /tmp/emp-b-ctype.json; exit 1; }

echo "========== EMP-TC-016 — org inactive → 400 =========="
HTTP=$(curl -s -o /tmp/emp-b-org.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeCode\":\"MNV-BADORG-$TS\",\"fullName\":\"Bad Org\",\"orgUnitCode\":\"ORG-INACTIVE\"}" \
  "$BASE/v1/emp/employees")
echo "inactive org → HTTP $HTTP"
[[ "$HTTP" == "400" ]] || { cat /tmp/emp-b-org.json; exit 1; }

echo "========== EMP-TC-015 — LM 403 payslip (pair IAM/PAY) =========="
# Reuse closed period if exists; else light seed via TIM+PAY for MNV-DEV
ACTIVE=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/templates/active")
VERSION=$(python3 -c 'import json,sys; d=json.load(sys.stdin); d=d.get("data",d); print(d["versionCode"])' <<<"$ACTIVE")
YM="2029-01"
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"periodYm\":\"$YM\",\"templateVersionCode\":\"$VERSION\",\"fileName\":\"emp-b.csv\",\"rows\":[{\"rowNumber\":1,\"employeeCode\":\"MNV-DEV\",\"workDays\":20,\"otUnclassified\":0}]}" \
  "$BASE/v1/tim/imports" >/tmp/emp-b-imp.json || true
BATCH=$(python3 -c 'import json; d=json.load(open("/tmp/emp-b-imp.json")); d=d.get("data",d); print(d.get("id",""))' 2>/dev/null || true)
if [[ -n "${BATCH:-}" ]]; then
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH/commit" >/dev/null || true
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/$YM/close" >/dev/null || true
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/run" >/dev/null || true
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/close" >/dev/null || true
fi
ME=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/payslips/me" || echo '[]')
SLIP=$(python3 - <<'PY' "$ME"
import json, sys
d = json.loads(sys.argv[1])
d = d.get("data", d)
if not d:
    print("")
else:
    print(d[0]["id"])
PY
)
if [[ -n "$SLIP" ]]; then
  HTTP=$(curl -s -o /tmp/emp-b-slip.json -w "%{http_code}" \
    -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/pay/payslips/$SLIP")
  echo "LM payslip → HTTP $HTTP"
  [[ "$HTTP" == "403" ]] || { cat /tmp/emp-b-slip.json; exit 1; }
else
  echo "SKIP payslip (no slip yet) — covered by pay-slice-f"
fi

echo ""
echo "OK — EMP slice B (FR-014/015/016)"
