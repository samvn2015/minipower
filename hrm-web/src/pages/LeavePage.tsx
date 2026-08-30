import { FormEvent, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
  createLeaveRequest,
  fetchLeaveTypes,
  fetchMyEmployee,
  fetchMyLeaveBalance,
  fetchMyLeaveRequests,
} from "../api/client";
import type { LeaveBalance, LeaveRequestItem, LeaveType } from "../api/types";

export function LeavePage() {
  const year = new Date().getFullYear();
  const [balance, setBalance] = useState<LeaveBalance | null>(null);
  const [types, setTypes] = useState<LeaveType[]>([]);
  const [requests, setRequests] = useState<LeaveRequestItem[]>([]);
  const [leaveTypeCode, setLeaveTypeCode] = useState("LEV-ANNUAL");
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [dayPart, setDayPart] = useState("FullDay");
  const [reason, setReason] = useState("");
  const [handoverEmployeeId, setHandoverEmployeeId] = useState("");
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);

  async function reload() {
    setLoading(true);
    setError(null);
    try {
      const [bal, typeRows, myRequests, profile] = await Promise.all([
        fetchMyLeaveBalance(year),
        fetchLeaveTypes(),
        fetchMyLeaveRequests(),
        fetchMyEmployee(),
      ]);
      setBalance(bal);
      setTypes(typeRows);
      setRequests(myRequests);
      if (profile.lineManagerEmployeeId && !handoverEmployeeId) {
        setHandoverEmployeeId(profile.lineManagerEmployeeId);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "Tải dữ liệu nghỉ phép thất bại");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void reload();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    if (!handoverEmployeeId.trim()) {
      setError("Thiếu người bàn giao.");
      return;
    }
    setSubmitting(true);
    setError(null);
    setMessage(null);
    try {
      const result = await createLeaveRequest({
        leaveTypeCode,
        fromDate,
        toDate: toDate || fromDate,
        dayPart,
        reason: reason.trim(),
        handoverEmployeeId: handoverEmployeeId.trim(),
      });
      setMessage(`Đã gửi đơn — trạng thái ${result.status}, ${result.totalDays} ngày.`);
      setReason("");
      await reload();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Gửi đơn thất bại");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="card stack">
      <div className="row" style={{ justifyContent: "space-between" }}>
        <div>
          <h2>Nghỉ phép</h2>
          <p className="muted">LEV-SCR — quỹ phép, nộp đơn, danh sách đơn của tôi.</p>
        </div>
        <Link className="btn btn-secondary" to="/leave/c1">
          Inbox duyệt C1 →
        </Link>
      </div>

      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}

      {loading ? (
        <p className="muted">Đang tải…</p>
      ) : (
        <>
          {balance && (
            <div className="card stack" style={{ background: "var(--surface-2)" }}>
              <h3>Quỹ phép {balance.year}</h3>
              <p>
                Còn lại: <strong>{balance.remainingDays}</strong> / {balance.entitledDays} ngày
                (đã dùng {balance.usedDays})
              </p>
            </div>
          )}

          <form className="stack" onSubmit={onSubmit}>
            <h3>Nộp đơn xin phép</h3>
            <label>
              Loại phép
              <select value={leaveTypeCode} onChange={(e) => setLeaveTypeCode(e.target.value)}>
                {types.map((t) => (
                  <option key={t.code} value={t.code}>
                    {t.name}
                  </option>
                ))}
              </select>
            </label>
            <div className="row">
              <label>
                Từ ngày
                <input type="date" required value={fromDate} onChange={(e) => setFromDate(e.target.value)} />
              </label>
              <label>
                Đến ngày
                <input type="date" value={toDate} onChange={(e) => setToDate(e.target.value)} />
              </label>
              <label>
                Buổi
                <select value={dayPart} onChange={(e) => setDayPart(e.target.value)}>
                  <option value="FullDay">Cả ngày</option>
                  <option value="Morning">Sáng</option>
                  <option value="Afternoon">Chiều</option>
                </select>
              </label>
            </div>
            <label>
              Lý do
              <textarea required rows={3} value={reason} onChange={(e) => setReason(e.target.value)} />
            </label>
            <label>
              Người bàn giao (Employee Id)
              <input
                required
                value={handoverEmployeeId}
                onChange={(e) => setHandoverEmployeeId(e.target.value)}
                placeholder="UUID người bàn giao"
              />
            </label>
            <button type="submit" className="btn" disabled={submitting}>
              {submitting ? "Đang gửi…" : "Gửi đơn"}
            </button>
          </form>

          <div className="stack">
            <h3>Đơn của tôi</h3>
            {requests.length === 0 ? (
              <div className="empty-state">Chưa có đơn nghỉ phép.</div>
            ) : (
              <div className="table-wrap">
                <table>
                  <thead>
                    <tr>
                      <th>Loại</th>
                      <th>Từ — Đến</th>
                      <th>Ngày</th>
                      <th>Trạng thái</th>
                    </tr>
                  </thead>
                  <tbody>
                    {requests.map((item) => (
                      <tr key={item.id}>
                        <td>{item.leaveTypeName ?? item.leaveTypeCode}</td>
                        <td>
                          {item.fromDate} — {item.toDate}
                        </td>
                        <td>{item.totalDays}</td>
                        <td>{item.status}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        </>
      )}
    </div>
  );
}
