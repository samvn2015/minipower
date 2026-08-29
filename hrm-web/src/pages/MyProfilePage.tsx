import { FormEvent, useEffect, useState } from "react";
import { fetchEmployee, fetchMyEmployee, updateEmployee } from "../api/client";
import type { EmployeeDetail } from "../api/types";
import { isHrOrIt, useCurrentUser } from "../hooks/useCurrentUser";

export function MyProfilePage() {
  const user = useCurrentUser();
  const [profile, setProfile] = useState<EmployeeDetail | null>(null);
  const [fullName, setFullName] = useState("");
  const [emailCty, setEmailCty] = useState("");
  const [cccd, setCccd] = useState("");
  const [taxId, setTaxId] = useState("");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);

  useEffect(() => {
    let active = true;
    fetchMyEmployee()
      .then((employee) => {
        if (!active) return;
        setProfile(employee);
        setFullName(employee.fullName ?? "");
        setEmailCty(employee.emailCty ?? "");
        setCccd(employee.cccd ?? "");
        setTaxId(employee.taxId ?? "");
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

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    if (!profile) return;
    setSaving(true);
    setError(null);
    setMessage(null);
    try {
      await updateEmployee(profile.id, {
        fullName: fullName.trim() || undefined,
        emailCty: emailCty.trim() || undefined,
        cccd: cccd.trim() || undefined,
        taxId: taxId.trim() || undefined,
      });
      setMessage("Đã lưu hồ sơ.");
      setProfile(await fetchEmployee(profile.id));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Lưu thất bại");
    } finally {
      setSaving(false);
    }
  }

  if (loading) {
    return (
      <div className="card">
        <p className="muted">Đang tải hồ sơ…</p>
      </div>
    );
  }

  if (!profile) {
    return (
      <div className="card stack">
        <h2>Hồ sơ của tôi</h2>
        <div className="error-box">{error ?? "Không tìm thấy hồ sơ liên kết tài khoản."}</div>
        <p className="muted">
          Cần IAM account liên kết MNV (IAM-FR-017). Thử đăng nhập bằng email trùng hồ sơ EMP.
        </p>
      </div>
    );
  }

  return (
    <div className="card stack">
      <div>
        <h2>Hồ sơ của tôi</h2>
        <p className="muted">
          EMP-SCR-003 — {user.name ?? user.sub} · field org/HĐ do HR quản lý (read-only).
        </p>
      </div>

      {error && <div className="error-box">{error}</div>}
      {message && <p className="badge">{message}</p>}

      <div className="form-grid">
        <p>
          <strong>Mã NV:</strong> {profile.employeeCode}
        </p>
        <p>
          <strong>Đơn vị:</strong> {profile.orgUnitCode ?? "—"}
        </p>
        <p>
          <strong>Trạng thái:</strong> {profile.status}
        </p>
        {profile.contract && (
          <p>
            <strong>HĐ:</strong> {profile.contract.contractType} · {profile.contract.startDate}
            {profile.contract.endDate ? ` → ${profile.contract.endDate}` : ""}
          </p>
        )}
        {profile.educationLevelName && (
          <p>
            <strong>Học vấn:</strong> {profile.educationLevelName}
          </p>
        )}
        {profile.seniority && (
          <p>
            <strong>Thâm niên:</strong> {profile.seniority.displayText}
          </p>
        )}
      </div>

      <form className="form-grid" onSubmit={onSubmit}>
        <label>
          Họ tên
          <input value={fullName} onChange={(e) => setFullName(e.target.value)} />
        </label>
        <label>
          Email công ty
          <input
            type="email"
            value={emailCty}
            onChange={(e) => setEmailCty(e.target.value)}
          />
        </label>
        <label>
          CCCD
          <input value={cccd} onChange={(e) => setCccd(e.target.value)} />
        </label>
        <label>
          MST
          <input value={taxId} onChange={(e) => setTaxId(e.target.value)} />
        </label>

        {!isHrOrIt(user) && (
          <p className="muted">NV không sửa org/HĐ trên self-service (EMP-FR-007).</p>
        )}

        <button type="submit" className="btn" disabled={saving}>
          {saving ? "Đang lưu…" : "Lưu thay đổi"}
        </button>
      </form>
    </div>
  );
}
