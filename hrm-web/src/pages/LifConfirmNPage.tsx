import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { confirmLifOffboardingN, fetchLifOffboardings } from "../api/client";
import type { LifOffboarding } from "../api/types";
import { isHr, useCurrentUser } from "../hooks/useCurrentUser";

/** LIF-SCR-003 — xác nhận ngày làm việc cuối (N); cấm nhầm ngày ký đơn. */
export function LifConfirmNPage() {
  const { caseId = "" } = useParams();
  const hr = isHr(useCurrentUser());
  const [row, setRow] = useState<LifOffboarding | null>(null);
  const [n, setN] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchLifOffboardings()
      .then((rows) => {
        const hit = rows.find((c) => c.id === caseId) ?? null;
        setRow(hit);
        if (hit?.lastWorkingDayN) setN(hit.lastWorkingDayN);
      })
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, [caseId]);

  async function onConfirm() {
    setError(null);
    setMessage(null);
    try {
      const dto = await confirmLifOffboardingN(caseId, n);
      setRow(dto);
      setMessage(
        `Đã xác nhận N=${dto.lastWorkingDayN} · N+3=${dto.nPlus3Expected} (ngày lịch).`,
      );
    } catch (err) {
      setError(err instanceof Error ? err.message : "Xác nhận N thất bại");
    }
  }

  return (
    <div className="card stack">
      <div>
        <h2>Xác nhận ngày LV cuối (N)</h2>
        <p className="muted">
          LIF-SCR-003 — chỉ ngày làm việc cuối. Không dùng ngày ký đơn = N.
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
        <>
          <p>
            <strong>{row.employeeCode}</strong> · {row.status} · N hiện tại{" "}
            {row.lastWorkingDayN ?? "—"} · N+3 {row.nPlus3Expected ?? "—"}
          </p>
          {hr ? (
            <div className="row" style={{ gap: 8, flexWrap: "wrap", alignItems: "center" }}>
              <label className="muted">
                Ngày làm việc cuối (N){" "}
                <input type="date" value={n} onChange={(e) => setN(e.target.value)} />
              </label>
              <button type="button" className="btn" onClick={onConfirm} disabled={!n}>
                HR xác nhận N
              </button>
            </div>
          ) : (
            <p className="muted">Chỉ HR/C&B ghi N — xem read-only.</p>
          )}
        </>
      )}
    </div>
  );
}
