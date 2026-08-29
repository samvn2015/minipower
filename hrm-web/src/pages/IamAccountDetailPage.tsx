import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import {
  assignAccountRole,
  disableIdentityAccount,
  fetchIdentityAccount,
  removeAccountRole,
} from "../api/client";
import { IAM_ASSIGNABLE_ROLES, type IdentityAccount } from "../api/types";
import { isHrOrIt, isIt, useCurrentUser } from "../hooks/useCurrentUser";

export function IamAccountDetailPage() {
  const { id } = useParams();
  const user = useCurrentUser();
  const [account, setAccount] = useState<IdentityAccount | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [roleToAdd, setRoleToAdd] = useState("");
  const [acting, setActing] = useState(false);

  async function reload() {
    if (!id) return;
    setLoading(true);
    setError(null);
    try {
      setAccount(await fetchIdentityAccount(id));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Tải tài khoản thất bại");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    if (!isHrOrIt(user) || !id) return;
    void reload();
  }, [user, id]);

  async function onAssignRole() {
    if (!id || !roleToAdd) return;
    setActing(true);
    setError(null);
    try {
      const result = await assignAccountRole(id, roleToAdd);
      setAccount((prev) =>
        prev
          ? { ...prev, roles: result.roles, status: result.status }
          : prev,
      );
      setRoleToAdd("");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Gán role thất bại");
    } finally {
      setActing(false);
    }
  }

  async function onRemoveRole(roleCode: string) {
    if (!id) return;
    if (!window.confirm(`Gỡ role ${roleCode}?`)) return;
    setActing(true);
    setError(null);
    try {
      const result = await removeAccountRole(id, roleCode);
      setAccount((prev) =>
        prev
          ? { ...prev, roles: result.roles, status: result.status }
          : prev,
      );
    } catch (err) {
      setError(err instanceof Error ? err.message : "Gỡ role thất bại");
    } finally {
      setActing(false);
    }
  }

  async function onDisable() {
    if (!id || !account) return;
    if (!window.confirm(`Vô hiệu login cho ${account.idpSubject}?`)) return;
    setActing(true);
    setError(null);
    try {
      const result = await disableIdentityAccount(id);
      setAccount((prev) =>
        prev
          ? { ...prev, roles: result.roles, status: result.status }
          : prev,
      );
    } catch (err) {
      setError(err instanceof Error ? err.message : "Vô hiệu thất bại");
    } finally {
      setActing(false);
    }
  }

  if (!isHrOrIt(user)) {
    return (
      <div className="card">
        <h2>Không có quyền</h2>
        <p className="muted">IAM-SCR-003 chỉ dành cho HR hoặc IT.</p>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="card">
        <p className="muted">Đang tải tài khoản…</p>
      </div>
    );
  }

  if (!account) {
    return (
      <div className="card">
        <div className="error-box">{error ?? "Không tìm thấy tài khoản."}</div>
        <Link to="/iam/accounts">← Danh sách IAM</Link>
      </div>
    );
  }

  const availableRoles = IAM_ASSIGNABLE_ROLES.filter(
    (role) => !account.roles.includes(role),
  );

  return (
    <div className="card stack">
      <div className="row" style={{ justifyContent: "space-between" }}>
        <div>
          <h2>{account.displayName ?? account.idpSubject}</h2>
          <p className="muted">IAM-SCR-003/004 — gán/gỡ role · IT vô hiệu login.</p>
        </div>
        <Link className="btn btn-secondary" to="/iam/accounts">
          ← Danh sách
        </Link>
      </div>

      {error && <div className="error-box">{error}</div>}

      <div className="form-grid">
        <p>
          <strong>Subject:</strong> {account.idpSubject}
        </p>
        <p>
          <strong>Email:</strong> {account.emailCty ?? "—"}
        </p>
        <p>
          <strong>Mã NV:</strong> {account.employeeCode ?? "—"}
        </p>
        <p>
          <strong>Trạng thái:</strong> {account.status}
        </p>
      </div>

      <div className="stack">
        <h3 style={{ margin: 0 }}>Roles hiện tại</h3>
        {account.roles.length === 0 ? (
          <p className="muted">Chưa có role.</p>
        ) : (
          <div className="row">
            {account.roles.map((role) => (
              <span key={role} className="row badge" style={{ gap: "0.35rem" }}>
                {role}
                <button
                  type="button"
                  className="btn btn-secondary"
                  style={{ padding: "0.15rem 0.45rem", fontSize: "0.75rem" }}
                  disabled={acting}
                  onClick={() => void onRemoveRole(role)}
                >
                  Gỡ
                </button>
              </span>
            ))}
          </div>
        )}
      </div>

      <div className="form-grid">
        <h3 style={{ margin: 0 }}>Gán role mới</h3>
        <div className="row">
          <select
            value={roleToAdd}
            onChange={(e) => setRoleToAdd(e.target.value)}
            disabled={acting || availableRoles.length === 0}
          >
            <option value="">— Chọn role —</option>
            {availableRoles.map((role) => (
              <option key={role} value={role}>
                {role}
              </option>
            ))}
          </select>
          <button
            type="button"
            className="btn"
            disabled={!roleToAdd || acting}
            onClick={() => void onAssignRole()}
          >
            Gán role
          </button>
        </div>
      </div>

      {isIt(user) && account.status === "Active" && (
        <div className="stack">
          <h3 style={{ margin: 0 }}>IAM-SCR-004 — Vô hiệu login</h3>
          <p className="muted">Chỉ IT. Không khóa Git/CRM trên màn này.</p>
          <button
            type="button"
            className="btn btn-secondary"
            disabled={acting}
            onClick={() => void onDisable()}
          >
            Vô hiệu tài khoản
          </button>
        </div>
      )}
    </div>
  );
}
