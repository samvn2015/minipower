import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { fetchTimesheetPeriods, unlockTimesheetPeriod } from "../api/client";
import type { TimesheetPeriod } from "../api/types";

/** TIM-SCR-006 — bỏ chốt; cấm nếu PAY đã chốt (API). */
export function TimUnlockPage() {
  const [periods, setPeriods] = useState<TimesheetPeriod[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function load() {
    setPeriods(await fetchTimesheetPeriods());
  }

  useEffect(() => {
    load().catch((err: Error) => setError(err.message));
  }, []);

  async function onUnlock(ym: string) {
    setBusy(true);
    setError(null);
    setMessage(null);
    try {
      const result = await unlockTimesheetPeriod(ym);
      setMessage(`Đã bỏ chốt kỳ ${result.periodYm} → Draft. Có thể import lại.`);
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Bỏ chốt thất bại");
    } finally {
      setBusy(false);
    }
  }

  const closed = periods.filter((p) => p.status === "Closed");

  return (
    <div className="card stack">
      <div>
        <h2>Bỏ chốt tháng công</h2>
        <p className="muted">
          TIM-SCR-006 — disable nếu kỳ PAY đã chốt · không form sửa ô trên PAY.
        </p>
        <Link className="btn btn-ghost" to="/tim">
          ← Danh sách
        </Link>
      </div>
      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}
      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>Kỳ</th>
              <th>Số dòng</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {closed.length === 0 && (
              <tr>
                <td colSpan={3} className="muted">
                  Không có kỳ Closed để bỏ chốt.
                </td>
              </tr>
            )}
            {closed.map((p) => (
              <tr key={p.id}>
                <td>{p.periodYm}</td>
                <td>{p.lineCount}</td>
                <td>
                  <button
                    type="button"
                    className="btn btn-secondary"
                    disabled={busy}
                    onClick={() => void onUnlock(p.periodYm)}
                  >
                    Bỏ chốt
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
