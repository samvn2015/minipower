import { useEffect, useState } from "react";
import { fetchProbationCases } from "../api/client";
import type { ProbationCase } from "../api/types";

export function PrbCasesPage() {
  const [items, setItems] = useState<ProbationCase[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchProbationCases()
      .then(setItems)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div className="card stack">
      <div>
        <h2>Hàng thử việc</h2>
        <p className="muted">
          PRB-SCR-001 — BĐ/KT lấy từ HĐ EMP (PRB-FR-001); không gán ngày mặc định.
        </p>
      </div>
      {error && <div className="error-box">{error}</div>}
      {loading && <p className="muted">Đang tải…</p>}
      {!loading && items.length === 0 && <p className="muted">Không có NV đang TV (Active).</p>}
      {items.length > 0 && (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Mã NV</th>
                <th>Họ tên</th>
                <th>BĐ TV</th>
                <th>KT TV</th>
                <th>T-15</th>
                <th>T-7</th>
                <th>Mốc đủ?</th>
              </tr>
            </thead>
            <tbody>
              {items.map((r) => (
                <tr key={r.employeeId}>
                  <td>{r.employeeCode}</td>
                  <td>{r.fullName ?? "—"}</td>
                  <td>{r.probationStartDate}</td>
                  <td>{r.probationEndDate ?? "— thiếu EndDate HĐ —"}</td>
                  <td>{r.t15DueDate ?? "—"}</td>
                  <td>{r.t7DueDate ?? "—"}</td>
                  <td>{r.hasCompleteMilestone ? "Có" : "Thiếu KT"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
