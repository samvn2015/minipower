import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { fetchLifOffboardings, fetchLifOnboardings } from "../api/client";
import type { LifOffboarding, LifOnboarding } from "../api/types";
import { isHr, useCurrentUser } from "../hooks/useCurrentUser";

/** LIF-SCR-001 — danh sách on/off; điều hướng SCR-002…006. */
export function LifCasesPage() {
  const hr = isHr(useCurrentUser());
  const [ons, setOns] = useState<LifOnboarding[]>([]);
  const [offs, setOffs] = useState<LifOffboarding[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([fetchLifOnboardings(), fetchLifOffboardings()])
      .then(([o, f]) => {
        setOns(o);
        setOffs(f);
      })
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div className="card stack">
      <div>
        <h2>Lifecycle — danh sách</h2>
        <p className="muted">LIF-SCR-001 — on/off · N · N+3 · trạng thái khóa. Mở màn chi tiết riêng.</p>
        <div className="row" style={{ gap: 8, flexWrap: "wrap" }}>
          <Link className="btn btn-ghost" to="/lif/onboarding">
            Mở on (SCR-002)
          </Link>
        </div>
      </div>
      {error && <div className="error-box">{error}</div>}
      {loading && <p className="muted">Đang tải…</p>}

      <div className="stack">
        <h3>Onboarding</h3>
        {ons.length === 0 && !loading && <p className="muted">Chưa có case on.</p>}
        {ons.length > 0 && (
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>NV</th>
                  <th>Status</th>
                  <th>Email</th>
                  <th>Git</th>
                  <th>CRM SP</th>
                  <th>Chat</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {ons.map((r) => (
                  <tr key={r.id}>
                    <td>{r.employeeCode}</td>
                    <td>{r.status}</td>
                    <td>{r.emailCtyProvisioned ? "Đã cấp" : "—"}</td>
                    <td>{r.gitProvisioned ? "Đã cấp" : "—"}</td>
                    <td>{r.crmSpProvisioned ? "Đã cấp" : "—"}</td>
                    <td>{r.chatProvisioned ? "Đã cấp" : "—"}</td>
                    <td>
                      <Link className="btn btn-ghost" to={`/lif/onboarding?case=${r.id}`}>
                        SCR-002
                      </Link>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <div className="stack">
        <h3>Offboarding</h3>
        {offs.length === 0 && !loading && <p className="muted">Chưa có case off.</p>}
        {offs.length > 0 && (
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>NV</th>
                  <th>Source</th>
                  <th>Status</th>
                  <th>N</th>
                  <th>N+3</th>
                  <th>Git</th>
                  <th>CRM SP</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {offs.map((r) => (
                  <tr key={r.id}>
                    <td>{r.employeeCode}</td>
                    <td>{r.source}</td>
                    <td>{r.status}</td>
                    <td>{r.lastWorkingDayN ?? "—"}</td>
                    <td>{r.nPlus3Expected ?? "—"}</td>
                    <td>{r.gitLocked ? "Đã khóa" : "Mở"}</td>
                    <td>{r.crmSpLocked ? "Đã khóa" : "Mở"}</td>
                    <td className="row" style={{ gap: 4, flexWrap: "wrap" }}>
                      {hr && (
                        <Link className="btn btn-ghost" to={`/lif/offboarding/${r.id}/n`}>
                          N
                        </Link>
                      )}
                      <Link className="btn btn-ghost" to={`/lif/offboarding/${r.id}/locks`}>
                        Khóa
                      </Link>
                      <Link className="btn btn-ghost" to={`/lif/offboarding/${r.id}/checklist`}>
                        Checklist
                      </Link>
                      <Link className="btn btn-ghost" to={`/lif/offboarding/${r.id}/security`}>
                        CR
                      </Link>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
