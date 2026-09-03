import { FormEvent, useEffect, useState } from "react";
import { fetchPayrollPeriod, runPayrollPeriod, closePayrollPeriod } from "../api/client";
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

  async function onClose() {
    setBusy(true);
    setError(null);
    setMessage(null);
    try {
      const result = await closePayrollPeriod(periodYm);
      setMessage(`Đã chốt kỳ lương ${result.periodYm}.`);
      await load(periodYm);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Chốt kỳ thất bại");
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="card stack">
      <div>
        <h2>Tính kỳ lương</h2>
        <p className="muted">
          PAY-SCR-002/003 — TIM đã chốt; N_tính ≤ ngày công chuẩn mới được chốt kỳ.
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
        {period && period.status === "Draft" && (
          <button
            type="button"
            className="btn btn-secondary"
            disabled={busy || period.hasNTinhOverCap}
            title={period.hasNTinhOverCap ? "N_tính vượt chuẩn — cấm chốt" : "Chốt kỳ lương"}
            onClick={() => onClose()}
          >
            Chốt kỳ
          </button>
        )}
      </form>

      {period && (
        <div className="stack">
          <p>
            Kỳ <strong>{period.periodYm}</strong> · {period.status} · {period.lineCount} dòng · chuẩn{" "}
            {period.standardWorkDays} ngày
          </p>
          {period.hasNTinhOverCap && (
            <div className="error-box">
              N_tính vượt chuẩn: {period.overCapEmployeeCodes.join(", ")} — cấm chốt (PAY-FR-007).
            </div>
          )}
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>MNV</th>
                  <th>N_thực</th>
                  <th>N_KHL</th>
                  <th>Phép hưởng</th>
                  <th>N_tính</th>
                  <th>Hệ số TV</th>
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
                    <td>{l.timeWageFactor}</td>
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
