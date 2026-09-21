# Doc-review pass 4 (hẹp) — B1 code + DOC-10/14/16 v0.2 + DOC-13 §3.3 M6 · [2026-09-18]

**Reviewer:** subagent context sạch. **Verdict:** ⛔ BLOCK hẹp — **0 Blocker · 7 Major · 10 Minor**; điều kiện mở baseline = sửa docs cùng ngày + PGD chọn M6. Bản đầy đủ nằm ở transcript agent `ac52ab0c`; dưới đây là bảng rút gọn + trạng thái xử lý (DEC-ARC-036).

| # | Finding | Xử lý |
|---|---|---|
| M1 | Audit LEV/IAM **không cùng transaction** — mỗi repo tự `SaveChanges`; DEC-ARC-034 ghi sai | **Code** PR #60 `IAtomicScope`; rollback chứng minh live. 5 context còn lại → **R-018** |
| M2 | IAM audit **no-op** (gán role đã có, disable tài khoản ma) | **Code** PR #60: repo trả `bool`, audit chỉ khi thay đổi; 404 trước audit |
| M3 | DOC-10/12/16/17 nói **422**; code ném `BadRequestException` = **400**, `BusinessException` trần 0 chỗ | **Docs** sửa 5 chỗ → 400 |
| M4 | DOC-14 còn "5/7", "LEV/IAM nợ" ×4 | **Docs** DOC-14 v0.2.1 |
| M5 | DOC-14 "UAT DEV Pass" vs DOC-16 "chưa chạy" — thật: smoke 09-04 qua `/dev/token`, chưa PGD ký | **Docs** cùng chữ ở cả hai |
| M6 | DOC-13 S08/S10 AC1 tự mâu thuẫn (5×401 trong 5 s đã đủ ngưỡng khoá 5) | **Chờ PGD** chọn (a) sửa AC1 "đã khoá" / (b) S10 = 4 |
| M7 | 0 test mới cho 5/8 handler | **Code** +10 test, 173/173 |
| Minor 1 | `hrm_migrator` UPDATE/DELETE audit; `ALTER DEFAULT PRIVILEGES` thiếu `shared` | DOC-08 ghi; DDL → Dev/DBA nợ |
| Minor 2 | DOC-10 cột `GitLockedAtUtc` gán nhầm `LifAccessLockOutbox` (thuộc `LifOffboardingCase`) | **Docs** sửa |
| Minor 3 | Version tham chiếu lệch (DOC-08 v0.4 vs 0.4.x…); README/doc-registry Draft | README/registry **chưa** — gộp cuối phase |
| Minor 4 | DOC-08 R-004/R-006 stale | **Docs** sửa |
| Minor 5 | DOC-16 "Go-live M6 2027" không nguồn | **Docs** → TBD DOC-15 |
| Minor 6/7 | DOC-13 Thông báo "3 trường hợp" (thật 4), độ trễ không ngưỡng; blocklist SoT file/version | chưa — gộp với M6 khi PGD chọn |
| Minor 8 | DOC-14 thứ tự 1.10.x | **Docs** sửa |
| Minor 9 | JIT-provision tạo tài khoản + gán NV không audit | Dev, gated CR-002 (bỏ theo R-012) |
| Minor 10 | DOC-10 §6 login monitor không đánh dấu chưa code | **Docs** sửa |

**Lớp lỗi lặp (pass 2→4):** cột gán nhầm bảng; **số/tính chất viết tay không đối chiếu code** (422, "cùng transaction"). Quy tắc mới: mã HTTP và transaction phải trích từ code.
