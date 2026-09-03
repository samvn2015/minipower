import { Link, Navigate, Outlet, useNavigate } from "react-router-dom";
import { clearStoredToken, fetchCurrentUser } from "../api/client";
import type { CurrentUser } from "../api/types";
import { useEffect, useState, type ReactNode } from "react";

export function AppLayout() {
  const navigate = useNavigate();
  const [user, setUser] = useState<CurrentUser | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let active = true;
    fetchCurrentUser()
      .then((current) => {
        if (active) setUser(current);
      })
      .catch((err: Error) => {
        if (active) {
          clearStoredToken();
          navigate("/login", { replace: true });
          setError(err.message);
        }
      });
    return () => {
      active = false;
    };
  }, [navigate]);

  function logout() {
    clearStoredToken();
    navigate("/login", { replace: true });
  }

  if (!user) {
    return (
      <div className="app-shell">
        <main className="app-main">
          <div className="card">
            <p className="muted">Đang tải phiên đăng nhập…</p>
            {error && <p className="error-box">{error}</p>}
          </div>
        </main>
      </div>
    );
  }

  return (
    <div className="app-shell">
      <header className="app-header">
        <div className="row">
          <h1>HRM — MVP</h1>
          <nav className="row">
            <Link className="btn btn-ghost" to="/profile">
              Hồ sơ tôi
            </Link>
            <Link className="btn btn-ghost" to="/leave">
              Nghỉ phép
            </Link>
            <Link className="btn btn-ghost" to="/pay/payslips">
              Phiếu lương
            </Link>
            {(user.roles.includes("IAM-ROLE-LM") ||
              user.roles.includes("IAM-ROLE-HR") ||
              user.roles.includes("IAM-ROLE-PGD")) && (
              <Link className="btn btn-ghost" to="/leave/c1">
                Duyệt phép C1
              </Link>
            )}
            {(user.roles.includes("IAM-ROLE-HR") ||
              user.roles.includes("IAM-ROLE-PGD")) && (
              <Link className="btn btn-ghost" to="/leave/c2">
                Duyệt phép C2
              </Link>
            )}
            {(user.roles.includes("IAM-ROLE-HR") ||
              user.roles.includes("IAM-ROLE-IT")) && (
              <>
                <Link className="btn btn-ghost" to="/employees">
                  Nhân viên
                </Link>
                <Link className="btn btn-ghost" to="/tim/templates">
                  Mẫu công TIM
                </Link>
                <Link className="btn btn-ghost" to="/tim/imports">
                  Import công
                </Link>
                <Link className="btn btn-ghost" to="/tim/periods">
                  Chốt công
                </Link>
                {user.roles.includes("IAM-ROLE-HR") && (
                  <>
                    <Link className="btn btn-ghost" to="/pay/periods">
                      Tính lương
                    </Link>
                    <Link className="btn btn-ghost" to="/pay/allowances">
                      PC tháng
                    </Link>
                  </>
                )}
                <Link className="btn btn-ghost" to="/line-manager-changes">
                  Duyệt LM
                </Link>
                <Link className="btn btn-ghost" to="/iam/accounts">
                  IAM
                </Link>
              </>
            )}
          </nav>
        </div>
        <div className="app-header-meta">
          <span>
            {user.name ?? user.sub} · {user.roles.join(", ")}
          </span>
          <button type="button" className="btn btn-ghost" onClick={logout}>
            Đăng xuất
          </button>
        </div>
      </header>
      <main className="app-main">
        <Outlet context={user} />
      </main>
    </div>
  );
}

export function RequireAuth({ children }: { children: ReactNode }) {
  const token = localStorage.getItem("hrm.accessToken");
  if (!token) return <Navigate to="/login" replace />;
  return <>{children}</>;
}
