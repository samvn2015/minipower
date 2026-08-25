# Decision Log — Requirements

> ID: `DEC-REQ-NNN`

### DEC-REQ-001 — Vào requirements từ BRD draft; slice đầu = leave/DOC-04 · [2026-08-24]
- Status: accepted *(elicitation — chưa baseline DOC-03)*
- Context: Anh chọn “sẵn sàng requirements / bước tiếp theo”. Cổng 1 (chốt BRD) chưa có chữ ký PGD. Fan-out 9 module sẽ phá token-guard.
- Options: A Chờ ký DOC-03 rồi fan-out DOC-04 mọi module · B STOP · C Mở phase, **một** module `leave`, chỉ DOC-04, ghi nợ chữ ký
- Decision: chọn C
- Why (loại A vì anh đã bảo thực hiện bước tiếp; loại B vì DOC-03 đủ scope Must; loại fan-out vì cổng 1 chưa baseline và slice = 1 DOC)
- Consequences: DOC-04 nháp. Prototype/UC sau khi anh chốt DOC-04. ROI/PC catalog vẫn nợ discovery.
- Affects: leave · DOC-04 · memory/requirements
- Trace: DOC-03 v0.3 · DEC-DIS-001…004
- Confidence: cao

### DEC-REQ-002 — Không fan-out DOC-04 các module khác trong phiên này · [2026-08-24]
- Status: accepted
- Context: In-scope 9 module; approval-gate + token-guard.
- Options: A Fan-out EMP…IAM · B Chỉ LEV
- Decision: chọn B
- Why (loại A vì chưa DEC chốt BRD baseline; một phiên một slice)
- Consequences: EMP/TIM/PAY… chưa có folder DOC-04
- Affects: docs/03-modules
- Trace: DEC-REQ-001
- Confidence: cao

### DEC-REQ-003 — Đóng OQ leave theo tick + mặc định trong ngoặc · [2026-08-24]
- Status: accepted
- Context: Anh tick hết `open-questions.md`, không ghi thêm số ngày / rule hủy.
- Options: A Hỏi lại từng OQ · B Coi tick = chấp nhận ngoặc; 002 = catalog cấu hình; 005 = không hủy MVP
- Decision: chọn B
- Why (loại A vì anh bảo đã cập nhật file; loại bịa số ngày luật vì không có số)
- Consequences: DOC-04 v0.2. Sai nếu anh tick nghĩa là “đã điền chỗ khác” — anh sửa OQ nếu lệch.
- Affects: leave · DOC-04 · OQ-REQ-001…009
- Trace: `memory/requirements/open-questions.md`
- Confidence: vừa *(002/005 là diễn giải)*

### DEC-REQ-004 — QA 3 câu: ngày công liền; hủy rồi nộp lại; mẫu Cty + không trần · [2026-08-24]
- Status: accepted
- Context: Anh trả lời 1C; câu 2 chỉ chốt “hủy xong nộp đơn khác cùng ngày được”; câu 3 mẫu Cty, không trần.
- Options: 1A tổng ngày phép / 1B ngày lịch / **1C ngày công chuẩn liền** · chồng đơn song song vs **hủy rồi nộp lại**
- Decision: 1C. Overlap vẫn chặn đơn Open; Đã hủy giải phóng ngày. File ốm = mẫu Cty. Trần loại trống = không chặn.
- Why (loại 1A/1B vì anh chọn C; loại bịa số trần vì anh bảo không trần)
- Consequences: DOC-04 v0.3 · LEV-BR-007/001/003/013/016. Ai hủy hộ = OQ-REQ-010 (giả định chỉ NV).
- Affects: leave · DOC-04
- Trace: chat 2026-08-24 QA
- Confidence: cao *(010 vừa)*

### DEC-REQ-005 — Chốt DOC-04 leave (cổng Business Rules) · [2026-08-24]
- Status: accepted *(đã chốt — người: Dư Hùng)*
- Context: Anh bảo chốt DOC-04. BRD DOC-03 vẫn draft. OQ-010 còn mở (không chặn).
- Options: A Giữ Draft · B Chốt leave DOC-04 kèm nợ 010 · C Chờ ký DOC-03
- Decision: chọn B
- Why (loại A vì anh chốt; loại C vì cổng BR là per-module, không đợi baseline BRD)
- Consequences: Mở DOC-05 + khung DOC-19. **Không** tự viết SRS. Wireframe HTML hoãn MCP.
- Affects: leave · DOC-04 v0.3 Chốt · DOC-05 · DOC-19
- Trace: DOC-04 · OQ-REQ-010
- Confidence: cao

