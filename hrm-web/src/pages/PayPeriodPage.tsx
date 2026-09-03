import { FormEvent, useEffect, useState } from "react";
import { fetchPayrollPeriod, runPayrollPeriod } from "../api/client";
import type { PayPeriod } from "../api/types";

export function PayPeriodPage() {
  const [periodYm, setPeriodYm] = useState("2027-07");
  const [period, setPeriod] = useState<PayPeriod | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function load(ym: string) {
    try {
      const row = await fetchPayrollPeriod(ym);
      setPeriod(row);
    } catch {
      setPeriod(null);
    }
  }

  useEffect(() => {
    load(periodYm).catch(() => setPeriod(null));
  }, [periodYm]);

  async function onRun(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    setError(null);
    setMessage(null);
    try {
      const result = await runPayrollPeriod(periodYm);
      setMessage(`Đã tính kỳ ${result.periodYm} → ${result.status} (${result.lineCount} dòng).`);
      await load(periodYm);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Tính kỳ thất bại");
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="card stack">
      <div>
        <h2>Tính kỳ lương</h2>
        <p className="muted">
          PAY-SCR-002 — chỉ khi TIM đã chốt; N_tính = N_thực − N_KHL (không cộng phép hưởng).
        </p>
      </div>

      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}

      <form className="row" style={{ gap: 12, alignItems: "end" }} onSubmit={onRun}>
        <label>
          Kỳ (YYYY-MM)
          <input required value={periodYm} onChange={(e) => setPeriodYm(e.target.value)} />
        </label>
        <button type="submit" className="btn" disabled={busy}>
          Tính kỳ
        </button>
      </form>

      {period && (
        <div className="stack">
          <p>
            Kỳ <strong>{period.periodYm}</strong> · {period.status} · {period.lineCount} dòng
          </p>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>MNV</th>
                  <th>N_thực</th>
                  <th>N_KHL</th>
                  <th>Phép hưởng</th>
                  <th>N_tính</th>
                  <th>OT 1.5</th>
                  <th>OT 2.0</th>
                  <th>OT 3.0</th>
                </tr>
              </thead>
              <tbody>
                {period.lines.map((l) => (
                  <tr key={l.id}>
                    <td>{l.employeeCode}</td>
                    <td>{l.workDays}</td>
                    <td>{l.leaveDaysUnpaid}</td>
                    <td>{l.leaveDaysPaid}</td>
                    <td>{l.nTinh}</td>
                    <td>{l.ot15}</td>
                    <td>{l.ot20}</td>
                    <td>{l.ot30}</td>
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
