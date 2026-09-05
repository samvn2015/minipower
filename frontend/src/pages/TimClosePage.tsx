import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { closeTimesheetPeriod, fetchTimesheetPeriods } from "../api/client";
import type { TimesheetPeriod } from "../api/types";

/** TIM-SCR-005 — chốt tháng Draft; merge phép đã duyệt (API). */
export function TimClosePage() {
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

  const drafts = periods.filter((p) => p.status === "Draft");

  return (
    <div className="card stack">
      <div>
        <h2>Chốt tháng công</h2>
        <p className="muted">
          TIM-SCR-005 — N_thực gồm phép hưởng · cảnh báo OT thiếu loại (disable Chốt).
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
              <th>OT chưa loại</th>
              <th>Phép hưởng</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {drafts.length === 0 && (
              <tr>
                <td colSpan={5} className="muted">
                  Không có kỳ Draft để chốt.
                </td>
              </tr>
            )}
            {drafts.map((p) => {
              const unclassified = p.lines.reduce((s, l) => s + Number(l.otUnclassified || 0), 0);
              const leavePaid = p.lines.reduce((s, l) => s + Number(l.leaveDaysPaid || 0), 0);
              return (
                <tr key={p.id}>
                  <td>{p.periodYm}</td>
                  <td>{p.lineCount}</td>
                  <td>{unclassified}</td>
                  <td>{leavePaid}</td>
                  <td>
                    <button
                      type="button"
                      className="btn"
                      disabled={busy || unclassified > 0}
                      title={unclassified > 0 ? "Còn OT chưa phân loại" : "Chốt tháng"}
                      onClick={() => void onClose(p.periodYm)}
                    >
                      Chốt
                    </button>
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
