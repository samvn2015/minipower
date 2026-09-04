import { useEffect, useState } from "react";
import {
  applyLifOffboardingLocks,
  closeLifOffboarding,
  confirmLifOffboardingN,
  fetchLifOffboardingChecklist,
  fetchLifOffboardings,
  upsertLifOffChecklistTick,
} from "../api/client";
import type { LifOffboarding, LifOffChecklistBoard } from "../api/types";

export function LifOffboardingPage() {
  const [items, setItems] = useState<LifOffboarding[]>([]);
  const [board, setBoard] = useState<LifOffChecklistBoard | null>(null);
  const [caseId, setCaseId] = useState("");
  const [n, setN] = useState("");
  const [earlyCrReason, setEarlyCrReason] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  async function reload(selected?: string) {
    const rows = await fetchLifOffboardings();
    setItems(rows);
    const id = selected || caseId || rows[0]?.id || "";
    if (!caseId && rows[0]) setCaseId(rows[0].id);
    if (id) {
      setCaseId(id);
      setBoard(await fetchLifOffboardingChecklist(id));
    } else {
      setBoard(null);
    }
  }

  useEffect(() => {
    reload()
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  async function onSelectCase(id: string) {
    setCaseId(id);
    setError(null);
    try {
      setBoard(await fetchLifOffboardingChecklist(id));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Không tải checklist");
    }
  }

  async function onConfirm() {
    setError(null);
    setMessage(null);
    try {
      const dto = await confirmLifOffboardingN(caseId, n);
      setMessage(
        `Đã xác nhận N=${dto.lastWorkingDayN} · N+3=${dto.nPlus3Expected} (job eligible=${dto.jobNPlus3Eligible}).`,
      );
      await reload(caseId);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Xác nhận N thất bại");
    }
  }

  async function onEarlyCrLock() {
    setError(null);
    setMessage(null);
    try {
      // Early CR an ninh — khóa Git+CRM SP trước N+3; không CRM sales.
      const dto = await applyLifOffboardingLocks(caseId, { earlyCrReason });
      setMessage(
        `Đã khóa early CR cho ${dto.employeeCode} (git=${dto.gitLocked ? "yes" : "no"}, crmSp=${dto.crmSpLocked ? "yes" : "no"}).`,
      );
      await reload(caseId);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Early CR lock thất bại");
    }
  }

  async function onTick(code: string, checked: boolean) {
    setError(null);
    try {
      setBoard(await upsertLifOffChecklistTick(caseId, code, checked));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Tick thất bại");
    }
  }

  async function onClose() {
    setError(null);
    setMessage(null);
    try {
      const dto = await closeLifOffboarding(caseId);
      setMessage(`Đã đóng off ${dto.employeeCode} (${dto.status}).`);
      await reload();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Đóng off thất bại");
    }
  }

  return (
    <div className="card stack">
      <div>
        <h2>Offboarding</h2>
        <p className="muted">
          LIF-SCR-001/003/004/005 — N = ngày LV cuối; checklist Must; khóa Git+CRM SP tại N+3 (IT/job;
          HR không khóa — FR-003/005–009).
        </p>
      </div>
      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}

      <div className="row" style={{ gap: 8, flexWrap: "wrap", alignItems: "center" }}>
        <label className="muted">
          Case{" "}
          <select value={caseId} onChange={(e) => onSelectCase(e.target.value)}>
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
        <button
          type="button"
          className="btn btn-secondary"
          onClick={onClose}
          disabled={!board?.canClose}
        >
          Đóng off
        </button>
      </div>

      <div className="row" style={{ gap: 8, flexWrap: "wrap", alignItems: "center" }}>
        <label className="muted">
          Early CR reason{" "}
          <input
            type="text"
            value={earlyCrReason}
            onChange={(e) => setEarlyCrReason(e.target.value)}
            placeholder="Lý do an ninh (không CRM sales)"
            style={{ minWidth: 240 }}
          />
        </label>
        <button
          type="button"
          className="btn btn-secondary"
          onClick={onEarlyCrLock}
          disabled={!caseId || !earlyCrReason.trim()}
        >
          Early CR lock
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
                <th>Git</th>
                <th>CRM SP</th>
                <th>Job?</th>
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
                  <td>{r.gitLocked ? "Đã khóa" : "Mở"}</td>
                  <td>{r.crmSpLocked ? "Đã khóa" : "Mở"}</td>
                  <td>{r.jobNPlus3Eligible ? "Eligible" : "No"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {board && (
        <div className="stack">
          <h3>Checklist off (SCR-005)</h3>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Tick</th>
                  <th>Mã</th>
                  <th>Hạng mục</th>
                  <th>Must?</th>
                </tr>
              </thead>
              <tbody>
                {board.items.map((i) => (
                  <tr key={i.code}>
                    <td>
                      <input
                        type="checkbox"
                        checked={i.isChecked}
                        onChange={(e) => onTick(i.code, e.target.checked)}
                      />
                    </td>
                    <td>{i.code}</td>
                    <td>{i.name}</td>
                    <td>{i.isMust ? "Must" : "Optional"}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <p className="muted">canClose = {board.canClose ? "true" : "false"}</p>
          <p className="muted">
            SCR-004: HR chỉ xem trạng thái Git/CRM SP — không nút khóa / không credential Git. Job IT:{" "}
            <code>POST /v1/lif/offboarding/jobs/nplus3-locks</code>. Early CR: IT/PGD qua form trên
            (không CRM sales).
          </p>
        </div>
      )}
    </div>
  );
}
