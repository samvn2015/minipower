import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import {
  closeLifOffboarding,
  fetchLifOffboardingChecklist,
  fetchLifOffboardings,
  upsertLifOffChecklistTick,
} from "../api/client";
import type { LifOffboarding, LifOffChecklistBoard } from "../api/types";
import { isHr, useCurrentUser } from "../hooks/useCurrentUser";

/** LIF-SCR-005 — checklist off (master); đóng khi đủ Must. */
export function LifOffChecklistPage() {
  const { caseId = "" } = useParams();
  const hr = isHr(useCurrentUser());
  const [row, setRow] = useState<LifOffboarding | null>(null);
  const [board, setBoard] = useState<LifOffChecklistBoard | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  async function reload() {
    const [rows, b] = await Promise.all([
      fetchLifOffboardings(),
      fetchLifOffboardingChecklist(caseId),
    ]);
    setRow(rows.find((c) => c.id === caseId) ?? null);
    setBoard(b);
  }

  useEffect(() => {
    reload()
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [caseId]);

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
        <h2>Checklist offboarding</h2>
        <p className="muted">LIF-SCR-005 — tick master Must · [Đóng off] disabled nếu thiếu.</p>
        <Link className="btn btn-ghost" to="/lif">
          ← Danh sách
        </Link>
      </div>
      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}
      {loading && <p className="muted">Đang tải…</p>}
      {row && (
        <p>
          <strong>{row.employeeCode}</strong> · {row.status}
        </p>
      )}
      {board && (
        <>
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
                        disabled={!hr}
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
          {hr && (
            <button
              type="button"
              className="btn btn-secondary"
              onClick={onClose}
              disabled={!board.canClose}
            >
              Đóng off
            </button>
          )}
        </>
      )}
    </div>
  );
}
