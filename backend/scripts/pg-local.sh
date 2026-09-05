#!/usr/bin/env bash
# Local PostgreSQL (Postgres.app) — HRM DEV tối thiểu
set -euo pipefail
PGBIN="${HOME}/Applications/Postgres.app/Contents/Versions/latest/bin"
PGDATA="${HOME}/Library/Application Support/Postgres/var-16"
export PATH="${PGBIN}:${PATH}"

cmd="${1:-status}"
case "$cmd" in
  start)
    if "$PGBIN/pg_ctl" -D "$PGDATA" status >/dev/null 2>&1; then
      echo "already running"
    else
      "$PGBIN/pg_ctl" -D "$PGDATA" -l "$PGDATA/server.log" -o "-p 5432" start
    fi
    ;;
  stop)
    "$PGBIN/pg_ctl" -D "$PGDATA" stop -m fast
    ;;
  status)
    if [[ -f "${PGDATA}/postmaster.pid" ]]; then
      pid="$(head -1 "${PGDATA}/postmaster.pid" 2>/dev/null || true)"
      if [[ -n "${pid}" ]] && kill -0 "${pid}" 2>/dev/null; then
        echo "server running (PID ${pid})"
        echo "data: ${PGDATA}"
      else
        echo "stale postmaster.pid (PID ${pid:-?}) — server not alive"
      fi
    else
      echo "no postmaster.pid — cluster not started via this data dir"
    fi
    "$PGBIN/pg_ctl" -D "$PGDATA" status 2>/dev/null || true
    nc -z 127.0.0.1 5432 && echo "port 5432: open" || echo "port 5432: closed"
    ;;
  *)
    echo "Usage: $0 {start|stop|status}"
    exit 1
    ;;
esac
