# UAT PAY-FR-018 — So kỳ mẫu vs bảng tay C&B = **0 đồng** (sau làm tròn quy chế)

| Meta | Giá trị |
|------|---------|
| Module | payroll |
| FR | PAY-FR-018 |
| Owner QC / C&B | |
| Ngày | 2026-09-04 |
| `periodYm` | **TBD** (file ghi `Tháng: ...... / 2026`) |

## Mục tiêu

Mỗi dòng `pay_line` kỳ mẫu khớp bảng spreadsheet C&B (thực lĩnh) **sai số 0 đồng** sau làm tròn quy chế.

## Tiền đề

- [ ] TIM tháng mẫu đã chốt (map NV → employeeCode HRM)
- [ ] Master BH/TNCN / PC / hệ số TV **khớp** bảng C&B
- [ ] PAY Draft đã chạy + Closed
- [x] File Excel lương C&B: `assets/Bang_Luong_Chuan_CB.xlsx` (copy từ Downloads — không sửa gốc)
- [x] File chấm công kèm: `assets/Mau_Bang_Cham_Cong_Hang_Ngay.xlsx` (TIM — không đủ FR-018 một mình)

## C&B — Thực lĩnh (tính từ công thức sheet, vì chưa có cached Excel values)

Sheet: `Bảng Lương Tổng Hợp`. Quy tắc file:

- `H = ROUND(E/F*G, 0)` · `L = H+I+J+K`
- BH trên **E** (lương thỏa thuận): 8% + 1.5% + 1% · `ROUND`
- TNCN lũy tiến trên `T = MAX(0, L−J−P−Q−S)` (trừ PC ăn trưa khỏi TNTT)
- `W = L − P − U − V` (Thực lĩnh); **U không ROUND** trên công thức → có thể lẻ xu

| employeeCode | E thỏa thuận | G công | H | L gross | P BH | U TNCN | V tạm ứng | **W Thực lĩnh** |
|--------------|-------------:|-------:|--:|--------:|-----:|-------:|----------:|----------------:|
| NV001 | 25_000_000 | 26 | 25_000_000 | 28_230_000 | 2_625_000 | 697_500 | 0 | **24_907_500** |
| NV002 | 15_000_000 | 25 | 14_423_077 | 15_453_077 | 1_575_000 | 107_403.85 | 1_000_000 | **12_770_673.15** |
| NV003 | 12_000_000 | 26 | 12_000_000 | 13_030_000 | 1_260_000 | 0 | 0 | **11_770_000** |
| NV004 | 18_000_000 | 24 | 16_615_385 | 18_645_385 | 1_890_000 | 252_538.5 | 0 | **16_502_846.5** |
| NV005 | 10_000_000 | 26 | 10_000_000 | 11_030_000 | 1_050_000 | 0 | 0 | **9_980_000** |

## Gap vs engine PAY (trước slice K)

| Hạng mục | Bảng C&B | MVP cũ |
|----------|----------|--------|
| Ngày công chuẩn | **26** | 22 |
| BH | 8+1.5+1 trên **E** | 10% trên gross |
| TNCN | Lũy tiến + GTGC/NPT; trừ PC ăn | Flat 5% |
| Tạm ứng | Có (V) | Không |

→ **Slice K** (`cursor/pay-slice-k-cb-quy-che`) align engine/master theo C&B.

## Bảng kết quả UAT (hệ thống vs C&B)

| employeeCode | netPay hệ thống | netPay C&B (W) | Δ | Pass? |
|--------------|----------------:|---------------:|--:|-------|
| NV001 | 24_907_500 | 24_907_500 | 0 | PASS |
| NV002 | 12_770_673.15 | 12_770_673.15 | 0 | PASS |
| NV003 | 11_770_000 | 11_770_000 | 0 | PASS |
| NV004 | 16_502_846.5 | 16_502_846.5 | 0 | PASS |
| NV005 | 9_980_000 | 9_980_000 | 0 | PASS |

Chạy: `hrm-backend/scripts/e2e-api-pay-slice-k-cb-uat.sh` (2026-09-04) → **all Δ=0**.

## Verdict

| | |
|--|--|
| **PASS** | Mọi dòng Δ = 0 (e2e K) — chờ Ban HR/QC ký chính thức |
| **FAIL** | Có dòng Δ ≠ 0 |
| **BLOCKED** | Engine/master chưa cùng quy chế |

Ký QC: ________ · C&B: ________ · Ngày: 2026-09-04
