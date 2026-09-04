#!/usr/bin/env bash
# PRB slice E — no-LM T-7 → HR · decide without propose · ProbationDecided audit.
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

echo "========== T-7 no LM → HR assignee =========="
# KT=2026-07-20 → T-7 = 2026-07-13
CODE_R="PRB-ER-$(date +%s | tail -c 5)"
ID_R=$(create_tv "$CODE_R" "2026-07-20")
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"asOfDate":"2026-07-13"}' "$BASE/v1/prb/jobs/reminders/run" >/dev/null
REMS=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/prb/reminders")
python3 - <<'PY' "$REMS" "$CODE_R"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
code = sys.argv[2]
rows = [x for x in data if x["employeeCode"] == code and x["kind"] == "T7"]
assert rows, data
r = rows[0]
assignee = r.get("assigneeEmployeeId")
msg = (r.get("inAppMessage") or "")
ok = assignee in (None, "") or "HR" in msg
assert ok, r
print("T-7 no-LM OK", "assignee=", assignee, "msg=", msg[:80])
PY

echo "========== Decide PASS without propose + audit =========="
CODE_D="PRB-ED-$(date +%s | tail -c 5)"
ID_D=$(create_tv "$CODE_D" "2026-06-30")
DEC=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"outcomeCode":"PASS"}' "$BASE/v1/prb/evaluations/$ID_D/decide")
python3 - <<'PY' "$DEC"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d.get("decidedByIdpSubject"), d
assert d.get("decidedOutcomeCode") == "PASS", d
print("decide PASS OK", d["decidedByIdpSubject"])
PY

AUDIT=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/emp/employees/$ID_D/audit-logs")
python3 - <<'PY' "$AUDIT"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert any(x.get("action") == "ProbationDecided" for x in data), data
print("ProbationDecided audit OK")
PY

echo ""
echo "OK — PRB slice E (no-LM T-7 + ProbationDecided audit)"
