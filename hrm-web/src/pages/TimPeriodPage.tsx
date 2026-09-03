import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { closeTimesheetPeriod, fetchTimesheetPeriods, unlockTimesheetPeriod } from "../api/client";
import type { TimesheetPeriod } from "../api/types";

export function TimPeriodPage() {
  const [periods, setPeriods] = useState<TimesheetPeriod[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function load() {
    const rows = await fetchTimesheetPeriods();
    setPeriods(rows);
  }

  useEffect(() => {
    load().catch((err: Error) => setError(err.message));
  }, []);

  async function onClose(ym: string) {
    setBusy(true);
    setError(null);
    setMessage(null);
    try {
      const result = await closeTimesheetPeriod(ym);
      setMessage(
        `Đã chốt kỳ ${result.periodYm} (${result.lineCount} dòng; phép hưởng ${result.totalLeaveDaysPaid}).`,
      );
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Chốt thất bại");
    } finally {
      setBusy(false);
    }
  }

  async function onUnlock(ym: string) {
    setBusy(true);
    setError(null);
    setMessage(null);
    try {
      const result = await unlockTimesheetPeriod(ym);
      setMessage(`Đã bỏ chốt kỳ ${result.periodYm} → Draft (TIM-SCR-006). Có thể import lại.`);
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Bỏ chốt thất bại");
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="card stack">
      <div className="row" style={{ justifyContent: "space-between" }}>
        <div>
          <h2>Chốt / bỏ chốt tháng công</h2>
          <p className="muted">
            TIM-SCR-005/006 — chốt Draft; bỏ chốt khi PAY chưa chốt; merge phép Đã duyệt (TIM-FR-006…012).
          </p>
        </div>
        <Link className="btn btn-secondary" to="/tim/imports">
          ← Import công
        </Link>
      </div>

      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}

      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>Kỳ</th>
              <th>Trạng thái</th>
              <th>Số dòng</th>
              <th>OT chưa loại</th>
              <th>Phép hưởng</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {periods.length === 0 && (
              <tr>
                <td colSpan={6} className="muted">
                  Chưa có kỳ công — import + commit trước.
                </td>
              </tr>
            )}
            {periods.map((p) => {
              const unclassified = p.lines.reduce((s, l) => s + Number(l.otUnclassified || 0), 0);
              const leavePaid = p.lines.reduce((s, l) => s + Number(l.leaveDaysPaid || 0), 0);
              return (
                <tr key={p.id}>
                  <td>{p.periodYm}</td>
                  <td>{p.status}</td>
                  <td>{p.lineCount}</td>
                  <td>{unclassified}</td>
                  <td>{leavePaid}</td>
                  <td className="row" style={{ gap: 8 }}>
                    {p.status === "Draft" && (
                      <button
                        type="button"
                        className="btn"
                        disabled={busy || unclassified > 0}
                        title={unclassified > 0 ? "Còn OT chưa phân loại" : "Chốt tháng"}
                        onClick={() => onClose(p.periodYm)}
                      >
                        Chốt
                      </button>
                    )}
                    {p.status === "Closed" && (
                      <button
                        type="button"
                        className="btn btn-secondary"
                        disabled={busy}
                        title="Bỏ chốt — cấm nếu PAY đã chốt"
                        onClick={() => onUnlock(p.periodYm)}
                      >
                        Bỏ chốt
                      </button>
                    )}
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </div>
  );
}
