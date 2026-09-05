import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
  approveLineManagerChange,
  fetchPendingLineManagerChanges,
  rejectLineManagerChange,
} from "../api/client";
import type { LineManagerChangeItem } from "../api/types";

export function LineManagerQueuePage() {
  const [items, setItems] = useState<LineManagerChangeItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [actingId, setActingId] = useState<string | null>(null);

  async function reload() {
    setLoading(true);
    setError(null);
    try {
      setItems(await fetchPendingLineManagerChanges());
    } catch (err) {
      setError(err instanceof Error ? err.message : "Tải hàng chờ thất bại");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void reload();
  }, []);

  async function approve(id: string) {
    setActingId(id);
    setError(null);
    try {
      await approveLineManagerChange(id);
      await reload();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Duyệt thất bại");
    } finally {
      setActingId(null);
    }
  }

  async function reject(id: string) {
    const note = window.prompt("Lý do từ chối (tuỳ chọn):");
    if (note === null) return;
    setActingId(id);
    setError(null);
    try {
      await rejectLineManagerChange(id, note || undefined);
      await reload();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Từ chối thất bại");
    } finally {
      setActingId(null);
    }
  }

  return (
    <div className="card stack">
      <div className="row" style={{ justifyContent: "space-between" }}>
        <div>
          <h2>Duyệt đổi Line Manager</h2>
          <p className="muted">EMP-SCR-006 — hàng chờ pending, duyệt hoặc từ chối.</p>
        </div>
        <Link className="btn btn-secondary" to="/employees">
          ← Danh sách NV
        </Link>
      </div>

      {error && <div className="error-box">{error}</div>}

      {loading ? (
        <p className="muted">Đang tải…</p>
      ) : items.length === 0 ? (
        <div className="empty-state">Không có đề xuất đang chờ duyệt.</div>
      ) : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Nhân viên</th>
                <th>LM đề xuất</th>
                <th>Người gửi</th>
                <th>Thời gian</th>
                <th />
              </tr>
            </thead>
            <tbody>
              {items.map((item) => (
                <tr key={item.id}>
                  <td>
                    <strong>{item.employeeCode}</strong>
                    <br />
                    <span className="muted">{item.employeeFullName ?? "—"}</span>
                  </td>
                  <td>
                    <strong>{item.proposedLineManagerCode}</strong>
                    <br />
                    <span className="muted">{item.proposedLineManagerName ?? "—"}</span>
                  </td>
                  <td>{item.requestedByIdpSubject}</td>
                  <td>{new Date(item.requestedAtUtc).toLocaleString("vi-VN")}</td>
                  <td>
                    <div className="row">
                      <button
                        type="button"
                        className="btn"
                        disabled={actingId !== null}
                        onClick={() => approve(item.id)}
                      >
                        {actingId === item.id ? "…" : "Duyệt"}
                      </button>
                      <button
                        type="button"
                        className="btn btn-secondary"
                        disabled={actingId !== null}
                        onClick={() => reject(item.id)}
                      >
                        Từ chối
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
