import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
  approveLeaveRequestC2,
  fetchPendingLeaveRequestsC2,
  rejectLeaveRequestC2,
} from "../api/client";
import type { LeaveRequestPendingC1Item } from "../api/types";

export function LeaveC2QueuePage() {
  const [items, setItems] = useState<LeaveRequestPendingC1Item[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [actingId, setActingId] = useState<string | null>(null);

  async function reload() {
    setLoading(true);
    setError(null);
    try {
      setItems(await fetchPendingLeaveRequestsC2());
    } catch (err) {
      setError(err instanceof Error ? err.message : "Tải hàng chờ C2 thất bại");
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
      await approveLeaveRequestC2(id);
      await reload();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Duyệt C2 thất bại");
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
      await rejectLeaveRequestC2(id, note || undefined);
      await reload();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Từ chối C2 thất bại");
    } finally {
      setActingId(null);
    }
  }

  return (
    <div className="card stack">
      <div className="row" style={{ justifyContent: "space-between" }}>
        <div>
          <h2>Duyệt nghỉ phép C2</h2>
          <p className="muted">LEV-SCR-006 — HR duyệt đơn chờ C2, trừ quỹ phép năm.</p>
        </div>
        <Link className="btn btn-secondary" to="/leave">
          ← Nghỉ phép
        </Link>
      </div>

      {error && <div className="error-box">{error}</div>}

      {loading ? (
        <p className="muted">Đang tải…</p>
      ) : items.length === 0 ? (
        <div className="empty-state">Không có đơn chờ duyệt C2.</div>
      ) : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Nhân viên</th>
                <th>Loại / Ngày</th>
                <th>Lý do</th>
                <th>Gửi lúc</th>
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
                    {item.leaveTypeName ?? item.leaveTypeCode}
                    <br />
                    <span className="muted">
                      {item.fromDate} — {item.toDate} ({item.totalDays} ngày)
                    </span>
                  </td>
                  <td>{item.reason}</td>
                  <td>{new Date(item.submittedAtUtc).toLocaleString("vi-VN")}</td>
                  <td>
                    <div className="row">
                      <button
                        type="button"
                        className="btn"
                        disabled={actingId !== null}
                        onClick={() => approve(item.id)}
                      >
                        {actingId === item.id ? "…" : "Duyệt C2"}
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
