import { FormEvent, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
  commitTimesheetImport,
  fetchActiveTimesheetTemplate,
  previewTimesheetImport,
} from "../api/client";
import type { TimesheetImportBatch, TimesheetTemplate } from "../api/types";

export function TimImportPage() {
  const [active, setActive] = useState<TimesheetTemplate | null>(null);
  const [periodYm, setPeriodYm] = useState("2026-10");
  const [csvText, setCsvText] = useState("mnv,n_thuc,ot_15,ot_20,ot_30\nMNV-DEV,22,0,0,0\n");
  const [batch, setBatch] = useState<TimesheetImportBatch | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    fetchActiveTimesheetTemplate()
      .then(setActive)
      .catch((err: Error) => setError(err.message));
  }, []);

  function parseCsv(text: string) {
    const lines = text
      .split(/\r?\n/)
      .map((l) => l.trim())
      .filter(Boolean);
    if (lines.length < 2) throw new Error("CSV cần header + ít nhất 1 dòng.");
    const headers = lines[0].split(",").map((h) => h.trim().toLowerCase());
    const idx = (key: string) => headers.indexOf(key);
    const mnv = idx("mnv");
    const nThuc = idx("n_thuc");
    if (mnv < 0 || nThuc < 0) throw new Error("CSV cần cột mnv và n_thuc (đúng ColumnKey master).");

    return lines.slice(1).map((line, i) => {
      const cols = line.split(",").map((c) => c.trim());
      const num = (i: number) => {
        if (i < 0 || !cols[i]) return undefined;
        const n = Number(cols[i]);
        return Number.isFinite(n) ? n : undefined;
      };
      return {
        rowNumber: i + 1,
        employeeCode: cols[mnv] || undefined,
        workDays: num(nThuc),
        ot15: num(idx("ot_15")),
        ot20: num(idx("ot_20")),
        ot30: num(idx("ot_30")),
      };
    });
  }

  async function onPreview(event: FormEvent) {
    event.preventDefault();
    if (!active) return;
    setBusy(true);
    setError(null);
    setMessage(null);
    try {
      const rows = parseCsv(csvText);
      const result = await previewTimesheetImport({
        periodYm,
        templateVersionCode: active.versionCode,
        fileName: "paste.csv",
        rows,
      });
      setBatch(result);
      setMessage(
        result.hasMustErrors
          ? `Preview có ${result.errorRows} lỗi Must — chưa được commit.`
          : `Preview sạch (${result.totalRows} dòng).`,
      );
    } catch (err) {
      setError(err instanceof Error ? err.message : "Preview thất bại");
    } finally {
      setBusy(false);
    }
  }

  async function onCommit() {
    if (!batch) return;
    setBusy(true);
    setError(null);
    setMessage(null);
    try {
      const result = await commitTimesheetImport(batch.id);
      setMessage(`Đã ghi bảng công Draft ${result.periodYm} — ${result.lineCount} dòng.`);
      setBatch(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Commit thất bại");
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="card stack">
      <div className="row" style={{ justifyContent: "space-between" }}>
        <div>
          <h2>Import + preview công</h2>
          <p className="muted">TIM-SCR-003/004 — khớp version mẫu, cấm commit khi còn lỗi Must.</p>
        </div>
        <Link className="btn btn-secondary" to="/tim/templates">
          ← Mẫu TIM
        </Link>
      </div>

      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}

      {active && (
        <p>
          Mẫu hiệu lực: <strong>{active.versionCode}</strong>
        </p>
      )}

      <form className="stack" onSubmit={onPreview}>
        <label>
          Kỳ (YYYY-MM)
          <input required value={periodYm} onChange={(e) => setPeriodYm(e.target.value)} />
        </label>
        <label>
          CSV (header = ColumnKey master)
          <textarea required rows={8} value={csvText} onChange={(e) => setCsvText(e.target.value)} />
        </label>
        <button type="submit" className="btn" disabled={busy || !active}>
          Preview
        </button>
      </form>

      {batch && (
        <div className="stack">
          <h3>
            Batch {batch.id.slice(0, 8)}… — {batch.hasMustErrors ? "Có lỗi Must" : "OK"}
          </h3>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>#</th>
                  <th>MNV</th>
                  <th>Công</th>
                  <th>OK</th>
                  <th>Lỗi</th>
                </tr>
              </thead>
              <tbody>
                {batch.rows.map((r) => (
                  <tr key={r.rowNumber}>
                    <td>{r.rowNumber}</td>
                    <td>{r.employeeCode ?? "—"}</td>
                    <td>{r.workDays ?? "—"}</td>
                    <td>{r.isOk ? "✓" : "✗"}</td>
                    <td>{r.errorMessage ?? ""}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <button
            type="button"
            className="btn"
            disabled={busy || batch.hasMustErrors}
            onClick={() => onCommit()}
          >
            Ghi bảng công (commit)
          </button>
        </div>
      )}
    </div>
  );
}
