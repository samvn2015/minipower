import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { fetchProbationCases } from "../api/client";
import type { ProbationCase } from "../api/types";

/** PRB-SCR-004 — cảnh báo thiếu BĐ/KT; chỉ link EMP, cấm date picker ảo. */
export function PrbIncompletePage() {
  const [items, setItems] = useState<ProbationCase[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchProbationCases()
      .then((cases) => setItems(cases.filter((c) => !c.hasCompleteMilestone)))
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div className="card stack">
      <div>
        <h2>Thiếu mốc hợp đồng TV</h2>
        <p className="muted">
          PRB-SCR-004 — cảnh báo + mở hồ sơ EMP. Không date picker KT ảo trên PRB.
        </p>
        <Link className="btn btn-ghost" to="/prb/cases">
          ← Hàng TV
        </Link>
      </div>
      {error && <div className="error-box">{error}</div>}
      {loading && <p className="muted">Đang tải…</p>}
      {!loading && items.length === 0 && (
        <p className="muted">Không có NV TV thiếu mốc KT.</p>
      )}
      {items.length > 0 && (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Mã NV</th>
                <th>Họ tên</th>
                <th>BĐ TV</th>
                <th>KT TV</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {items.map((r) => (
                <tr key={r.employeeId}>
                  <td>{r.employeeCode}</td>
                  <td>{r.fullName ?? "—"}</td>
                  <td>{r.probationStartDate}</td>
                  <td>{r.probationEndDate ?? "— thiếu —"}</td>
                  <td>
                    <Link className="btn btn-ghost" to={`/employees/${r.employeeId}`}>
                      Mở hồ sơ HĐ EMP
                    </Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
