import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { fetchTimesheetPeriods } from "../api/client";
import type { TimesheetPeriod } from "../api/types";

/** TIM-SCR-001 — danh sách tháng công; điều hướng SCR-002…006. */
export function TimListPage() {
  const [periods, setPeriods] = useState<TimesheetPeriod[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchTimesheetPeriods()
      .then(setPeriods)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div className="card stack">
      <div>
        <h2>Danh sách tháng công</h2>
        <p className="muted">TIM-SCR-001 — Draft / Chốt. Ẩn NV/LM (route RequireHr).</p>
        <div className="row" style={{ gap: 8, flexWrap: "wrap" }}>
          <Link className="btn btn-ghost" to="/tim/templates">
            Mẫu (SCR-002)
          </Link>
          <Link className="btn btn-ghost" to="/tim/imports">
            Import (SCR-003)
          </Link>
          <Link className="btn btn-ghost" to="/tim/close">
            Chốt (SCR-005)
          </Link>
          <Link className="btn btn-ghost" to="/tim/unlock">
            Bỏ chốt (SCR-006)
          </Link>
        </div>
      </div>
      {error && <div className="error-box">{error}</div>}
      {loading && <p className="muted">Đang tải…</p>}
      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>Kỳ</th>
              <th>Trạng thái</th>
              <th>Số dòng</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {periods.length === 0 && !loading && (
              <tr>
                <td colSpan={4} className="muted">
                  Chưa có kỳ — import + commit (SCR-003/004).
                </td>
              </tr>
            )}
            {periods.map((p) => (
              <tr key={p.id}>
                <td>{p.periodYm}</td>
                <td>{p.status}</td>
                <td>{p.lineCount}</td>
                <td className="row" style={{ gap: 6, flexWrap: "wrap" }}>
                  {p.status === "Draft" && (
                    <Link className="btn btn-ghost" to="/tim/close">
                      Chốt
                    </Link>
                  )}
                  {p.status === "Closed" && (
                    <Link className="btn btn-ghost" to="/tim/unlock">
                      Bỏ chốt
                    </Link>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
