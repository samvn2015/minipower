# Doc-review regression — DOC-11 v0.2 + DOC-12 v0.2 · [2026-09-07]

**Slice:** `04-platform` — DOC-11 v0.2 · DOC-12 v0.2 + `openapi.yaml` v0.2
**Mục đích:** xác nhận Blocker **B2/B3** của [pass 2026-09-07](doc-review-2026-09-07-doc11-12.md) đã đóng
**Reviewer:** QC (một pass đối kháng) · **Verdict: ⛔ BLOCK** — 1 Blocker mới, B1 chưa giải

> Bản v0.2 do trợ lý soạn. Pass này soi **chính bản sửa đó**, không mặc định nó đúng.

---

## Kết quả với B2 / B3

| Blocker | Trạng thái | Bằng chứng |
|---------|-----------|-----------|
| **B2** — DOC-11 lệch schema | ✅ **đóng về nội dung** | 41 entity trong DOC-11 §3 **khớp tuyệt đối** 41 `DbSet` trong `AppDbContext` — `comm` hai chiều đều rỗng. Bảng, unique, tên cột lấy từ `AppDbContextModelSnapshot` |
| **B3** — DOC-12 lệch API | ✅ **đóng về nội dung** | `openapi.yaml` 82 path / 34 schema, sinh từ Swagger runtime; round-trip YAML→JSON khớp tuyệt đối với tài liệu gốc |

Nhưng cả hai **chưa đóng về quy trình** — xem F5.

---

## Finding

### 🔴 Blocker

**[Blocker] Nhất quán version — DOC-11 §9 · DOC-12 §9**
Bằng chứng: bảng phiên bản của cả hai DOC ghi `0.2 … **Chốt**`, nhưng mục Phê duyệt chỉ có chữ ký **v0.1 ngày 2026-08-26**, và **không có `DEC-*` nào** cho v0.2. Đối chiếu: ADR-007 v0.2 có DEC-ARC-015, ADR-010 v0.2/v0.3 có DEC-ARC-018/019.
Vì sao lỗi: checklist chiều 5 — *"Version gán trước sign-off"*. `02-baseline/` lấy bản **đã ký**; baseline v0.2 lúc này là baseline một phiên bản chưa ai duyệt. Đúng cái bẫy mà gate này tồn tại để chặn.
Đề xuất: PGD ký v0.2 kèm `DEC-*`, **hoặc** hạ nhãn về `Draft` cho tới khi ký. Owner: PGD + SA.

### 🟡 Major

**[Major] Mâu thuẫn — DOC-11 §5 từ điển nhạy cảm**
Bằng chứng: dòng `` | `Cccd`, `Mst` trên `Employee` | Định danh | **Y** | ``. Thuộc tính thật là **`TaxId`** (`public string? TaxId`), unique `TaxId` — `Mst` **không tồn tại** trong code.
Vì sao lỗi: đây **đúng loại lỗi mà B2 tồn tại để sửa**, và do chính bản v0.2 tạo ra khi viết lại §5. Phân loại PII trỏ vào cột không có → QC không viết được test, dev tìm `Mst` không thấy.
Đề xuất: đổi `Mst` → `TaxId`. Owner: SA.

**[Major] Traceability — DOC-12 §8**
Bằng chứng: bảng truy vết vẫn dùng path v0.1: `/lev/*`, `/tim/imports`, `/pay/payslips/*`, `/prb/.../decide`. Thiếu tiền tố **`/v1`** ở tất cả; riêng `/prb/.../decide` sai hình dạng — route thật là `/v1/prb/evaluations/{employeeId}/decide` (chính §4.1 đã ghi).
Vì sao lỗi: B3 sửa §4 nhưng **bỏ sót §8**. Chuỗi `NFR-002 → /pay/payslips/*` vẫn trỏ path không tồn tại đúng chữ → trace đứt ở đúng NFR mạnh nhất.
Đề xuất: đồng bộ §8 với `openapi.yaml`. Owner: SA.

