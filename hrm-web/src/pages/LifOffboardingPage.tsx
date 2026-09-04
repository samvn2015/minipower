import { useEffect, useState } from "react";
import {
  confirmLifOffboardingN,
  fetchLifOffboardings,
} from "../api/client";
import type { LifOffboarding } from "../api/types";

export function LifOffboardingPage() {
  const [items, setItems] = useState<LifOffboarding[]>([]);
  const [caseId, setCaseId] = useState("");
  const [n, setN] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  async function reload() {
    const rows = await fetchLifOffboardings();
    setItems(rows);
    if (!caseId && rows[0]) setCaseId(rows[0].id);
  }

  useEffect(() => {
    reload()
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  async function onConfirm() {
    setError(null);
    setMessage(null);
    try {
      const dto = await confirmLifOffboardingN(caseId, n);
      setMessage(
        `Đã xác nhận N=${dto.lastWorkingDayN} · N+3=${dto.nPlus3Expected} (job eligible=${dto.jobNPlus3Eligible}).`,
      );
      await reload();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Xác nhận N thất bại");
    }
  }

  return (
    <div className="card stack">
      <div>
        <h2>Offboarding — xác nhận N</h2>
        <p className="muted">
          LIF-SCR-001/003 — N = <strong>ngày LV cuối</strong> (không dùng ngày ký đơn). Hiện N+3 lịch
          (LIF-FR-003/013). Chỉ HR.
        </p>
      </div>
      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}

      <div className="row" style={{ gap: 8, flexWrap: "wrap", alignItems: "center" }}>
        <label className="muted">
          Case{" "}
          <select value={caseId} onChange={(e) => setCaseId(e.target.value)}>
            {items.map((c) => (
              <option key={c.id} value={c.id}>
                {c.employeeCode} · {c.status}
              </option>
            ))}
          </select>
        </label>
        <label className="muted">
          Ngày LV cuối (N){" "}
          <input type="date" value={n} onChange={(e) => setN(e.target.value)} />
        </label>
        <button type="button" className="btn" onClick={onConfirm} disabled={!caseId || !n}>
          HR xác nhận N
        </button>
      </div>

      {loading && <p className="muted">Đang tải…</p>}
      {items.length > 0 && (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>NV</th>
                <th>Source</th>
                <th>Status</th>
                <th>N</th>
                <th>N+3</th>
                <th>Job?</th>
                <th>Ngày ký (tham chiếu)</th>
              </tr>
            </thead>
            <tbody>
              {items.map((r) => (
                <tr key={r.id}>
                  <td>{r.employeeCode}</td>
                  <td>{r.source}</td>
                  <td>{r.status}</td>
                  <td>{r.lastWorkingDayN ?? "—"}</td>
                  <td>{r.nPlus3Expected ?? "—"}</td>
                  <td>{r.jobNPlus3Eligible ? "Eligible" : "No"}</td>
                  <td>{r.resignationSignedDate ?? "—"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
