import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { fetchIdentityAccounts } from "../api/client";
import type { IdentityAccount } from "../api/types";
import { isHrOrIt, useCurrentUser } from "../hooks/useCurrentUser";

export function IamAccountListPage() {
  const user = useCurrentUser();
  const [items, setItems] = useState<IdentityAccount[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!isHrOrIt(user)) return;
    let active = true;
    fetchIdentityAccounts()
      .then((rows) => {
        if (active) setItems(rows);
      })
      .catch((err: Error) => {
        if (active) setError(err.message);
      })
      .finally(() => {
        if (active) setLoading(false);
      });
    return () => {
      active = false;
    };
  }, [user]);

  if (!isHrOrIt(user)) {
    return (
      <div className="card">
        <h2>Không có quyền</h2>
        <p className="muted">IAM-SCR-003 chỉ dành cho HR hoặc IT (IAM-FR-013).</p>
      </div>
    );
  }

  return (
    <div className="card stack">
      <div>
        <h2>Tài khoản IAM</h2>
        <p className="muted">IAM-SCR-003 — danh sách account, gán/gỡ role trên chi tiết.</p>
      </div>

      {error && <div className="error-box">{error}</div>}

      {loading ? (
        <p className="muted">Đang tải…</p>
      ) : items.length === 0 ? (
        <div className="empty-state">Chưa có tài khoản.</div>
      ) : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Subject</th>
                <th>Tên hiển thị</th>
                <th>Email</th>
                <th>Mã NV</th>
                <th>Roles</th>
                <th>Trạng thái</th>
                <th />
              </tr>
            </thead>
            <tbody>
              {items.map((item) => (
                <tr key={item.id}>
                  <td>{item.idpSubject}</td>
                  <td>{item.displayName ?? "—"}</td>
                  <td>{item.emailCty ?? "—"}</td>
                  <td>{item.employeeCode ?? "—"}</td>
                  <td>{item.roles.join(", ") || "—"}</td>
                  <td>
                    <span
                      className={`badge${item.status !== "Active" ? " badge-warn" : ""}`}
                    >
                      {item.status}
                    </span>
                  </td>
                  <td>
                    <Link to={`/iam/accounts/${item.id}`}>Quản lý</Link>
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
