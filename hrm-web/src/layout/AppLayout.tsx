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
            <Link className="btn btn-ghost" to="/employees">
              Nhân viên
            </Link>
            <Link className="btn btn-ghost" to="/line-manager-changes">
              Duyệt LM
            </Link>
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
