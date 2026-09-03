import { FormEvent, useEffect, useState } from "react";
import {
  fetchPayAllowanceCatalog,
  fetchPayMonthlyAllowances,
  upsertPayMonthlyAllowance,
} from "../api/client";
import type { PayAllowanceCatalogItem, PayMonthlyAllowance } from "../api/types";

export function PayAllowancePage() {
  const [periodYm, setPeriodYm] = useState("2027-12");
  const [employeeCode, setEmployeeCode] = useState("MNV-DEV");
  const [code, setCode] = useState("PC-XANG");
  const [amount, setAmount] = useState("200000");
  const [catalog, setCatalog] = useState<PayAllowanceCatalogItem[]>([]);
  const [rows, setRows] = useState<PayMonthlyAllowance[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function load(ym: string) {
    const [cat, list] = await Promise.all([
      fetchPayAllowanceCatalog(),
      fetchPayMonthlyAllowances(ym).catch(() => []),
    ]);
    setCatalog(cat.filter((c) => c.isActive));
    setRows(list);
  }

  useEffect(() => {
    load(periodYm).catch((err: Error) => setError(err.message));
  }, [periodYm]);

  async function onSave(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    setError(null);
    setMessage(null);
    try {
      const result = await upsertPayMonthlyAllowance({
        periodYm,
        employeeCode: employeeCode.trim(),
        code,
        amount: Number(amount),
      });
      setMessage(`Đã lưu ${result.employeeCode} · ${result.code} = ${result.amount}.`);
      await load(periodYm);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Lưu PC tháng thất bại");
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="card stack">
      <div>
        <h2>PC / thưởng tháng</h2>
        <p className="muted">PAY-SCR-004 — mã phải thuộc master kỳ (PAY-FR-015). PC HĐ không nhập tại đây.</p>
      </div>

      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}

      <form className="row" style={{ gap: 12, alignItems: "end", flexWrap: "wrap" }} onSubmit={onSave}>
        <label>
          Kỳ (YYYY-MM)
          <input required value={periodYm} onChange={(e) => setPeriodYm(e.target.value)} />
        </label>
        <label>
          MNV
          <input required value={employeeCode} onChange={(e) => setEmployeeCode(e.target.value)} />
        </label>
        <label>
          Mã master
          <select value={code} onChange={(e) => setCode(e.target.value)}>
            {catalog.map((c) => (
              <option key={c.code} value={c.code}>
                {c.code} — {c.name}
              </option>
            ))}
          </select>
        </label>
        <label>
          Số tiền
          <input required type="number" min={0} step="1" value={amount} onChange={(e) => setAmount(e.target.value)} />
        </label>
        <button type="submit" className="btn" disabled={busy}>
          Lưu dòng tháng
        </button>
      </form>

      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>MNV</th>
              <th>Mã</th>
              <th>Số tiền</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((r) => (
              <tr key={r.id}>
                <td>{r.employeeCode}</td>
                <td>{r.code}</td>
                <td>{r.amount}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
