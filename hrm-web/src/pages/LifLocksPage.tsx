import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { fetchLifOffboardings, runLifNPlus3Locks } from "../api/client";
import type { LifOffboarding } from "../api/types";
import { isHr, isIt, useCurrentUser } from "../hooks/useCurrentUser";

/** LIF-SCR-004 — trạng thái khóa Git/CRM SP; HR read-only; IT chạy job N+3. */
export function LifLocksPage() {
  const { caseId = "" } = useParams();
  const user = useCurrentUser();
  const hr = isHr(user);
  const canJob = isIt(user) || user.roles.includes("IAM-ROLE-PGD");
  const [row, setRow] = useState<LifOffboarding | null>(null);
  const [asOf, setAsOf] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  async function reload() {
    const rows = await fetchLifOffboardings();
    setRow(rows.find((c) => c.id === caseId) ?? null);
  }

  useEffect(() => {
    reload()
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [caseId]);

  async function onRunJob() {
    setError(null);
    setMessage(null);
    try {
      const result = await runLifNPlus3Locks(asOf || undefined);
      setMessage(
        `Job N+3 ${result.asOfDate}: locked +${result.locked}, skipDue ${result.skippedNotDue}, already ${result.skippedAlreadyLocked}.`,
      );
      await reload();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Job N+3 thất bại");
    }
  }

  return (
    <div className="card stack">
      <div>
        <h2>Trạng thái khóa Git / CRM SP</h2>
        <p className="muted">
          LIF-SCR-004 — N+3 ngày lịch. HR không khóa / không credential Git. Không CRM sales.
        </p>
        <Link className="btn btn-ghost" to="/lif">
          ← Danh sách
        </Link>
      </div>
      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}
      {loading && <p className="muted">Đang tải…</p>}
      {!loading && !row && <p className="muted">Không tìm thấy case.</p>}
      {row && (
        <div className="table-wrap">
          <table>
            <tbody>
              <tr>
                <th>NV</th>
                <td>{row.employeeCode}</td>
              </tr>
              <tr>
                <th>N</th>
                <td>{row.lastWorkingDayN ?? "—"}</td>
              </tr>
              <tr>
                <th>N+3 (dự kiến)</th>
                <td>{row.nPlus3Expected ?? "—"}</td>
              </tr>
              <tr>
                <th>Git</th>
                <td>{row.gitLocked ? "Đã khóa" : "Mở"}</td>
              </tr>
              <tr>
                <th>CRM SP</th>
                <td>{row.crmSpLocked ? "Đã khóa" : "Mở"}</td>
              </tr>
              <tr>
                <th>Job eligible</th>
                <td>{row.jobNPlus3Eligible ? "Yes" : "No"}</td>
              </tr>
            </tbody>
          </table>
        </div>
      )}
      {hr && !canJob && (
        <p className="muted">HR: chỉ xem — tạo ticket IT nếu cần khóa (không nút SSH/khóa).</p>
      )}
      {canJob && (
        <div className="row" style={{ gap: 8, flexWrap: "wrap", alignItems: "center" }}>
          <label className="muted">
            AsOf{" "}
            <input value={asOf} onChange={(e) => setAsOf(e.target.value)} placeholder="yyyy-MM-dd" />
          </label>
          <button type="button" className="btn" onClick={onRunJob}>
            Chạy job N+3 locks
          </button>
          <Link className="btn btn-ghost" to={`/lif/offboarding/${caseId}/security`}>
            Early CR (SCR-006)
          </Link>
        </div>
      )}
    </div>
  );
}
