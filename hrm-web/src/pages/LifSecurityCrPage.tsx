import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { applyLifOffboardingLocks, fetchLifOffboardings } from "../api/client";
import type { LifOffboarding } from "../api/types";
import { isIt, useCurrentUser } from "../hooks/useCurrentUser";

/** LIF-SCR-006 — Early CR an ninh / khóa trước N+3; không CRM sales. */
export function LifSecurityCrPage() {
  const { caseId = "" } = useParams();
  const user = useCurrentUser();
  const canLock = isIt(user) || user.roles.includes("IAM-ROLE-PGD");
  const [row, setRow] = useState<LifOffboarding | null>(null);
  const [earlyCrReason, setEarlyCrReason] = useState("");
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

  async function onEarlyCrLock() {
    setError(null);
    setMessage(null);
    try {
      const dto = await applyLifOffboardingLocks(caseId, { earlyCrReason });
      setRow(dto);
      setMessage(
        `Early CR ${dto.employeeCode}: git=${dto.gitLocked ? "yes" : "no"}, crmSp=${dto.crmSpLocked ? "yes" : "no"}.`,
      );
    } catch (err) {
      setError(err instanceof Error ? err.message : "Early CR lock thất bại");
    }
  }

  return (
    <div className="card stack">
      <div>
        <h2>Khóa chat / CR an ninh</h2>
        <p className="muted">
          LIF-SCR-006 — khóa Git/CRM trước N+3 cần lý do CR. Không gửi CRM sales.
        </p>
        <Link className="btn btn-ghost" to="/lif">
          ← Danh sách
        </Link>
      </div>
      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}
      {loading && <p className="muted">Đang tải…</p>}
      {row && (
        <p>
          <strong>{row.employeeCode}</strong> · N {row.lastWorkingDayN ?? "—"} · N+3{" "}
          {row.nPlus3Expected ?? "—"} · Git {row.gitLocked ? "Đã khóa" : "Mở"} · CRM SP{" "}
          {row.crmSpLocked ? "Đã khóa" : "Mở"}
        </p>
      )}
      {canLock ? (
        <div className="row" style={{ gap: 8, flexWrap: "wrap", alignItems: "center" }}>
          <label className="muted">
            Lý do CR an ninh{" "}
            <input
              type="text"
              value={earlyCrReason}
              onChange={(e) => setEarlyCrReason(e.target.value)}
              placeholder="Bắt buộc nếu khóa trước N+3"
              style={{ minWidth: 260 }}
            />
          </label>
          <button
            type="button"
            className="btn"
            onClick={onEarlyCrLock}
            disabled={!earlyCrReason.trim()}
          >
            Early CR lock
          </button>
        </div>
      ) : (
        <p className="muted">Chỉ IT/PGD thực hiện Early CR.</p>
      )}
    </div>
  );
}
