#!/usr/bin/env bash
# API unhappy paths — EMP-TC-002/003/004 (unique + org inactive).
set -euo pipefail
BASE="${BASE_URL:-http://localhost:5167}"

token() {
  curl -sf "$BASE/dev/token?sub=local-dev" | python3 -c 'import json,sys; print(json.load(sys.stdin)["accessToken"])'
}

expect_http() {
  local method="$1" url="$2" token="$3" body="${4:-}" expected="$5"
  local http
  if [[ -n "$body" ]]; then
    http=$(curl -s -o /tmp/e2e-api-body.json -w "%{http_code}" -X "$method" \
      -H "Authorization: Bearer $token" -H "Content-Type: application/json" \
      -d "$body" "$url")
  else
    http=$(curl -s -o /tmp/e2e-api-body.json -w "%{http_code}" -X "$method" \
      -H "Authorization: Bearer $token" "$url")
  fi
  echo "$method $url → HTTP $http (expect $expected)"
  [[ "$http" == "$expected" ]] || { cat /tmp/e2e-api-body.json; exit 1; }
}

HR_TOKEN=$(token)
TS=$(date +%s)

echo "========== EMP-TC-002 — trùng MNV =========="
expect_http POST "$BASE/v1/emp/employees" "$HR_TOKEN" \
  '{"employeeCode":"MNV-DEV","fullName":"Dup","orgUnitCode":"ORG-HQ"}' 409

echo "========== EMP-TC-002 — trùng CCCD =========="
CCCD="CCCD-SEED-001"
expect_http POST "$BASE/v1/emp/employees" "$HR_TOKEN" \
  "{\"employeeCode\":\"MNV-CCCD-$TS\",\"fullName\":\"Dup CCCD\",\"cccd\":\"$CCCD\",\"orgUnitCode\":\"ORG-HQ\"}" 201
expect_http POST "$BASE/v1/emp/employees" "$HR_TOKEN" \
  "{\"employeeCode\":\"MNV-CCCD2-$TS\",\"fullName\":\"Dup CCCD2\",\"cccd\":\"$CCCD\",\"orgUnitCode\":\"ORG-HQ\"}" 409

echo "========== EMP-TC-003 — trùng email / OK trống email =========="
expect_http POST "$BASE/v1/emp/employees" "$HR_TOKEN" \
  '{"employeeCode":"MNV-EMAIL-DUP-'"$TS"'","fullName":"Dup Email","emailCty":"dev@company.local","orgUnitCode":"ORG-HQ"}' 409
expect_http POST "$BASE/v1/emp/employees" "$HR_TOKEN" \
  '{"employeeCode":"MNV-NOEMAIL-'"$TS"'","fullName":"No Email","orgUnitCode":"ORG-HQ"}' 201

echo "========== EMP-TC-004 — org không hiệu lực =========="
expect_http POST "$BASE/v1/emp/employees" "$HR_TOKEN" \
  '{"employeeCode":"MNV-BADORG-'"$TS"'","fullName":"Bad Org","orgUnitCode":"ORG-INACTIVE"}' 400

echo ""
echo "OK — E2E API unhappy (EMP-TC-002/003/004)"
