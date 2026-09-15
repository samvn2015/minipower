import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { fetchCurrentUser, fetchDevToken, loginWithPassword } from "../api/client";
import { DEV_PERSONAS } from "../api/types";

export function LoginPage() {
  const navigate = useNavigate();
  const [loadingId, setLoadingId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  async function goHome() {
    const me = await fetchCurrentUser();
    // IT seed không gắn MNV — tránh landing /profile (IAM-FR-017)
    const home = me.roles.includes("IAM-ROLE-IT") && !me.roles.includes("IAM-ROLE-HR")
      ? "/iam/accounts"
      : "/profile";
    navigate(home, { replace: true });
  }

  async function login(personaId: string, sub: string, email?: string) {
    setLoadingId(personaId);
    setError(null);
    try {
      await fetchDevToken(sub, email);
      await goHome();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Đăng nhập thất bại");
    } finally {
      setLoadingId(null);
    }
  }

  async function submitCredentials(event: React.FormEvent) {
    event.preventDefault();
    setLoadingId("credentials");
    setError(null);
    try {
      await loginWithPassword(username, password);
      await goHome();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Sai tài khoản hoặc mật khẩu");
    } finally {
      setLoadingId(null);
    }
  }

  return (
    <div className="app-shell">
      <header className="app-header">
        <h1>HRM — MVP</h1>
      </header>
      <main className="app-main">
        <div className="card stack">
          <div>
            <h2>Đăng nhập (Development)</h2>
            <p className="muted">
              Chọn vai trò để trải nghiệm. Production sẽ dùng Lark SSO (OQ-DLV-001).
              Hiện DEV/UAT dùng JWT local (`/dev/token`) — DEC-DLV-011 tạm bỏ qua JWKS.
            </p>
          </div>

          {error && <div className="error-box">{error}</div>}

          <form className="stack" onSubmit={submitCredentials} aria-label="Đăng nhập tài khoản">
            <label htmlFor="login-username">Tên đăng nhập</label>
            <input
              id="login-username"
              name="username"
              type="text"
              autoComplete="username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />

            <label htmlFor="login-password">Mật khẩu</label>
            <input
              id="login-password"
              name="password"
              type="password"
              autoComplete="current-password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />

            <button type="submit" className="btn" disabled={loadingId !== null}>
              {loadingId === "credentials" ? "Đang vào…" : "Đăng nhập"}
            </button>
          </form>

          <p className="muted">hoặc chọn nhanh vai trò:</p>

          <div className="login-grid">
            {DEV_PERSONAS.map((persona) => (
              <div key={persona.id} className="login-option">
                <div>
                  <strong>{persona.label}</strong>
                  <span className="muted">sub={persona.sub}</span>
                </div>
                <button
                  type="button"
                  className="btn"
                  disabled={loadingId !== null}
                  onClick={() => login(persona.id, persona.sub, persona.email)}
                >
                  {loadingId === persona.id ? "Đang vào…" : "Vào ứng dụng"}
                </button>
              </div>
            ))}
          </div>

          <p className="muted">
            Yêu cầu backend đang chạy tại <code>http://localhost:5167</code>.
          </p>
        </div>
      </main>
    </div>
  );
}
