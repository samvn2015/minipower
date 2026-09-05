import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import {
  fetchProbationCriteria,
  fetchProbationOutcomes,
  proposeProbationEvaluation,
} from "../api/client";
import type { ProbationMasterItem } from "../api/types";

/** PRB-SCR-002 — phiếu T-7 / đề xuất LM (HR khi không LM). Không nút Chốt. */
export function PrbEvaluatePage() {
  const { employeeId = "" } = useParams();
  const [outcomes, setOutcomes] = useState<ProbationMasterItem[]>([]);
  const [criteria, setCriteria] = useState<ProbationMasterItem[]>([]);
  const [outcomeCode, setOutcomeCode] = useState("PASS");
  const [note, setNote] = useState("");
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([fetchProbationOutcomes(), fetchProbationCriteria()])
      .then(([outs, crit]) => {
        setOutcomes(outs);
        setCriteria(crit);
        if (outs[0]) setOutcomeCode(outs[0].code);
      })
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  async function onPropose() {
    setError(null);
    setMessage(null);
    try {
      const dto = await proposeProbationEvaluation(employeeId, {
        outcomeCode,
        note: note || "Đề xuất từ SCR-002",
      });
      setMessage(`Đã lưu đề xuất ${dto.employeeCode}: ${dto.proposedOutcomeCode} (chưa SoT).`);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Lưu đề xuất thất bại");
    }
  }

  return (
    <div className="card stack">
      <div>
        <h2>Phiếu đánh giá T-7</h2>
        <p className="muted">
          PRB-SCR-002 — tiêu chí master · [Lưu đề xuất]. Không [Chốt] (SCR-003).
        </p>
        <Link className="btn btn-ghost" to="/prb/cases">
          ← Hàng TV
        </Link>
      </div>
      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}
      {loading && <p className="muted">Đang tải…</p>}
      {!loading && (
        <>
          <p className="muted">EmployeeId: {employeeId || "—"}</p>
          {criteria.length > 0 && (
            <div className="stack">
              <h3>Tiêu chí (master)</h3>
              <ul>
                {criteria.map((c) => (
                  <li key={c.code}>
                    {c.code} — {c.name}
                  </li>
                ))}
              </ul>
            </div>
          )}
          <div className="row" style={{ gap: 8, flexWrap: "wrap", alignItems: "center" }}>
            <label className="muted">
              Đề xuất{" "}
              <select value={outcomeCode} onChange={(e) => setOutcomeCode(e.target.value)}>
                {outcomes.map((o) => (
                  <option key={o.code} value={o.code}>
                    {o.name}
                  </option>
                ))}
              </select>
            </label>
            <label className="muted">
              Ghi chú{" "}
              <input value={note} onChange={(e) => setNote(e.target.value)} />
            </label>
            <button type="button" className="btn" onClick={onPropose} disabled={!employeeId}>
              Lưu đề xuất
            </button>
          </div>
        </>
      )}
    </div>
  );
}