**[Major] Mâu thuẫn — DOC-12 §1 vs `openapi.yaml`**
Bằng chứng: §1 ghi **`OAS 3.0.3`**; `openapi.yaml` dòng 1 là `openapi: 3.0.1` (Swashbuckle sinh).
Vì sao lỗi: DOC mâu thuẫn với chính file mà nó tuyên bố là SoT machine.
Đề xuất: sửa §1 thành 3.0.1, hoặc nâng cấu hình Swashbuckle. Owner: SA.

**[Major] Mâu thuẫn — DOC-12 §2 vs `openapi.yaml`**
Bằng chứng: §2 ghi *"**Cấm:** `POST /auth/login` với password"*, trong khi `openapi.yaml` nay mô tả `POST /dev/login` nhận `username` + `password`.
Vì sao lỗi: lệnh cấm tuyệt đối trong §2 mâu thuẫn nội dung chính spec. Ngoại lệ **có thật và hợp lệ** (DEC-DLV-011 · CR-001 closed-rejected cho Prod, giữ DEV/UAT) nhưng chưa được ghi vào §2 → auditor đọc §2 sẽ coi `/dev/login` là vi phạm.
Đề xuất: §2 thêm ngoại lệ DEV/UAT có trace CR-001. Owner: SA.

### ⚪ Minor

- **[Minor] DOC-11 §2** trỏ [ADR-011](../../docs/04-platform/DOC-09-adr/ADR-011-lo-trinh-tach-service.md) làm lộ trình, nhưng ADR-011 đang **Proposed**. DOC nhãn Chốt tham chiếu kế hoạch chưa duyệt.
- **[Minor] DOC-11 §3** sinh từ `AppDbContextModelSnapshot`, **không** từ database đang chạy. Nếu có ai sửa DB bằng tay thì drift không phát hiện được. Ghi giả định này vào §3.
- **[Minor] DOC-11 §4** master data còn dùng tên khái niệm cũ (*"Mẫu Excel CC | TIM template"*) trong khi §3 là `TimesheetTemplateVersion` / `TimesheetTemplateColumn`.

---

## Trace gaps

| Chuỗi | Trước (pass 1) | Nay |
|---|---|---|
| `NFR-002 → Payslip → DB-PAY` | đứt — entity không tồn tại | ✅ nối: `PayLine`/`PayPeriod`, ghi rõ chưa có hàng rào DB |
| `LEV-FR → LeaveBalance/LeaveRequest` | đứt | ✅ nối (DOC-11 §8 bổ sung) |
| `TIM-FR → TimesheetPeriod/Line` | đứt | ✅ nối |
| `INT-003 → ImportBatch` | đứt | ✅ `TimesheetImportBatch/Row` |
| `INT-004/005 → AccessLock` | đứt | ✅ `LifAccessLockOutbox` |
| 70/87 endpoint → FR | đứt | ✅ 82 path trong OAS |
| **`NFR-002 → /pay/payslips/*`** | — | ⚠️ **đứt mới** — DOC-12 §8 path thiếu `/v1` |

---

## Verdict

⛔ **BLOCK baseline** — 1 Blocker, 4 Major, 3 Minor.

**B2 và B3 đóng về nội dung** — dữ liệu trong hai DOC nay khớp hệ thống thật, đo được bằng diff và round-trip. Nhưng:

1. **F5 chặn:** cả hai mang nhãn `Chốt v0.2` mà chưa ai ký. Không baseline được một phiên bản chưa duyệt.
2. **B1 chưa giải:** ADR-011 còn **Proposed**, `OQ-ARC-014` (khách có chấp nhận lộ trình wave?) còn mở.

Bốn Major đều là **sửa nhỏ trong một file**, không phải làm lại. Riêng finding `Mst`/`TaxId` là lỗi **do chính bản v0.2 tạo ra** — ghi lại để không lặp: viết tay tên cột trong lúc đang sửa lỗi tên cột là chỗ dễ sai nhất.

**Không tự sửa DOC** — chuyển owner SA.
