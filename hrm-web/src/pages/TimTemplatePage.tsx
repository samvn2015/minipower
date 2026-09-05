import { FormEvent, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
  createTimesheetTemplate,
  fetchActiveTimesheetTemplate,
  fetchTimesheetTemplates,
  publishTimesheetTemplate,
} from "../api/client";
import type { TimesheetTemplate } from "../api/types";

const DEFAULT_COLUMNS = [
  { columnKey: "mnv", displayName: "Mã NV", sortOrder: 1, isRequired: true, mapsTo: "EmployeeCode" },
  { columnKey: "n_thuc", displayName: "Ngày công thực", sortOrder: 2, isRequired: true, mapsTo: "WorkDays" },
  { columnKey: "ot_15", displayName: "OT 1.5", sortOrder: 3, isRequired: false, mapsTo: "Ot15" },
  { columnKey: "ot_20", displayName: "OT 2.0", sortOrder: 4, isRequired: false, mapsTo: "Ot20" },
  { columnKey: "ot_30", displayName: "OT 3.0", sortOrder: 5, isRequired: false, mapsTo: "Ot30" },
];

export function TimTemplatePage() {
  const [active, setActive] = useState<TimesheetTemplate | null>(null);
  const [items, setItems] = useState<TimesheetTemplate[]>([]);
  const [versionCode, setVersionCode] = useState("");
  const [name, setName] = useState("");
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);

  async function reload() {
    setLoading(true);
    setError(null);
    try {
      const [a, list] = await Promise.all([
        fetchActiveTimesheetTemplate().catch(() => null),
        fetchTimesheetTemplates(),
      ]);
      setActive(a);
      setItems(list);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Tải mẫu TIM thất bại");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void reload();
  }, []);

  async function onCreate(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    setError(null);
    setMessage(null);
    try {
      const created = await createTimesheetTemplate({
        versionCode: versionCode.trim(),
        name: name.trim(),
        columns: DEFAULT_COLUMNS,
      });
      setMessage(`Đã tạo Draft ${created.versionCode}.`);
      setVersionCode("");
      setName("");
      await reload();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Tạo mẫu thất bại");
    } finally {
      setBusy(false);
    }
  }

  async function onPublish(id: string) {
    setBusy(true);
    setError(null);
    setMessage(null);
    try {
      const result = await publishTimesheetTemplate(id);
      setMessage(`Đã công bố ${result.versionCode} — mẫu cũ Retired (TIM-FR-001).`);
      await reload();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Công bố thất bại");
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="card stack">
      <div>
        <h2>Công bố mẫu chấm công</h2>
        <p className="muted">TIM-SCR-002 — một version Active; cột lấy từ master (không hardcode URD).</p>
        <div className="row" style={{ gap: 8, flexWrap: "wrap" }}>
          <Link className="btn btn-ghost" to="/tim">
            ← Danh sách
          </Link>
          <Link className="btn btn-secondary" to="/tim/imports">
            Import công →
          </Link>
        </div>
      </div>

      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}

      {loading ? (
        <p className="muted">Đang tải…</p>
      ) : (
        <>
          {active && (
            <div className="card stack" style={{ background: "var(--surface-2)" }}>
              <h3>
                Đang hiệu lực: <strong>{active.versionCode}</strong> — {active.name}
              </h3>
              <div className="table-wrap">
                <table>
                  <thead>
                    <tr>
                      <th>#</th>
                      <th>Key</th>
                      <th>Tên cột</th>
                      <th>MapsTo</th>
                      <th>Bắt buộc</th>
                    </tr>
                  </thead>
                  <tbody>
                    {active.columns.map((c) => (
                      <tr key={c.columnKey}>
                        <td>{c.sortOrder}</td>
                        <td>{c.columnKey}</td>
                        <td>{c.displayName}</td>
                        <td>{c.mapsTo}</td>
                        <td>{c.isRequired ? "Có" : "Không"}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          )}

          <form className="stack" onSubmit={onCreate}>
            <h3>Tạo mẫu Draft mới</h3>
            <label>
              Version code
              <input required value={versionCode} onChange={(e) => setVersionCode(e.target.value)} placeholder="TIM-V2" />
            </label>
            <label>
              Tên
              <input required value={name} onChange={(e) => setName(e.target.value)} placeholder="Mẫu công V2" />
            </label>
            <p className="muted">Cột mặc định copy từ master seed (có thể chỉnh API sau).</p>
            <button type="submit" className="btn" disabled={busy}>
              Tạo Draft
            </button>
          </form>

          <div className="stack">
            <h3>Danh sách version</h3>
            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>Code</th>
                    <th>Tên</th>
                    <th>Status</th>
                    <th>Cột</th>
                    <th />
                  </tr>
                </thead>
                <tbody>
                  {items.map((item) => (
                    <tr key={item.id}>
                      <td>{item.versionCode}</td>
                      <td>{item.name}</td>
                      <td>{item.status}</td>
                      <td>{item.columns.length}</td>
                      <td>
                        {item.status === "Draft" && (
                          <button
                            type="button"
                            className="btn"
                            disabled={busy}
                            onClick={() => onPublish(item.id)}
                          >
                            Công bố
                          </button>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
