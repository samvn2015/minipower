import { FormEvent, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import {
  createEmployee,
  fetchContractTypes,
  fetchEducationLevels,
  fetchEmployee,
  fetchEmployees,
  submitLineManagerChange,
  updateEmployee,
} from "../api/client";
import type { EducationLevel, EmpCatalogItem, EmployeeListItem } from "../api/types";

const ORG_UNITS = [{ code: "ORG-HQ", label: "Trụ sở chính (ORG-HQ)" }];

export function EmployeeFormPage() {
  const { id } = useParams();
  const isNew = !id || id === "new";
  const navigate = useNavigate();

  const [employeeCode, setEmployeeCode] = useState("");
  const [fullName, setFullName] = useState("");
  const [emailCty, setEmailCty] = useState("");
  const [orgUnitCode, setOrgUnitCode] = useState("ORG-HQ");
  const [educationLevelCode, setEducationLevelCode] = useState("");
  const [educationLevels, setEducationLevels] = useState<EducationLevel[]>([]);
  const [contractTypes, setContractTypes] = useState<EmpCatalogItem[]>([]);
  const [contractType, setContractType] = useState("");
  const [seniorityText, setSeniorityText] = useState<string | null>(null);
  const [withContract, setWithContract] = useState(true);
  const [status, setStatus] = useState<string | null>(null);
  const [lineManagerId, setLineManagerId] = useState<string | null>(null);
  const [candidates, setCandidates] = useState<EmployeeListItem[]>([]);
  const [proposedLmId, setProposedLmId] = useState("");
  const [lmSubmitting, setLmSubmitting] = useState(false);
  const [lmMessage, setLmMessage] = useState<string | null>(null);
  const [loading, setLoading] = useState(!isNew);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let active = true;
    Promise.all([fetchEducationLevels(), fetchContractTypes()])
      .then(([edu, contracts]) => {
        if (!active) return;
        setEducationLevels(edu);
        setContractTypes(contracts);
        if (!contractType && contracts[0]) setContractType(contracts[0].code);
      })
      .catch(() => {
        if (active) {
          setEducationLevels([]);
          setContractTypes([]);
        }
      });
    return () => {
      active = false;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    if (isNew || !id) return;

    let active = true;
    fetchEmployee(id)
      .then((employee) => {
        if (!active) return;
        setEmployeeCode(employee.employeeCode);
        setFullName(employee.fullName ?? "");
        setEmailCty(employee.emailCty ?? "");
        setOrgUnitCode(employee.orgUnitCode ?? "ORG-HQ");
        setEducationLevelCode(employee.educationLevelCode ?? "");
        setSeniorityText(employee.seniority?.displayText ?? null);
        setWithContract(employee.contract !== null);
        setStatus(employee.status);
        setLineManagerId(employee.lineManagerEmployeeId);
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
  }, [id, isNew]);

  useEffect(() => {
    if (isNew || !id) return;
    let active = true;
    fetchEmployees()
      .then((rows) => {
        if (active) setCandidates(rows.filter((row) => row.id !== id));
      })
      .catch(() => {
        if (active) setCandidates([]);
      });
    return () => {
      active = false;
    };
  }, [id, isNew]);

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    setSaving(true);
    setError(null);

    try {
      if (isNew) {
        const created = await createEmployee({
          employeeCode: employeeCode.trim(),
          fullName: fullName.trim() || undefined,
          emailCty: emailCty.trim() || undefined,
          orgUnitCode,
          educationLevelCode: educationLevelCode || undefined,
          contract: withContract
            ? {
                contractType: contractType || contractTypes[0]?.code || "PROBATION",
                startDate: "2026-01-01",
                endDate: "2026-06-30",
                isProbation: (contractType || contractTypes[0]?.code) === "PROBATION",
              }
            : undefined,
        });
        navigate(`/employees/${created.id}`, { replace: true });
        return;
      }

      if (!id) return;
      await updateEmployee(id, {
        fullName: fullName.trim() || undefined,
        emailCty: emailCty.trim() || undefined,
        orgUnitCode,
        educationLevelCode: educationLevelCode || undefined,
      });
      navigate("/employees");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Lưu thất bại");
    } finally {
      setSaving(false);
    }
  }

  async function onSubmitLmChange() {
    if (!id || !proposedLmId) return;
    setLmSubmitting(true);
    setLmMessage(null);
    setError(null);
    try {
      const result = await submitLineManagerChange(id, proposedLmId);
      setLmMessage(`Đã gửi đề xuất — mã ${result.requestId} · ${result.status}`);
      setProposedLmId("");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Gửi đổi LM thất bại");
    } finally {
      setLmSubmitting(false);
    }
  }

  if (loading) {
    return (
      <div className="card">
        <p className="muted">Đang tải hồ sơ…</p>
      </div>
    );
  }

  return (
    <div className="card stack">
      <div className="row" style={{ justifyContent: "space-between" }}>
        <div>
          <h2>{isNew ? "Tạo nhân viên" : "Sửa hồ sơ"}</h2>
          <p className="muted">EMP-SCR-002 — HR/IT chỉnh hồ sơ (đổi LM qua SCR-005/006).</p>
        </div>
        <Link className="btn btn-secondary" to="/employees">
          ← Danh sách
        </Link>
      </div>

      {error && <div className="error-box">{error}</div>}

      <form className="form-grid" onSubmit={onSubmit}>
        <label>
          Mã nhân viên
          <input
            value={employeeCode}
            onChange={(e) => setEmployeeCode(e.target.value)}
            required
            disabled={!isNew}
            placeholder="MNV-001"
          />
        </label>

        <label>
          Họ tên
          <input
            value={fullName}
            onChange={(e) => setFullName(e.target.value)}
            placeholder="Nguyễn Văn A"
          />
        </label>

        <label>
          Email công ty
          <input
            type="email"
            value={emailCty}
            onChange={(e) => setEmailCty(e.target.value)}
            placeholder="name@company.local"
          />
        </label>

        <label>
          Đơn vị
          <select value={orgUnitCode} onChange={(e) => setOrgUnitCode(e.target.value)}>
            {ORG_UNITS.map((unit) => (
              <option key={unit.code} value={unit.code}>
                {unit.label}
              </option>
            ))}
          </select>
        </label>

        <label>
          Trình độ học vấn
          <select
            value={educationLevelCode}
            onChange={(e) => setEducationLevelCode(e.target.value)}
          >
            <option value="">— Chọn bậc —</option>
            {educationLevels.map((level) => (
              <option key={level.code} value={level.code}>
                {level.name}
              </option>
            ))}
          </select>
        </label>

        {!isNew && seniorityText && (
          <p className="muted">
            <strong>Thâm niên:</strong> {seniorityText} (master)
          </p>
        )}

        {isNew && (
          <>
            <label className="row">
              <input
                type="checkbox"
                checked={withContract}
                onChange={(e) => setWithContract(e.target.checked)}
              />
              Tạo kèm HĐ (loại từ master FR-014)
            </label>
            {withContract && (
              <label>
                Loại HĐ
                <select value={contractType} onChange={(e) => setContractType(e.target.value)}>
                  {contractTypes.map((t) => (
                    <option key={t.code} value={t.code}>
                      {t.name}
                    </option>
                  ))}
                </select>
              </label>
            )}
          </>
        )}

        {!isNew && (
          <>
            <p className="muted">Trạng thái: {status ?? "—"}</p>
            {lineManagerId && (
              <p className="muted">Line Manager ID: {lineManagerId}</p>
            )}
          </>
        )}

        <div className="row">
          <button type="submit" className="btn" disabled={saving}>
            {saving ? "Đang lưu…" : isNew ? "Tạo nhân viên" : "Lưu thay đổi"}
          </button>
        </div>
      </form>

      {!isNew && id && (
        <div className="card stack" style={{ marginTop: "0.5rem", background: "#f8fafc" }}>
          <h3 style={{ margin: 0 }}>Đề xuất đổi Line Manager</h3>
          <p className="muted">EMP-SCR-005 — HR gửi; duyệt tại màn hàng chờ SCR-006.</p>
          {lmMessage && <p className="badge">{lmMessage}</p>}
          <div className="form-grid">
            <label>
              Line Manager mới
              <select
                value={proposedLmId}
                onChange={(e) => setProposedLmId(e.target.value)}
              >
                <option value="">— Chọn LM —</option>
                {candidates.map((candidate) => (
                  <option key={candidate.id} value={candidate.id}>
                    {candidate.employeeCode} — {candidate.fullName ?? "—"}
                  </option>
                ))}
              </select>
            </label>
            <div className="row">
              <button
                type="button"
                className="btn"
                disabled={!proposedLmId || lmSubmitting}
                onClick={() => void onSubmitLmChange()}
              >
                {lmSubmitting ? "Đang gửi…" : "Gửi đề xuất đổi LM"}
              </button>
              <Link className="btn btn-secondary" to="/line-manager-changes">
                Mở hàng chờ duyệt
              </Link>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
