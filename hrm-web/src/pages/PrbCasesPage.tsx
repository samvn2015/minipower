import { useEffect, useState } from "react";
import {
  decideProbationEvaluation,
  fetchProbationCases,
  fetchProbationOutcomes,
  fetchProbationReminders,
  runProbationReminders,
} from "../api/client";
import type { ProbationCase, ProbationMasterItem, ProbationReminder } from "../api/types";

export function PrbCasesPage() {
  const [items, setItems] = useState<ProbationCase[]>([]);
  const [reminders, setReminders] = useState<ProbationReminder[]>([]);
  const [outcomes, setOutcomes] = useState<ProbationMasterItem[]>([]);
  const [asOf, setAsOf] = useState("");
  const [decideEmpId, setDecideEmpId] = useState("");
  const [decideCode, setDecideCode] = useState("PASS");
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  async function reload() {
    const [cases, rem, outs] = await Promise.all([
      fetchProbationCases(),
      fetchProbationReminders(),
      fetchProbationOutcomes(),
    ]);
    setItems(cases);
    setReminders(rem);
    setOutcomes(outs);
    if (!decideEmpId && cases[0]) setDecideEmpId(cases[0].employeeId);
  }

  useEffect(() => {
    reload()
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
    // eslint-disable-next-line react-hooks/exhaustive-deps
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

  async function onDecide() {
    setError(null);
    setMessage(null);
    try {
      const dto = await decideProbationEvaluation(decideEmpId, {
        outcomeCode: decideCode,
        note: "Chốt từ SCR-001",
        extendDurationCode: decideCode === "EXTEND" ? "EXT-1M" : undefined,
      });
      setMessage(`Đã chốt ${dto.employeeCode}: ${dto.decidedOutcomeCode} (SoT HR).`);
      await reload();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Chốt thất bại");
    }
  }

  return (
    <div className="card stack">
      <div>
        <h2>Hàng thử việc</h2>
        <p className="muted">
          PRB-SCR-001/003 — mốc EMP · job T-15/T-7 · HR chốt 3 mã master (FR-004/009).
        </p>
      </div>
      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}

      <div className="row" style={{ gap: 8, flexWrap: "wrap", alignItems: "center" }}>
        <label className="muted">
          AsOf{" "}
          <input value={asOf} onChange={(e) => setAsOf(e.target.value)} placeholder="yyyy-MM-dd" />
        </label>
        <button type="button" className="btn" onClick={onRunJob}>
          Chạy job T-15/T-7
        </button>
      </div>

      <div className="row" style={{ gap: 8, flexWrap: "wrap", alignItems: "center" }}>
        <label className="muted">
          NV{" "}
          <select value={decideEmpId} onChange={(e) => setDecideEmpId(e.target.value)}>
            {items.map((c) => (
              <option key={c.employeeId} value={c.employeeId}>
                {c.employeeCode}
              </option>
            ))}
          </select>
        </label>
        <label className="muted">
          Kết quả{" "}
          <select value={decideCode} onChange={(e) => setDecideCode(e.target.value)}>
            {outcomes.map((o) => (
              <option key={o.code} value={o.code}>
                {o.name}
              </option>
            ))}
          </select>
        </label>
        <button type="button" className="btn" onClick={onDecide} disabled={!decideEmpId}>
          HR Chốt SoT
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
