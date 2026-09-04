import { FormEvent, useEffect, useState } from "react";
import {
  fetchPayrollPeriod,
  runPayrollPeriod,
  closePayrollPeriod,
  exportPayrollPeriod,
} from "../api/client";
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

  async function onExport(includeEmail: boolean) {
    setBusy(true);
    setError(null);
    setMessage(null);
    try {
      const result = await exportPayrollPeriod(periodYm, {
        includePdf: true,
        includeEmail,
      });
      setMessage(
        `Xuất ${result.periodYm}: ${result.pdfCount} PDF` +
          (includeEmail ? `, ${result.emailCount} email (outbox)` : "") +
          ".",
      );
      const first = result.items.find((i) => i.pdfBase64 && i.pdfFileName);
      if (first?.pdfBase64 && first.pdfFileName) {
        const bin = atob(first.pdfBase64);
        const bytes = new Uint8Array(bin.length);
        for (let i = 0; i < bin.length; i++) bytes[i] = bin.charCodeAt(i);
        const blob = new Blob([bytes], { type: "application/pdf" });
        const url = URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = first.pdfFileName;
        a.click();
        URL.revokeObjectURL(url);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "Xuất thất bại");
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="card stack">
      <div>
        <h2>Tính kỳ lương</h2>
        <p className="muted">
          PAY-SCR-002/003/007 — TIM đã chốt; bảng công chỉ đọc (sửa trên TIM rồi chạy lại). Xuất PDF/email khi Closed.
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
        {period && period.status === "Closed" && (
          <>
            <button
              type="button"
              className="btn btn-secondary"
              disabled={busy}
              onClick={() => onExport(false)}
            >
              Xuất PDF
            </button>
            <button
              type="button"
              className="btn btn-secondary"
              disabled={busy}
              onClick={() => onExport(true)}
            >
              Xuất PDF + email
            </button>
          </>
        )}
      </form>

      {period && (
        <div className="stack">
          <p>
            Kỳ <strong>{period.periodYm}</strong> · {period.status} · {period.lineCount} dòng · chuẩn{" "}
            {period.standardWorkDays} ngày
          </p>
          <div className="muted" style={{ fontSize: 13 }}>
            PAY-FR-008: không sửa N_thực / OT / phép trên màn này — chỉnh TIM rồi tính/chốt lại.
          </div>
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
                  <th>PC HĐ</th>
                  <th>PC tháng</th>
                  <th>BH</th>
                  <th>TNCN tạm</th>
                  <th>Thực lĩnh</th>
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
                    <td>{l.contractAllowance}</td>
                    <td>{l.monthlyAllowance}</td>
                    <td>{l.bhAmount}</td>
                    <td>{l.tncnAmount}</td>
                    <td>{l.netPay}</td>
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
