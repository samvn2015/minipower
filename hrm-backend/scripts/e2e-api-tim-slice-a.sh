#!/usr/bin/env bash
# TIM slice A — template master: one Active (FR-001/015), columns from master (FR-002), HR-only publish (FR-011).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=$1" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}
auth_hdr() { echo "Authorization: Bearer $1"; }

HR_TOKEN=$(token local-dev)
LM_TOKEN=$(token local-lm)

echo "========== TIM-TC-001 — active template exists =========="
ACTIVE=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/templates/active")
python3 - <<'PY' "$ACTIVE"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "Active", data
assert data["versionCode"], data
assert len(data["columns"]) >= 1, data
keys = {c["columnKey"] for c in data["columns"]}
assert "mnv" in keys, keys
print("active:", data["versionCode"], "columns:", sorted(keys))
PY

echo "========== TIM-TC-011 — LM cannot list/publish =========="
HTTP=$(curl -s -o /tmp/tim-lm-list.json -w "%{http_code}" -H "$(auth_hdr "$LM_TOKEN")" "$BASE/v1/tim/templates")
echo "LM list → HTTP $HTTP (expect 403)"
[[ "$HTTP" == "403" ]] || { cat /tmp/tim-lm-list.json; exit 1; }

echo "========== create draft + publish V2 =========="
SUFFIX=$(date +%s)
CREATE=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" -H "Content-Type: application/json" \
  -d "{\"versionCode\":\"TIM-V2-$SUFFIX\",\"name\":\"Mẫu V2\",\"columns\":[{\"columnKey\":\"mnv\",\"displayName\":\"Mã NV\",\"sortOrder\":1,\"isRequired\":true,\"mapsTo\":\"EmployeeCode\"},{\"columnKey\":\"n_thuc\",\"displayName\":\"Ngày công\",\"sortOrder\":2,\"isRequired\":true,\"mapsTo\":\"WorkDays\"}]}" \
  "$BASE/v1/tim/templates")
DRAFT_ID=$(python3 - <<'PY' "$CREATE"
import json, sys
print(json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))["id"])
PY
)
PUB=$(curl -sf -X POST -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/templates/$DRAFT_ID/publish")
python3 - <<'PY' "$PUB"
import json, sys
data = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
assert data["status"] == "Active", data
print("published:", data)
PY

echo "========== TIM-TC-015 — still exactly one Active =========="
LIST=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" "$BASE/v1/tim/templates")
python3 - <<'PY' "$LIST"
import json, sys
rows = json.loads(sys.argv[1]).get("data", json.loads(sys.argv[1]))
actives = [r for r in rows if r["status"] == "Active"]
assert len(actives) == 1, actives
print("active count:", len(actives), "code:", actives[0]["versionCode"])
PY

echo "========== TIM-TC-NFR-002 — publish audit =========="
AUDIT=$(curl -sf -H "$(auth_hdr "$HR_TOKEN")" \
  "$BASE/v1/emp/audit-logs?action=TimesheetTemplatePublished&take=5")
python3 - <<'PY' "$AUDIT"
import json, sys
d = json.loads(sys.argv[1])
rows = d.get("data", d) if isinstance(d, dict) else d
assert any(r.get("action") == "TimesheetTemplatePublished" for r in rows), rows
print("TimesheetTemplatePublished audit OK")
PY

echo ""
echo "OK — TIM slice A (template master MVP)"
