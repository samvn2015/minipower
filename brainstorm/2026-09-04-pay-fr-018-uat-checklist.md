# UAT PAY-FR-018 — So kỳ mẫu vs bảng tay C&B = **0 đồng** (sau làm tròn quy chế)

| Meta | Giá trị |
|------|---------|
| Module | payroll |
| FR | PAY-FR-018 |
| Owner QC / C&B | |
| Ngày | |

## Mục tiêu

Mỗi dòng `pay_line` kỳ mẫu khớp bảng spreadsheet C&B (thực lĩnh) **sai số 0 đồng** sau làm tròn quy chế.

## Tiền đề

- [ ] TIM tháng mẫu đã chốt
- [ ] Master BH/TNCN / PC / hệ số TV đúng kỳ
- [ ] PAY Draft đã chạy + Closed
- [ ] File Excel C&B kỳ mẫu (đặt `assets/` — không sửa file gốc)

## Bước

1. Chọn `periodYm` mẫu: ________
2. Export / query lines: `GET /v1/pay/periods/{ym}` (HR) — cột N_tính, OT, PC, BH, TNCN, netPay
3. Hoặc script: `hrm-backend/scripts/e2e-api-pay-slice-*.sh` seed rồi so DTO
4. Đối chiếu từng `employeeCode` với cột thực lĩnh C&B
5. Ghi lệch (nếu có) → bug / CR; không “làm tròn tay” ngoài quy chế

## Bảng kết quả

| employeeCode | netPay hệ thống | netPay C&B | Δ | Pass? |
|--------------|-----------------|------------|---|-------|
| | | | | |

## Verdict

| | |
|--|--|
| **PASS** | Mọi dòng Δ = 0 |
| **FAIL** | Có dòng Δ ≠ 0 — liệt kê mã NV |

Ký QC: ________ · C&B: ________ · Ngày: ________
