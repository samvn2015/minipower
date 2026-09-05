import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import {
  decideProbationEvaluation,
  fetchProbationCases,
  fetchProbationExtendDurations,
  fetchProbationOutcomes,
} from "../api/client";
import type { ProbationCase, ProbationMasterItem } from "../api/types";

/** PRB-SCR-003 — HR chốt SoT; LM/NV không vào route này (RequirePrbHr). */
export function PrbDecidePage() {
  const { employeeId = "" } = useParams();
  const [row, setRow] = useState<ProbationCase | null>(null);
  const [outcomes, setOutcomes] = useState<ProbationMasterItem[]>([]);
  const [extendsList, setExtendsList] = useState<ProbationMasterItem[]>([]);
  const [outcomeCode, setOutcomeCode] = useState("PASS");
  const [extendCode, setExtendCode] = useState("EXT-1M");
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([
      fetchProbationCases(),
      fetchProbationOutcomes(),
      fetchProbationExtendDurations(),
    ])
      .then(([cases, outs, exts]) => {
        setOutcomes(outs);
        setExtendsList(exts);
        if (outs[0]) setOutcomeCode(outs[0].code);
        if (exts[0]) setExtendCode(exts[0].code);
        setRow(cases.find((c) => c.employeeId === employeeId) ?? null);
      })
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, [employeeId]);

  async function onDecide() {
    setError(null);
    setMessage(null);
    try {
      const dto = await decideProbationEvaluation(employeeId, {
        outcomeCode,
        note: "Chốt từ SCR-003",
        extendDurationCode: outcomeCode === "EXTEND" ? extendCode : undefined,
      });
      setMessage(`Đã chốt ${dto.employeeCode}: ${dto.decidedOutcomeCode} (SoT HR).`);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Chốt thất bại");
    }
  }

  return (
    <div className="card stack">
      <div>
        <h2>Chốt kết quả thử việc</h2>
        <p className="muted">
          PRB-SCR-003 — HR chọn 3 mã master · Gia hạn = dropdown thời lượng master · [Chốt].
        </p>
        <Link className="btn btn-ghost" to="/prb/cases">
          ← Hàng TV
        </Link>
      </div>
      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}
      {loading && <p className="muted">Đang tải…</p>}
      {!loading && !row && <p className="muted">Không tìm thấy NV trong hàng TV.</p>}
      {row && (
        <>
          <p>
            <strong>{row.employeeCode}</strong> · {row.fullName ?? "—"} · KT{" "}
            {row.probationEndDate ?? "—"}
          </p>
          {outcomeCode === "PASS" && (
            <p className="muted">Đạt: EMP chuyển chính thức — lương theo HĐ/PAY.</p>
          )}
          {outcomeCode === "FAIL" && (
            <p className="muted">Không đạt: mở offboarding LIF (PRB-FAIL).</p>
          )}
          <div className="row" style={{ gap: 8, flexWrap: "wrap", alignItems: "center" }}>
            <label className="muted">
              Kết quả{" "}
              <select value={outcomeCode} onChange={(e) => setOutcomeCode(e.target.value)}>
                {outcomes.map((o) => (
                  <option key={o.code} value={o.code}>
                    {o.name}
                  </option>
                ))}
              </select>
            </label>
            {outcomeCode === "EXTEND" && (
              <label className="muted">
                Thời lượng{" "}
                <select value={extendCode} onChange={(e) => setExtendCode(e.target.value)}>
                  {extendsList.map((x) => (
                    <option key={x.code} value={x.code}>
                      {x.name}
                    </option>
                  ))}
                </select>
              </label>
            )}
            <button type="button" className="btn" onClick={onDecide}>
              HR Chốt SoT
            </button>
          </div>
        </>
      )}
    </div>
  );
}
