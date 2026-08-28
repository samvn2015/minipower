# Bản đồ màn hình UI — 7 module Must (DOC-19)

> Nguồn: `docs/03-modules/*/DOC-19-prototype.md`. Hình: thư mục này. **Chưa baseline.**  
> Visual: brand **HRM**, teal `#0F5C4C`, sand ấm. SSO theo ADR-007.

| Ký hiệu | Ý nghĩa |
|---------|---------|
| ✅ | Có PNG |
| ◐ | Gộp / biến thể gần đúng |

---

## Shell

| Màn | Web | App |
|-----|-----|-----|
| Trang chủ NV | `hrm-web-home.png` ✅ | `hrm-app-home.png` ✅ |

## IAM

| ID | Tên | File |
|----|-----|------|
| IAM-SCR-001 | Login web SSO | `hrm-web-sso-login.png` ✅ |
| IAM-SCR-002 | Login app SSO | `hrm-app-sso-login.png` ✅ |
| IAM-SCR-003 | Quản trị role | `hrm-web-iam-roles.png` ✅ |
| IAM-SCR-004 | Vô hiệu login | `hrm-web-iam-disable.png` ✅ |

## LEV

| ID | Tên | File |
|----|-----|------|
| LEV-SCR-001 | DS đơn của tôi | `hrm-web-leave.png` ✅ |
| LEV-SCR-002 | Form nộp (web) | `hrm-web-leave-form.png` ✅ |
| LEV-SCR-003 | Form nộp (app) | `hrm-app-leave.png` ✅ |
| LEV-SCR-004 | Chi tiết + hủy | `hrm-web-leave-detail.png` · `hrm-app-leave-detail.png` ✅ |
| LEV-SCR-005 | Inbox C1 | `hrm-web-leave-approve.png` · `hrm-app-leave-approve.png` ✅ |
| LEV-SCR-006 | Inbox C2 | `hrm-web-leave-c2.png` ✅ |
| LEV-SCR-007 | Quỹ phép | `hrm-web-leave-balance.png` · `hrm-app-leave-balance.png` ✅ |
| LEV-SCR-008 | Catalog loại phép | `hrm-web-leave-catalog.png` ✅ |

## TIM (HR)

| ID | Tên | File |
|----|-----|------|
| TIM-SCR-001 | DS tháng công | `hrm-web-tim-months.png` ✅ |
| TIM-SCR-002 | Công bố mẫu | `hrm-web-tim-template.png` ✅ |
| TIM-SCR-003 | Import + preview | `hrm-web-tim-import.png` ✅ |
| TIM-SCR-004 | Commit ghi | ◐ `hrm-web-tim-import.png` (nút Ghi trên preview) |
| TIM-SCR-005 | Chốt tháng | `hrm-web-tim-lock.png` ✅ |
| TIM-SCR-006 | Bỏ chốt | `hrm-web-tim-unlock.png` ✅ |
| *(ngoài DOC-19)* | NV chấm công | `hrm-web-timekeeping.png` · `hrm-app-timekeeping.png` ◐ |

## PAY

| ID | Tên | File |
|----|-----|------|
| PAY-SCR-001 | DS kỳ lương | `hrm-web-pay-periods.png` ✅ |
| PAY-SCR-002 | Preview kỳ | `hrm-web-pay-preview.png` ✅ |
| PAY-SCR-003 | Chốt kỳ | ◐ `hrm-web-pay-lock-export.png` |
| PAY-SCR-004 | PC tháng | `hrm-web-pay-allowance.png` ✅ |
| PAY-SCR-005 | Phiếu của tôi (web) | `hrm-web-pay-payslip.png` ✅ |
| PAY-SCR-006 | Phiếu của tôi (app) | `hrm-app-pay-payslip.png` ✅ |
| PAY-SCR-007 | Xuất hàng loạt | ◐ `hrm-web-pay-lock-export.png` |

## EMP

| ID | Tên | File |
|----|-----|------|
| EMP-SCR-001 | DS nhân viên | `hrm-web-emp-list.png` ✅ |
| EMP-SCR-002 | Tạo/sửa HR | `hrm-web-emp-form.png` ✅ |
| EMP-SCR-003 | Hồ sơ của tôi (web) | `hrm-web-emp-me.png` ✅ |
| EMP-SCR-004 | Hồ sơ của tôi (app) | `hrm-app-emp-me.png` ✅ |
| EMP-SCR-005/006 | Đề xuất + duyệt đổi LM | `hrm-web-emp-change-lm.png` ✅ |

## LIF

| ID | Tên | File |
|----|-----|------|
| LIF-SCR-001 | DS on/off | `hrm-web-lif-list.png` ✅ |
| LIF-SCR-002 | Onboarding | `hrm-web-lif-onboarding.png` ✅ |
| LIF-SCR-003 | Ngày LV cuối (N) | `hrm-web-lif-last-day.png` ✅ |
| LIF-SCR-004 | Khóa Git/CRM | `hrm-web-lif-access-lock.png` ✅ |
| LIF-SCR-005 | Offboarding | `hrm-web-lif-offboarding.png` ✅ |
| LIF-SCR-006 | Khóa chat/CR | `hrm-web-lif-chat-lock.png` ✅ |

## PRB

| ID | Tên | File |
|----|-----|------|
| PRB-SCR-001 | Hàng TV / T-15 | `hrm-web-prb-inbox.png` ✅ |
| PRB-SCR-002 | Phiếu T-7 | `hrm-web-prb-eval.png` ✅ |
| PRB-SCR-003 | Chốt kết quả | `hrm-web-prb-finalize.png` ✅ |
| PRB-SCR-004 | Thiếu mốc HĐ | `hrm-web-prb-missing.png` ✅ |

## Ngoài scope

EVT, RPT — chưa vẽ.
