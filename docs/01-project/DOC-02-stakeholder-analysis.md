# DOC-02 — Phân tích stakeholder

| Phiên bản | Ngày | Tác giả | Trạng thái |
|-----------|------|---------|------------|
| 0.2 | 2026-08-24 | Trịnh Yên (BA) | **Chốt** (DEC-DIS-006 + **012**) |

---

## 1. Mục đích

Stakeholder HRM nội bộ mInvoice (không phải CRM khách hàng).

**Cổng:** DOC-02 **đã chốt** (PGD) — DEC-DIS-006. **DEC-DIS-012:** BA = **Trịnh Yên** (SH-009); PGD **không** kiêm BA. **Chưa** `02-baseline/`. A **BRD** = ký **DOC-03**. Ban NS chưa ký dòng Owner.

## 2. Đăng ký stakeholder

| ID | Stakeholder | Vai trò | Quyền lợi | Ảnh hưởng | Quan tâm | Chiến lược |
|----|-------------|---------|-----------|-----------|----------|------------|
| SH-001 | **Mr. Dư Hùng, PGD** | A ký BRD (không kiêm BA) | Phạm vi, baseline | H | H | Manage closely |
| SH-002 | Ban Nhân sự / C&B | Business owner | Công, lương, phép, on/off | H | H | Manage closely |
| SH-003 | Ban QT vận hành | Đồng yêu cầu URD | Vận hành, cảnh báo | M | H | Manage closely |
| SH-004 | Nhân viên | End-user | Self-service web+mobile | L | H | Keep informed |
| SH-005 | Line Manager | Duyệt phép C1, đánh giá TV | Lịch phòng, thử việc | M | H | Keep informed |
| SH-006 | IT Admin | Email, máy, Git/CRM, CC | Checklist LIF | M | H | Manage closely |
| SH-007 | Kế toán | Quyết toán lương cuối / tạm ứng | Offboarding | M | M | Keep satisfied |
| SH-008 | BGĐ | Báo cáo turnover, quỹ lương | URD-08 | H | M | Keep satisfied |
| SH-009 | **Trịnh Yên** | BA (R soạn DOC) | URD/BRD/BR đúng | M | H | Keep informed |
| SH-010 | Dev / Tester | Xây + UAT kỹ thuật | Hệ thống đúng spec | M | H | Keep informed |

## 3. Bản đồ

| Quadrant | IDs |
|----------|-----|
| Manage closely | SH-001, SH-002, SH-003, SH-006 |
| Keep satisfied | SH-007, SH-008 |
| Keep informed | SH-004, SH-005, SH-009, SH-010 |
| Monitor | — |

## 4. RACI (sơ bộ)

| Deliverable | SH-001 | SH-002 | SH-006 | SH-009 | SH-010 |
|-------------|--------|--------|--------|--------|--------|
| Approve BRD | A | C | I | R | I |
| Công thức lương / 85% | A | C | I | R | I |
| Mẫu Excel công | I | A | C | R | I |
| N+3 khóa Git/CRM | A | C | R | C | C |
| UAT lương 0 đồng | I | A | I | C | R |

**Lưu ý:** SH-001 = **A** (PGD). SH-009 = **R soạn** (Trịnh Yên). Không gộp A+BA một người (DEC-DIS-012).

## 5. Truyền thông

| Stakeholder | Nội dung | Tần suất | Kênh | Owner |
|-------------|----------|----------|------|-------|
| SH-001, 002 | Scope, BR lương | Gate discovery | `docs/` | SH-009 |
| SH-004, 005 | Phép, mobile | Sprint | HRM / email | SH-002 |

## 6. Giả định

| ID | Giả định |
|----|----------|
| A-SH-01 | A BRD = Mr. Dư Hùng, PGD. |
| A-SH-02 | URD-02 **không** bắn thông báo sang CRM bán hàng (anh xác nhận không có chữ CRM trên URD). |
| A-SH-03 | BA dự án = **Trịnh Yên** (DEC-DIS-012). |

## 7. Phê duyệt

| Vai trò | Họ tên | Chữ ký | Ngày |
|---------|--------|--------|------|
| Sponsor / A (stakeholder) | Mr. Dư Hùng, PGD | **Chốt** | 2026-08-24 |
| Business Owner HR | Ban Nhân sự mInvoice | | ☐ |
| BA (R soạn) | Trịnh Yên | R — DEC-DIS-012 | 2026-08-24 |
