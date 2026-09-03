import { useEffect, useState } from "react";
import {
  fetchProbationCases,
  fetchProbationReminders,
  runProbationReminders,
} from "../api/client";
import type { ProbationCase, ProbationReminder } from "../api/types";

export function PrbCasesPage() {
  const [items, setItems] = useState<ProbationCase[]>([]);
  const [reminders, setReminders] = useState<ProbationReminder[]>([]);
  const [asOf, setAsOf] = useState("");
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  async function reload() {
    const [cases, rem] = await Promise.all([fetchProbationCases(), fetchProbationReminders()]);
    setItems(cases);
    setReminders(rem);
  }

  useEffect(() => {
    reload()
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  async function onRunJob() {
    setError(null);
    setMessage(null);
    try {
      const result = await runProbationReminders(asOf || undefined);
      setMessage(
        `Job ${result.asOfDate}: T-15 +${result.t15Created}, T-7 +${result.t7Created}` +
          ` (skip thiếu KT ${result.skippedIncompleteMilestone}, đã có ${result.skippedAlreadyExists}).`,
      );
      await reload();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Chạy job thất bại");
    }
  }

  return (
    <div className="card stack">
      <div>
        <h2>Hàng thử việc</h2>
        <p className="muted">
          PRB-SCR-001 — BĐ/KT từ HĐ EMP (FR-001). Job T-15/T-7 ngày lịch (FR-002/003); kênh in-app+email
          (FR-011); không CRM sales.
        </p>
      </div>
      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}

      <div className="row" style={{ gap: 8, flexWrap: "wrap", alignItems: "center" }}>
        <label className="muted">
          AsOf (yyyy-MM-dd){" "}
          <input value={asOf} onChange={(e) => setAsOf(e.target.value)} placeholder="hôm nay UTC" />
        </label>
        <button type="button" className="btn" onClick={onRunJob}>
          Chạy job T-15/T-7
        </button>
      </div>

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

      {reminders.length > 0 && (
        <div className="stack">
          <h3>Nhắc đã tạo</h3>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Loại</th>
                  <th>NV</th>
                  <th>Due</th>
                  <th>Assignee</th>
                  <th>Email</th>
                  <th>Channel</th>
                </tr>
              </thead>
              <tbody>
                {reminders.map((r) => (
                  <tr key={r.id}>
                    <td>{r.kind}</td>
                    <td>{r.employeeCode}</td>
                    <td>{r.dueDate}</td>
                    <td>{r.assigneeEmployeeCode ?? "HR"}</td>
                    <td>{r.emailTo}</td>
                    <td>{r.channel}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}
