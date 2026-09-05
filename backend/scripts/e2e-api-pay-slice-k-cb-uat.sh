#!/usr/bin/env bash
# PAY slice K — align engine/master với quy chế C&B (PAY-FR-018 UAT Δ=0).
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

TS=$(date +%s)
YM=$(python3 - <<PY
ts = int("$TS")
print(f"{2040 + (ts % 40)}-{(ts // 40) % 12 + 1:02d}")
PY
)
echo "periodYm=$YM"

create_emp() {
  local code=$1 name=$2
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
    -d "{\"employeeCode\":\"$code\",\"fullName\":\"$name\",\"emailCty\":\"$code@$TS.test.local\",\"orgUnitCode\":\"ORG-HQ\",\"contract\":{\"contractType\":\"OFFICIAL\",\"startDate\":\"2025-01-01\",\"endDate\":null,\"isProbation\":false}}" \
    "$BASE/v1/emp/employees" >/dev/null
}

upsert_salary() {
  local code=$1 amount=$2 deps=$3
  curl -sf -X PUT -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
    -d "{\"employeeCode\":\"$code\",\"amount\":$amount,\"dependentCount\":$deps}" \
    "$BASE/v1/pay/contract-salaries" >/dev/null
}

upsert_pc() {
  local code=$1 pc=$2 amount=$3
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
    -d "{\"periodYm\":\"$YM\",\"employeeCode\":\"$code\",\"code\":\"$pc\",\"amount\":$amount}" \
    "$BASE/v1/pay/monthly-allowances" >/dev/null
}

echo "========== Seed NV001–NV005 (C&B) =========="
# Unique codes per run to avoid 409 on re-run
NV1="CB1-$TS"; NV2="CB2-$TS"; NV3="CB3-$TS"; NV4="CB4-$TS"; NV5="CB5-$TS"
create_emp "$NV1" "Nguyen Van A"
create_emp "$NV2" "Tran Thi B"
create_emp "$NV3" "Le Van C"
create_emp "$NV4" "Pham Thi D"
create_emp "$NV5" "Hoang Van E"

upsert_salary "$NV1" 25000000 1
upsert_salary "$NV2" 15000000 0
upsert_salary "$NV3" 12000000 2
upsert_salary "$NV4" 18000000 0
upsert_salary "$NV5" 10000000 1

# I / J / K (+ V tạm ứng)
upsert_pc "$NV1" "PC-TRACHNHIEM" 2000000
upsert_pc "$NV1" "PC-ANTRUA" 730000
upsert_pc "$NV1" "PC-DIENTHOAI" 500000

upsert_pc "$NV2" "PC-ANTRUA" 730000
upsert_pc "$NV2" "PC-DIENTHOAI" 300000
upsert_pc "$NV2" "PC-TAMUNG" 1000000

upsert_pc "$NV3" "PC-ANTRUA" 730000
upsert_pc "$NV3" "PC-DIENTHOAI" 300000

upsert_pc "$NV4" "PC-TRACHNHIEM" 1000000
upsert_pc "$NV4" "PC-ANTRUA" 730000
upsert_pc "$NV4" "PC-DIENTHOAI" 300000

upsert_pc "$NV5" "PC-ANTRUA" 730000
upsert_pc "$NV5" "PC-DIENTHOAI" 300000

curl -sf -X PUT -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"standardWorkDays":26}' "$BASE/v1/pay/calendar/$YM" >/dev/null

echo "========== TIM Closed $YM =========="
ROWS=$(python3 - <<PY
import json
rows=[
  {"rowNumber":1,"employeeCode":"$NV1","workDays":26,"otUnclassified":0},
  {"rowNumber":2,"employeeCode":"$NV2","workDays":25,"otUnclassified":0},
  {"rowNumber":3,"employeeCode":"$NV3","workDays":26,"otUnclassified":0},
  {"rowNumber":4,"employeeCode":"$NV4","workDays":24,"otUnclassified":0},
  {"rowNumber":5,"employeeCode":"$NV5","workDays":26,"otUnclassified":0},
]
print(json.dumps({"periodYm":"$YM","templateVersionCode":"$VERSION","fileName":"pay-k.csv","rows":rows}))
PY
)
PREVIEW=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "$ROWS" "$BASE/v1/tim/imports")
BATCH=$(python3 - <<'PY' "$PREVIEW"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/imports/$BATCH/commit" >/dev/null
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/periods/$YM/close" >/dev/null

echo "========== PAY run + assert Δ=0 vs C&B =========="
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/run" >/dev/null
PERIOD=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM")
python3 - <<'PY' "$PERIOD" "$NV1" "$NV2" "$NV3" "$NV4" "$NV5"
import json, sys, decimal
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
expected = {
  sys.argv[2]: decimal.Decimal("24907500"),
  sys.argv[3]: decimal.Decimal("12770673.15"),
  sys.argv[4]: decimal.Decimal("11770000"),
  sys.argv[5]: decimal.Decimal("16502846.5"),
  sys.argv[6]: decimal.Decimal("9980000"),
}
by_code = {l["employeeCode"]: l for l in data["lines"]}
for code, want in expected.items():
  got = decimal.Decimal(str(by_code[code]["netPay"]))
  assert got == want, (code, got, want)
  print(f"OK {code} netPay={got}")
print("all Δ=0")
PY

curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/pay/periods/$YM/close" >/dev/null

echo ""
echo "OK — PAY slice K (C&B FR-018 Δ=0)"
