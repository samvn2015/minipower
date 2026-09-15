# Xác minh OQ-ARC-011 — schema + DB role có đủ cho NFR-002? · [2026-09-07]

**Câu hỏi:** ADR-011 W1 giả định *tách schema + DB role theo bounded context trong **một** instance PostgreSQL* đủ để dựng hàng rào NFR-002 dưới tầng ứng dụng. Giả định AS-04 của [deliberation B1](../../brainstorm/2026-09-07-arc-b1-monolith-vs-microservices.md), tin cậy **vừa** — chưa ai kiểm.

**Kết luận: ✅ ĐỦ.** 8/8 case đúng kỳ vọng. Kèm **2 phát hiện** cần đưa vào thiết kế W1.

---

## Cách kiểm

PostgreSQL **16.8** (Postgres.app) — cùng engine với DEV, cùng major version với Prod (ADR-009 yêu cầu 16+).

Database thử nghiệm **riêng** `hrm_oq011`, hai schema `pay` / `lev`, hai role đăng nhập `hrm_pay` / `hrm_lev`. **Không đụng database `hrm` thật.** Đã dọn sạch sau khi đo — xác nhận chỉ còn `hrm`, không còn role `hrm_*`.

Siết quyền theo đúng mô hình W1:

```sql
REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON DATABASE hrm_oq011 FROM PUBLIC;
GRANT CONNECT ON DATABASE hrm_oq011 TO hrm_pay, hrm_lev;
GRANT USAGE ON SCHEMA pay TO hrm_pay;
GRANT SELECT,INSERT,UPDATE,DELETE ON ALL TABLES IN SCHEMA pay TO hrm_pay;
-- tương tự cho lev / hrm_lev
```

## Kết quả

| # | Role | Hành vi | Kỳ vọng | Thực tế |
|---|------|---------|---------|---------|
| 1 | `hrm_lev` | đọc `lev.leave_request` | cho | ✅ OK |
| 2 | `hrm_lev` | đọc `pay.pay_line` | **chặn** | ✅ `permission denied for schema pay` |
| 3 | `hrm_lev` | **JOIN chéo** `lev ↔ pay` | **chặn** | ✅ `permission denied for schema pay` |
| 4 | `hrm_lev` | ghi vào `pay.pay_line` | **chặn** | ✅ denied |
| 5 | `hrm_pay` | đọc `pay.pay_line` | cho | ✅ OK |
| 6 | `hrm_pay` | đọc `lev.leave_request` | **chặn** | ✅ denied |
| 7 | `hrm_pay` | **tự GRANT** quyền trên `lev` cho mình | **chặn** | ✅ denied — không leo thang được |
| 8 | `hrm_pay` | tạo bảng trong `lev` | **chặn** | ✅ denied |

Case 3 và 7 là quan trọng nhất: **join chéo bị chặn ở tầng DB**, và role **không tự nâng quyền** được.

---

## Phát hiện 1 — role ứng dụng KHÔNG migrate được nếu chỉ có USAGE

`hrm_lev` **không** `CREATE TABLE` được ngay trong schema của chính nó:

```
hrm_lev → CREATE TABLE lev.mig_test(id int)
        → permission denied for schema lev
```

Cần thêm `GRANT CREATE ON SCHEMA lev TO hrm_lev`. Sau khi cấp, đã kiểm lại:

- ✅ migrate được trong `lev`
- ✅ **vẫn** không đọc được `pay`
- ✅ **vẫn** không tạo bảng trong `pay`

→ Cấp `CREATE` **không** làm thủng hàng rào. Nhưng vẫn nên tách vai:

| Phương án | Đánh đổi |
|---|---|
| **(i)** App role có `CREATE` trên schema mình | Đơn giản, `AutoMigrate` chạy như hiện tại. Role ứng dụng có quyền DDL |
| **(ii)** Role **migrator riêng** mỗi context, app role chỉ DML | Chặt hơn, khớp DOC-17 *"Prod: pipeline migrate riêng"*. Thêm N connection string |

DOC-17 §2.1 đã ghi `AutoMigrate` **chỉ DEV**, Prod dùng pipeline riêng → **(ii) khớp hướng đã chốt**. Quyết ở W1.

## Phát hiện 2 — rò rỉ metadata qua `pg_catalog`

`hrm_lev` **thấy tên bảng** của schema `pay`:

```
pg_class → pay_line, pay_line_pkey
```

Nhưng:

- ❌ **không** đọc được dữ liệu
- ❌ **không** thấy tên cột (`information_schema.columns` trả rỗng)
- ✅ `information_schema.tables` lọc đúng theo quyền → trả 0 dòng

`pg_catalog` mở cho mọi role là hành vi chuẩn của PostgreSQL, không tắt được bằng GRANT. **Không vi phạm NFR-002** — NFR-002 nói về *con số lương* và *phiếu lương*, không phải tên bảng. Ghi nhận để không bất ngờ khi audit.

---

## Phát hiện 3 — role `admin` hiện không tạo được role

```
admin  → rolsuper=false, rolcreaterole=false, rolcreatedb=true
```

Nghĩa là W1 **cần DBA/superuser** để provision schema + role, không tự làm bằng tài khoản ứng dụng. Trên Prod phải đưa vào yêu cầu gửi IT/DBA cùng lúc với `OQ-DLV-003` (host/user/password Prod).

---

## Bổ sung cho tiêu chí ra của W1

Tiêu chí gốc trong ADR-011 giữ nguyên, thêm hai mục:

- [ ] Chốt phương án migrate: **(i)** app role có `CREATE`, hay **(ii)** migrator role riêng *(nghiêng (ii) — khớp DOC-17)*
- [ ] Yêu cầu DBA provision schema + role Prod, gộp vào `OQ-DLV-003`

ADR-011 đang **Accepted** nên **không sửa** — hai mục này là chi tiết hiện thực, ghi ở đây và ở `DEC-ARC-023`.
