import { useEffect, useState } from "react";
import { fetchMyPayslips, fetchPayslip } from "../api/client";
import type { PayPayslip } from "../api/types";

export function PayPayslipPage() {
  const [items, setItems] = useState<PayPayslip[]>([]);
  const [selected, setSelected] = useState<PayPayslip | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLoading(true);
    fetchMyPayslips()
      .then((rows) => {
        setItems(rows);
        setSelected(rows[0] ?? null);
      })
      .catch((err: Error) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  async function openSlip(id: string) {
    setError(null);
    try {
      const slip = await fetchPayslip(id);
      setSelected(slip);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Không mở được phiếu");
    }
  }

  return (
    <div className="card stack">
      <div>
        <h2>Phiếu lương của tôi</h2>
        <p className="muted">PAY-SCR-005 — chỉ kỳ đã chốt; không xem phiếu người khác (PAY-FR-010).</p>
      </div>

      {error && <div className="error-box">{error}</div>}
      {loading && <p className="muted">Đang tải…</p>}

      {!loading && items.length === 0 && <p className="muted">Chưa có phiếu kỳ Closed.</p>}

      {items.length > 0 && (
        <div className="row" style={{ gap: 8, flexWrap: "wrap" }}>
          {items.map((p) => (
            <button
              key={p.id}
              type="button"
              className={selected?.id === p.id ? "btn" : "btn btn-secondary"}
              onClick={() => openSlip(p.id)}
            >
              {p.periodYm}
            </button>
          ))}
        </div>
      )}

      {selected && (
        <div className="stack">
          <p>
            Kỳ <strong>{selected.periodYm}</strong> · {selected.employeeCode} · {selected.status}
          </p>
          <div className="table-wrap">
            <table>
              <tbody>
                <tr>
                  <th>N_tính</th>
                  <td>{selected.nTinh}</td>
                </tr>
                <tr>
                  <th>Hệ số TV</th>
                  <td>{selected.timeWageFactor}</td>
                </tr>
                <tr>
                  <th>PC HĐ</th>
                  <td>{selected.contractAllowance}</td>
                </tr>
                <tr>
                  <th>PC tháng</th>
                  <td>{selected.monthlyAllowance}</td>
                </tr>
                <tr>
                  <th>BH</th>
                  <td>{selected.bhAmount}</td>
                </tr>
                <tr>
                  <th>TNCN tạm</th>
                  <td>{selected.tncnAmount}</td>
                </tr>
                <tr>
                  <th>Thực lĩnh</th>
                  <td>{selected.netPay}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}
