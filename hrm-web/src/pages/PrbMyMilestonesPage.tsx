import { useEffect, useState } from "react";
import { fetchMyProbationMilestones } from "../api/client";
import type { ProbationMilestone } from "../api/types";

/** PRB-SCR-004 — chỉ hiển thị; cấm date picker ảo (FR-015). */
export function PrbMyMilestonesPage() {
  const [data, setData] = useState<ProbationMilestone | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchMyProbationMilestones()
      .then(setData)
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div className="card stack">
      <div>
        <h2>Mốc thử việc của tôi</h2>
        <p className="muted">
          PRB-SCR-004 — nguồn {data?.source ?? "EMP.Contract"}; sửa ngày chỉ trên hồ sơ HĐ EMP.
        </p>
      </div>
      {error && <div className="error-box">{error}</div>}
      {loading && <p className="muted">Đang tải…</p>}
      {data && (
        <div className="table-wrap">
          <table>
            <tbody>
              <tr>
                <th>Mã NV</th>
                <td>{data.employeeCode}</td>
              </tr>
              <tr>
                <th>Đang TV?</th>
                <td>{data.isOnProbation ? "Có" : "Không"}</td>
              </tr>
              <tr>
                <th>Loại HĐ</th>
                <td>{data.contractType ?? "—"}</td>
              </tr>
              <tr>
                <th>BĐ TV</th>
                <td>{data.probationStartDate ?? "—"}</td>
              </tr>
              <tr>
                <th>KT TV</th>
                <td>{data.probationEndDate ?? "—"}</td>
              </tr>
              <tr>
                <th>Mốc T-15</th>
                <td>{data.t15DueDate ?? "—"}</td>
              </tr>
              <tr>
                <th>Mốc T-7</th>
                <td>{data.t7DueDate ?? "—"}</td>
              </tr>
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
