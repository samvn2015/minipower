#!/usr/bin/env bash
# PRB slice A — milestones from EMP only (FR-001/008/015).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}
auth_hdr() { echo "Authorization: Bearer $1"; }

HR_TOKEN=$(token local-dev)
LM_TOKEN=$(token local-lm)

CODE="PRB-A-$(date +%s | tail -c 6)"
EMAIL="${CODE}@test.local"
START="2026-01-01"
END="2026-06-30"

echo "========== Create TV employee $CODE =========="
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeCode\":\"$CODE\",\"fullName\":\"PRB A NV\",\"emailCty\":\"$EMAIL\",\"orgUnitCode\":\"ORG-HQ\",\"contract\":{\"contractType\":\"PROBATION\",\"startDate\":\"$START\",\"endDate\":\"$END\",\"isProbation\":true}}" \
  "$BASE/v1/emp/employees" >/dev/null

echo "========== FR-001/008 — HR list cases includes EMP dates =========="
CASES=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/prb/cases")
python3 - <<'PY' "$CASES" "$CODE" "$START" "$END"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
code, start, end = sys.argv[2], sys.argv[3], sys.argv[4]
row = next(x for x in data if x["employeeCode"] == code)
assert row["probationStartDate"].startswith(start), row
assert row["probationEndDate"].startswith(end), row
assert row["hasCompleteMilestone"] is True
assert row["t15DueDate"].startswith("2026-06-15"), row  # END - 15
assert row["t7DueDate"].startswith("2026-06-23"), row   # END - 7
print("HR cases OK")
PY

echo "========== FR-015 — LM/NV cannot list HR cases =========="
HTTP=$(curl -s -o /tmp/prb-a-lm.json -w "%{http_code}" -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/prb/cases")
echo "LM cases → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/prb-a-lm.json; exit 1; }

echo "========== milestones/me for HR (MNV-DEV) — source EMP =========="
ME=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/prb/milestones/me")
python3 - <<'PY' "$ME"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["source"] == "EMP.Contract", d
print("milestones/me source OK", d.get("employeeCode"), "onProbation=", d.get("isOnProbation"))
PY

echo ""
echo "OK — PRB slice A (EMP milestones)"
