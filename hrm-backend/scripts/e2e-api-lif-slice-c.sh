#!/usr/bin/env bash
# LIF slice C — N+3 lock Git+CRM SP together; HR 403; no early without CR (FR-005…008/010/014).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}
auth_hdr() { echo "Authorization: Bearer $1"; }

HR_TOKEN=$(token local-dev)
IT_TOKEN=$(token it-dev)

CODE="LIF-C-$(date +%s | tail -c 6)"
EMP=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeCode\":\"$CODE\",\"fullName\":\"LIF C\",\"emailCty\":\"$CODE@test.local\",\"orgUnitCode\":\"ORG-HQ\",\"contract\":{\"contractType\":\"OFFICIAL\",\"startDate\":\"2025-01-01\",\"endDate\":null,\"isProbation\":false}}" \
  "$BASE/v1/emp/employees")
EMP_ID=$(python3 -c 'import json,sys; d=json.load(sys.stdin); d=d.get("data",d); print(d["id"])' <<<"$EMP")

CASE=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeId\":\"$EMP_ID\",\"resignationSignedDate\":\"2026-08-01\"}" \
  "$BASE/v1/lif/offboarding")
CASE_ID=$(python3 -c 'import json,sys; d=json.load(sys.stdin); d=d.get("data",d); print(d["id"])' <<<"$CASE")

curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"lastWorkingDayN":"2026-09-30"}' \
  "$BASE/v1/lif/offboarding/$CASE_ID/confirm-n" >/dev/null

echo "========== FR-008 HR lock → 403 =========="
HTTP=$(curl -s -o /tmp/lif-c-hr.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"asOfDate":"2026-10-03"}' \
  "$BASE/v1/lif/offboarding/$CASE_ID/locks")
echo "HR locks → HTTP $HTTP"
[[ "$HTTP" == "403" ]] || { cat /tmp/lif-c-hr.json; exit 1; }

echo "========== FR-007 early without CR → 400 =========="
HTTP=$(curl -s -o /tmp/lif-c-early.json -w "%{http_code}" -X POST \
  -H "$(auth_hdr "$IT_TOKEN")" -H "Content-Type: application/json" \
  -d '{"asOfDate":"2026-10-01"}' \
  "$BASE/v1/lif/offboarding/$CASE_ID/locks")
echo "early → HTTP $HTTP"
[[ "$HTTP" == "400" ]] || { cat /tmp/lif-c-early.json; exit 1; }

echo "========== Job before N+3 → locked 0 =========="
JOB1=$(curl -sf -X POST -H "$(auth_hdr "$IT_TOKEN")" -H "Content-Type: application/json" \
  -d '{"asOfDate":"2026-10-01"}' \
  "$BASE/v1/lif/offboarding/jobs/nplus3-locks")
python3 - <<'PY' "$JOB1"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["locked"] == 0, d
print("job early OK skippedNotDue=", d.get("skippedNotDue"))
PY

echo "========== Job at N+3 → Git+CRM SP together =========="
JOB2=$(curl -sf -X POST -H "$(auth_hdr "$IT_TOKEN")" -H "Content-Type: application/json" \
  -d '{"asOfDate":"2026-10-03"}' \
  "$BASE/v1/lif/offboarding/jobs/nplus3-locks")
python3 - <<'PY' "$JOB2"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["locked"] >= 1, d
print("job due OK locked=", d["locked"])
PY

GOT=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/lif/offboarding/$CASE_ID")
python3 - <<'PY' "$GOT"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["gitLocked"] is True and d["crmSpLocked"] is True, d
assert d["jobNPlus3Eligible"] is False
assert d.get("isEarlySecurityCr") in (False, None)
print("locks OK", d.get("lockedAtUtc"))
PY

echo "========== FR-014 early CR path =========="
CODE2="LIF-C2-$(date +%s | tail -c 5)"
EMP2=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeCode\":\"$CODE2\",\"fullName\":\"LIF C2\",\"emailCty\":\"$CODE2@test.local\",\"orgUnitCode\":\"ORG-HQ\",\"contract\":{\"contractType\":\"OFFICIAL\",\"startDate\":\"2025-01-01\",\"endDate\":null,\"isProbation\":false}}" \
  "$BASE/v1/emp/employees")
EMP2_ID=$(python3 -c 'import json,sys; d=json.load(sys.stdin); d=d.get("data",d); print(d["id"])' <<<"$EMP2")
CASE2=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeId\":\"$EMP2_ID\"}" "$BASE/v1/lif/offboarding")
CASE2_ID=$(python3 -c 'import json,sys; d=json.load(sys.stdin); d=d.get("data",d); print(d["id"])' <<<"$CASE2")
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"lastWorkingDayN":"2026-09-30"}' \
  "$BASE/v1/lif/offboarding/$CASE2_ID/confirm-n" >/dev/null
EARLY=$(curl -sf -X POST -H "$(auth_hdr "$IT_TOKEN")" -H "Content-Type: application/json" \
  -d '{"asOfDate":"2026-10-01","earlyCrReason":"CR-SEC test"}' \
  "$BASE/v1/lif/offboarding/$CASE2_ID/locks")
python3 - <<'PY' "$EARLY"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["gitLocked"] and d["crmSpLocked"]
assert d["isEarlySecurityCr"] is True
assert "CR-SEC" in (d.get("earlyCrReason") or "")
print("early CR OK")
PY

echo "========== LIF-TC-014 / NFR-002 — EmpAudit N + AccessLocked =========="
AUDIT_N=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/emp/employees/$EMP2_ID/audit-logs")
python3 - <<'PY' "$AUDIT_N"
import json, sys
d = json.loads(sys.argv[1])
rows = d.get("data", d) if isinstance(d, dict) else d
actions = {r.get("action") for r in rows}
assert "LifOffboardingNConfirmed" in actions, actions
assert "LifOffboardingAccessLocked" in actions, actions
locked = next(r for r in rows if r.get("action") == "LifOffboardingAccessLocked")
assert "CR-SEC" in (locked.get("detail") or ""), locked
print("LIF audit EmpLog OK", sorted(actions))
PY

echo ""
echo "OK — LIF slice C (N+3 Git+CRM SP locks)"