### DEC-REQ-006 — Chốt Prototype leave (DOC-19) khung text · [2026-08-24]
- Status: accepted *(đã chốt — người: Dư Hùng)*
- Context: Anh chọn “chốt khung” (không chờ HTML).
- Options: A Chốt khung text/mermaid · B Chờ MCP HTML · C Bỏ prototype, nhảy SRS
- Decision: chọn A
- Why (loại B vì HTML hoãn MCP; loại C vì phá A2)
- Consequences: Mở DOC-06. HTML bổ sung sau không đổi field/luồng đã chốt trừ CR.
- Affects: leave · DOC-19 · DOC-06
- Trace: DEC-REQ-005 · DOC-19
- Confidence: cao

### DEC-REQ-007 — Chốt SRS leave (DOC-06) · [2026-08-24]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh bảo chốt DOC-06. FR-001…018 + §6.1 BRQ. OQ-010 / DOC-13 / DOC-07 còn nợ.
- Options: A **Chốt SRS leave kèm nợ** · B Sửa FR · C Chờ DOC-07 AC cùng lúc
- Decision: chọn A
- Why (loại B vì anh không yêu cầu sửa FR; loại C vì cổng 4 = SRS, AC là bước 9)
- Consequences: Mở **DOC-07**. Architecture toàn hệ **sau** khi SRS đủ module hoặc anh cho phép SAD mỏng. HTML MCP không mở lại field. Sửa FR sau = CR.
- Confidence: cao

### DEC-REQ-008 — Viết + chốt DOC-07 AC leave · [2026-08-24]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh: thực hiện luôn viết + chốt AC cùng slice phép.
- Options: A Draft AC chờ duyệt riêng · B **Viết LEV-AC-001…018 và chốt ngay**
- Decision: chọn B
- Why (loại A vì anh bảo thực hiện luôn)
- Consequences: Cổng AC leave đóng. DOC-16/test sau. Architecture không tự mở. Sửa AC = CR.
- Confidence: cao

### DEC-REQ-009 — Chốt DOC-05 UC leave · [2026-08-24]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt DOC-05. UC-005/006 bổ sung dressed; BA Trịnh Yên.
- Options: A Giữ Draft Casual 005/006 · B **Chốt v0.2 kèm nợ OQ-010 / ACT-004**
- Decision: chọn B
- Why (loại A vì anh chốt cổng UC)
- Consequences: Slice leave 04–07 + 19 khung đủ. Sửa UC = CR.
- Confidence: cao

### DEC-REQ-010 — Mở slice payroll; chỉ DOC-04 Draft · [2026-08-24]
- Status: accepted
- Context: Anh: tạo file DOC-04 payroll.
- Options: A Fan-out 05–07 luôn · B **Chỉ PAY DOC-04**
- Decision: chọn B
- Why (loại A vì một phiên một DOC; cổng BR chưa chốt)
- Consequences: Folder `payroll/`. Không tự Prototype/SRS. Leave đứng.
- Confidence: cao

### DEC-REQ-011 — Chốt DOC-04 payroll (cổng BR) · [2026-08-24]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh: không bổ sung thì chốt. BRQ lương trên BRD đã có PAY-BR-001…012; PC/BH động; làm tròn → DOC-07.
- Options: A Thêm BR (tạm ứng, lương CB tách dòng, …) · B **Chốt v0.1 kèm nợ DOC-07/master**
- Decision: chọn B
- Why (loại A vì BRD không bắt tạm ứng; không bịa công thức tổng)
- Consequences: Mở DOC-05 + khung DOC-19 payroll. **Không** tự viết. Sửa BR = CR.
- Affects: payroll · DOC-04
- Trace: `docs/03-modules/payroll/DOC-04-business-rules.md`
- Confidence: cao
