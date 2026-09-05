#!/usr/bin/env bash
# PRB slice B — T-15 / T-7 reminders (FR-002/003/008/011; no CRM).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}
auth_hdr() { echo "Authorization: Bearer $1"; }

HR_TOKEN=$(token local-dev)

CODE="PRB-B-$(date +%s | tail -c 6)"
EMAIL="${CODE}@test.local"
# KT = 2026-07-20 → T-15 = 2026-07-05, T-7 = 2026-07-13
START="2026-01-01"
END="2026-07-20"

echo "========== Create TV $CODE KT=$END =========="
curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"employeeCode\":\"$CODE\",\"fullName\":\"PRB B NV\",\"emailCty\":\"$EMAIL\",\"orgUnitCode\":\"ORG-HQ\",\"contract\":{\"contractType\":\"PROBATION\",\"startDate\":\"$START\",\"endDate\":\"$END\",\"isProbation\":true}}" \
  "$BASE/v1/emp/employees" >/dev/null

echo "========== FR-002 — run asOf T-15 =========="
RUN=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"asOfDate":"2026-07-05"}' "$BASE/v1/prb/jobs/reminders/run")
python3 - <<'PY' "$RUN"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["t15Created"] >= 1, d
assert d["t7Created"] == 0, d
print("T-15 created", d["t15Created"])
PY

echo "========== FR-003 — run asOf T-7 =========="
RUN2=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"asOfDate":"2026-07-13"}' "$BASE/v1/prb/jobs/reminders/run")
python3 - <<'PY' "$RUN2"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["t7Created"] >= 1, d
print("T-7 created", d["t7Created"])
PY

echo "========== FR-011 channels + FR-010 no CRM =========="
REMS=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/prb/reminders")
python3 - <<'PY' "$REMS" "$CODE" "$EMAIL"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
code, email = sys.argv[2], sys.argv[3]
rows = [x for x in data if x["employeeCode"] == code]
assert any(x["kind"] == "T15" for x in rows), rows
assert any(x["kind"] == "T7" for x in rows), rows
for x in rows:
    assert "inapp" in x["channel"].lower() or "email" in x["channel"].lower(), x
    assert "crm" not in x["channel"].lower()
    assert "sales" not in (x.get("emailTo") or "").lower()
    assert email in x["emailTo"], x
print("reminders OK", len(rows))
PY

echo "========== Idempotent re-run =========="
RUN3=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d '{"asOfDate":"2026-07-05"}' "$BASE/v1/prb/jobs/reminders/run")
python3 - <<'PY' "$RUN3"
import json, sys
d = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert d["t15Created"] == 0, d
assert d["skippedAlreadyExists"] >= 1, d
print("idempotent OK")
PY

echo ""
echo "OK — PRB slice B (T-15/T-7)"
