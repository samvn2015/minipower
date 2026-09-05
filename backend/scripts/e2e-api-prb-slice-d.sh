#!/usr/bin/env bash
# PRB slice D — PASS→EMP official · EXTEND→KT · FAIL→LIF (FR-005/006/007/016).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}
auth_hdr() { echo "Authorization: Bearer $1"; }

HR_TOKEN=$(token local-dev)

create_tv() {
  local code=$1 end=$2
  curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
    -d "{\"employeeCode\":\"$code\",\"fullName\":\"$code\",\"emailCty\":\"$code@test.local\",\"orgUnitCode\":\"ORG-HQ\",\"contract\":{\"contractType\":\"PROBATION\",\"startDate\":\"2026-01-01\",\"endDate\":\"$end\",\"isProbation\":true}}" \
    "$BASE/v1/emp/employees" | python3 -c 'import json,sys; d=json.load(sys.stdin); d=d.get("data",d); print(d["id"])'
}

echo "========== PASS → OFFICIAL =========="
CODE_P="PRB-DP-$(date +%s | tail -c 5)"
ID_P=$(create_tv "$CODE_P" "2026-06-30")
DEC=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"outcomeCode":"PASS"}' "$BASE/v1/prb/evaluations/$ID_P/decide")
python3 - <<'PY' "$DEC"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["contractConvertedToOfficial"] is True, d
print("pass effect OK")
PY
EMP=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/emp/employees/$ID_P")
python3 - <<'PY' "$EMP"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
c = d["contract"]
assert c["contractType"] == "OFFICIAL" and c["isProbation"] is False, c
print("EMP official OK")
PY

echo "========== EXTEND → KT+1M =========="
CODE_E="PRB-DE-$(date +%s | tail -c 5)"
ID_E=$(create_tv "$CODE_E" "2026-06-30")
DEC=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"outcomeCode":"EXTEND","extendDurationCode":"EXT-1M"}' "$BASE/v1/prb/evaluations/$ID_E/decide")
python3 - <<'PY' "$DEC"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["newProbationEndDate"].startswith("2026-07-30"), d
print("extend effect OK")
PY

echo "========== FAIL → LIF open, EMP remains =========="
CODE_F="PRB-DF-$(date +%s | tail -c 5)"
ID_F=$(create_tv "$CODE_F" "2026-06-30")
DEC=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"outcomeCode":"FAIL","note":"không đạt"}' "$BASE/v1/prb/evaluations/$ID_F/decide")
LIF_ID=$(python3 - <<'PY' "$DEC"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d.get("lifOffboardingCaseId"), d
print(d["lifOffboardingCaseId"])
PY
)
curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/emp/employees/$ID_F" >/dev/null
LIF=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/lif/offboarding/open")
python3 - <<'PY' "$LIF" "$CODE_F" "$LIF_ID"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
code, lid = sys.argv[2], sys.argv[3]
row = next(x for x in data if x["employeeCode"] == code)
assert row["id"] == lid
assert row["source"] == "PRB-FAIL"
assert row["status"] == "Open"
assert row.get("lastWorkingDayN") in (None, "")
print("LIF open OK")
PY

echo ""
echo "OK — PRB slice D (outcomes → EMP/LIF)"
