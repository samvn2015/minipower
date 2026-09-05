import { useState } from "react";
import { Link, useLocation, useNavigate, useParams } from "react-router-dom";
import { commitTimesheetImport } from "../api/client";
import type { TimesheetImportBatch } from "../api/types";

/** TIM-SCR-004 — xác nhận ghi bảng công khi hết lỗi Must. */
export function TimCommitPage() {
  const { batchId = "" } = useParams();
  const location = useLocation();
  const navigate = useNavigate();
  const fromState = (location.state as { batch?: TimesheetImportBatch } | null)?.batch;
  const [batch] = useState<TimesheetImportBatch | null>(
    fromState && fromState.id === batchId ? fromState : null,
  );
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function onCommit() {
    if (!batch) return;
    setBusy(true);
    setError(null);
    setMessage(null);
    try {
      const result = await commitTimesheetImport(batch.id);
      setMessage(`Đã ghi bảng công Draft ${result.periodYm} — ${result.lineCount} dòng.`);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Commit thất bại");
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="card stack">
      <div>
        <h2>Xác nhận ghi bảng công</h2>
        <p className="muted">TIM-SCR-004 — chỉ khi preview hết lỗi Must. Không ô sửa công kiểu PAY.</p>
        <Link className="btn btn-ghost" to="/tim/imports">
          ← Preview (SCR-003)
        </Link>
      </div>
      {error && <div className="error-box">{error}</div>}
      {message && (
        <div className="success-box">
          {message}{" "}
          <Link className="btn btn-ghost" to="/tim/close">
            Chốt tháng →
          </Link>
        </div>
      )}
      {!batch && (
        <p className="muted">
          Thiếu batch preview. Chạy SCR-003 rồi [Ghi] khi sạch.{" "}
          <button type="button" className="btn btn-ghost" onClick={() => navigate("/tim/imports")}>
            Về import
          </button>
        </p>
      )}
      {batch && (
        <>
          <p>
            Batch <strong>{batch.id.slice(0, 8)}…</strong> · kỳ {batch.periodYm} ·{" "}
            {batch.totalRows} dòng · lỗi Must {batch.errorRows}
          </p>
          <button
            type="button"
            className="btn"
            disabled={busy || batch.hasMustErrors || !!message}
            onClick={() => void onCommit()}
          >
            Ghi bảng công (commit)
          </button>
        </>
      )}
    </div>
  );
}
