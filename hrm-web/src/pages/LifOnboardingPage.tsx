import { useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import {
  closeLifOnboarding,
  createLifOnboarding,
  fetchLifOnboardingChecklist,
  fetchLifOnboardings,
  markLifOnboardingProvisioned,
  upsertLifOnChecklistTick,
} from "../api/client";
import type { LifOffChecklistBoard, LifOnboarding } from "../api/types";
import { isHr, isItOnly, useCurrentUser } from "../hooks/useCurrentUser";

const SYSTEMS = ["EmailCty", "Git", "CrmSp", "Chat"] as const;

/** LIF-SCR-002 — checklist on + cấp TK lúc on; cấm hẹn Git = N+3. */
export function LifOnboardingPage() {
  const user = useCurrentUser();
  const hr = isHr(user);
  const itOnly = isItOnly(user);
  const [searchParams] = useSearchParams();
  const [items, setItems] = useState<LifOnboarding[]>([]);
  const [board, setBoard] = useState<LifOffChecklistBoard | null>(null);
  const [caseId, setCaseId] = useState(searchParams.get("case") ?? "");
  const [employeeId, setEmployeeId] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  async function reload(selected?: string) {
    const rows = await fetchLifOnboardings();
    setItems(rows);
    const id = selected || caseId || rows[0]?.id || "";
    if (!caseId && rows[0]) setCaseId(rows[0].id);
    if (id) {
      setCaseId(id);
      setBoard(await fetchLifOnboardingChecklist(id));
    } else {
      setBoard(null);
    }
  }

  useEffect(() => {
    const fromQuery = searchParams.get("case");
    reload(fromQuery || undefined)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  async function onSelectCase(id: string) {
    setCaseId(id);
    setError(null);
    try {
      setBoard(await fetchLifOnboardingChecklist(id));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Không tải checklist");
    }
  }

  async function onCreate() {
    setError(null);
    setMessage(null);
    try {
      const dto = await createLifOnboarding(employeeId);
      setMessage(`Đã mở onboarding ${dto.employeeCode}.`);
      await reload(dto.id);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Tạo onboarding thất bại");
    }
  }

  async function onTick(code: string, checked: boolean) {
    setError(null);
    try {
      setBoard(await upsertLifOnChecklistTick(caseId, code, checked));
      await reload(caseId);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Tick thất bại");
    }
  }

  async function onProvision(system: string) {
    setError(null);
    setMessage(null);
    try {
      const dto = await markLifOnboardingProvisioned(caseId, system, false);
      setMessage(`Đã cấp ${system} cho ${dto.employeeCode} (lúc on).`);
      await reload(caseId);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Cấp TK thất bại");
    }
  }

  async function onClose() {
    setError(null);
    setMessage(null);
    try {
      const dto = await closeLifOnboarding(caseId);
      setMessage(`Đã đóng on ${dto.employeeCode}.`);
      await reload();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Đóng on thất bại");
    }
  }

  const selected = items.find((x) => x.id === caseId);

  return (
    <div className="card stack">
      <div>
        <h2>Onboarding</h2>
        <p className="muted">
          LIF-SCR-002 — checklist Must + cấp Email/Git/CRM SP/chat lúc on; cấm hẹn Git = N+3
          (FR-001/002).
          {itOnly ? " · IT: xem case + cấp TK; HR mở/đóng case." : ""}
        </p>
        <Link className="btn btn-ghost" to="/lif">
          ← Danh sách (SCR-001)
        </Link>
      </div>
      {error && <div className="error-box">{error}</div>}
      {message && <div className="success-box">{message}</div>}

      <div className="row" style={{ gap: 8, flexWrap: "wrap", alignItems: "center" }}>
        {hr && (
          <>
            <label className="muted">
              EmployeeId{" "}
              <input
                value={employeeId}
                onChange={(e) => setEmployeeId(e.target.value)}
                placeholder="guid NV"
                style={{ minWidth: 260 }}
              />
            </label>
            <button type="button" className="btn" onClick={onCreate} disabled={!employeeId}>
              Mở onboarding
            </button>
          </>
        )}
        <label className="muted">
          Case{" "}
          <select value={caseId} onChange={(e) => onSelectCase(e.target.value)}>
            {items.map((c) => (
              <option key={c.id} value={c.id}>
                {c.employeeCode} · {c.status}
              </option>
            ))}
          </select>
        </label>
        {hr && (
          <button
            type="button"
            className="btn btn-secondary"
            onClick={onClose}
            disabled={!board?.canClose}
          >
            Đóng on
          </button>
        )}
      </div>

      {loading && <p className="muted">Đang tải…</p>}

      {selected && (
        <div className="stack">
          <h3>Cấp TK lúc on</h3>
          <div className="row" style={{ gap: 8, flexWrap: "wrap" }}>
            {SYSTEMS.map((sys) => {
              const done =
                sys === "EmailCty"
                  ? selected.emailCtyProvisioned
                  : sys === "Git"
                    ? selected.gitProvisioned
                    : sys === "CrmSp"
                      ? selected.crmSpProvisioned
                      : selected.chatProvisioned;
              return (
                <button
                  key={sys}
                  type="button"
                  className="btn btn-secondary"
                  disabled={!!done || selected.status === "Closed"}
                  onClick={() => onProvision(sys)}
                >
                  {sys}: {done ? "Đã cấp" : "Cấp ngay"}
                </button>
              );
            })}
          </div>
        </div>
      )}

      {board && (
        <div className="stack">
          <h3>Checklist on</h3>
          <div className="table-wrap">
            <table>
              <thead>
                <tr>
                  <th>Tick</th>
                  <th>Mã</th>
                  <th>Hạng mục</th>
                  <th>Must?</th>
                </tr>
              </thead>
              <tbody>
                {board.items.map((i) => (
                  <tr key={i.code}>
                    <td>
                      <input
                        type="checkbox"
                        checked={i.isChecked}
                        disabled={!hr}
                        onChange={(e) => onTick(i.code, e.target.checked)}
                      />
                    </td>
                    <td>{i.code}</td>
                    <td>{i.name}</td>
                    <td>{i.isMust ? "Must" : "Optional"}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <p className="muted">canClose = {board.canClose ? "true" : "false"}</p>
        </div>
      )}
    </div>
  );
}
