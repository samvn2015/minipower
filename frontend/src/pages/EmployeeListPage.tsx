import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { fetchEmployees } from "../api/client";
import type { EmployeeListItem } from "../api/types";

export function EmployeeListPage() {
  const [items, setItems] = useState<EmployeeListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let active = true;
    fetchEmployees()
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
  }, []);

  return (
    <div className="card stack">
      <div className="row" style={{ justifyContent: "space-between" }}>
        <div>
          <h2>Danh sách nhân viên</h2>
          <p className="muted">EMP-SCR-001 — HR/IT xem và mở hồ sơ.</p>
        </div>
        <div className="row">
          <Link className="btn btn-secondary" to="/line-manager-changes">
            Duyệt đổi LM
          </Link>
          <Link className="btn" to="/employees/new">
            + Tạo nhân viên
          </Link>
        </div>
      </div>

      {error && <div className="error-box">{error}</div>}

      {loading ? (
        <p className="muted">Đang tải…</p>
      ) : items.length === 0 ? (
        <div className="empty-state">Chưa có nhân viên nào.</div>
      ) : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>Mã NV</th>
                <th>Họ tên</th>
                <th>Email cty</th>
                <th>Đơn vị</th>
                <th>HĐ</th>
                <th>Trạng thái</th>
                <th />
              </tr>
            </thead>
            <tbody>
              {items.map((item) => (
                <tr key={item.id}>
                  <td>{item.employeeCode}</td>
                  <td>{item.fullName ?? "—"}</td>
                  <td>{item.emailCty ?? "—"}</td>
                  <td>{item.orgUnitCode ?? "—"}</td>
                  <td>
                    {item.hasContract ? (
                      <span className="badge">Có HĐ</span>
                    ) : (
                      <span className="badge badge-warn">Thiếu HĐ</span>
                    )}
                  </td>
                  <td>{item.status}</td>
                  <td>
                    <Link to={`/employees/${item.id}`}>Mở hồ sơ</Link>
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
