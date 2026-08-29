import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { fetchDevToken } from "../api/client";
import { DEV_PERSONAS } from "../api/types";

export function LoginPage() {
  const navigate = useNavigate();
  const [loadingId, setLoadingId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function login(personaId: string, sub: string, email?: string) {
    setLoadingId(personaId);
    setError(null);
    try {
      await fetchDevToken(sub, email);
      navigate("/employees", { replace: true });
    } catch (err) {
      setError(err instanceof Error ? err.message : "Đăng nhập thất bại");
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
              Chọn vai trò để trải nghiệm. Production sẽ dùng Lark SSO — hiện dùng JWT dev
              từ backend local.
            </p>
          </div>

          {error && <div className="error-box">{error}</div>}

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
