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

### DEC-REQ-012 — Chốt DOC-05 UC payroll · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt payroll DOC-05. PAY-UC-001…005 dressed; PAY-BR-001…012 có UC; BA Trịnh Yên.
- Options: A Giữ Draft · B **Chốt v0.1 kèm nợ DOC-19/06/07 / Ban HR / hủy chốt kỳ ngoài MVP**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Mở khung DOC-19 payroll. **Không** tự viết Prototype/SRS. Sửa UC = CR.
- Affects: payroll · DOC-05
- Trace: `docs/03-modules/payroll/DOC-05-use-cases.md`
- Confidence: cao

### DEC-REQ-013 — Chốt Prototype payroll (DOC-19) khung text · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chọn “chốt khung” (không chờ HTML). PAY-SCR-001…007.
- Options: A Chốt khung text/mermaid · B Chờ MCP HTML · C Bỏ prototype, nhảy SRS
- Decision: chọn A
- Why (loại B vì HTML hoãn MCP; loại C vì phá A2)
- Consequences: Mở DOC-06. HTML bổ sung sau không đổi field/luồng đã chốt trừ CR. **Không** tự viết SRS.
- Affects: payroll · DOC-19 · DOC-06
- Trace: `docs/03-modules/payroll/DOC-19-prototype.md`
- Confidence: cao

### DEC-REQ-014 — Chốt SRS payroll (DOC-06) · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt DOC-06. PAY-FR-001…018 + §6.1 BRQ. DOC-07 / DOC-13 còn nợ.
- Options: A **Chốt SRS payroll kèm nợ** · B Sửa FR · C Chờ DOC-07 AC cùng lúc
- Decision: chọn A
- Why (loại B vì anh không yêu cầu sửa FR; loại C vì cổng 4 = SRS, AC là bước 9)
- Consequences: Mở **DOC-07**. Architecture không tự mở. HTML MCP không đổi field. Sửa FR sau = CR. **Không** tự viết AC.
- Affects: payroll · DOC-06 · DOC-07
- Trace: `docs/03-modules/payroll/DOC-06-srs.md`
- Confidence: cao

### DEC-REQ-015 — Chốt DOC-07 AC payroll · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt AC. PAY-AC-001…018 Gherkin; làm tròn = master kỳ UAT.
- Options: A Giữ Draft · B **Chốt v0.1 kèm nợ DOC-13 / DOC-16 / làm tròn master**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Cổng AC payroll đóng. DOC-16/test sau. Architecture không tự mở. Sửa AC = CR.
- Affects: payroll · DOC-07
- Trace: `docs/03-modules/payroll/DOC-07-acceptance-criteria.md`
- Confidence: cao

### DEC-REQ-016 — Mở slice timekeeping; chỉ DOC-04 Draft · [2026-08-25]
- Status: accepted
- Context: Anh chọn timekeeping DOC-04 (BRQ-004) sau khi payroll 04–07+19 khung Chốt.
- Options: A Fan-out 05–07 luôn · B **Chỉ TIM DOC-04** · C DOC-16 payroll
- Decision: chọn B
- Why (loại A vì một phiên một DOC; cổng BR chưa chốt; loại C vì anh chọn TIM)
- Consequences: Folder `timekeeping/`. Payroll/leave đứng. **Không** tự Prototype/SRS. CRM PARKED.
- Affects: timekeeping · DOC-04
- Trace: `docs/03-modules/timekeeping/DOC-04-business-rules.md`
- Confidence: cao

### DEC-REQ-017 — Chốt DOC-04 timekeeping (cổng BR) · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt BR. TIM-BR-001…012; cột Excel không đóng trên BR; xem công NV không Must.
- Options: A Thêm BR (NV xem công, API máy CC, …) · B **Chốt v0.1 kèm nợ master cột / Ban HR**
- Decision: chọn B
- Why (loại A vì BRD 4.2 không máy CC; BRQ-006 không liệt kê xem công)
- Consequences: Mở DOC-05 + khung DOC-19 timekeeping. **Không** tự viết. Sửa BR = CR. Payroll/leave đứng.
- Affects: timekeeping · DOC-04
- Trace: `docs/03-modules/timekeeping/DOC-04-business-rules.md`
- Confidence: cao

### DEC-REQ-018 — Chốt DOC-05 UC timekeeping · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt (gõ “chót”). TIM-UC-001…005 dressed; TIM-BR-001…012 có UC; không UC xem công NV.
- Options: A Giữ Draft · B **Chốt v0.1 kèm nợ DOC-19/06/07 / Ban HR**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Mở khung DOC-19 timekeeping. **Không** tự viết Prototype/SRS. Sửa UC = CR.
- Affects: timekeeping · DOC-05
- Trace: `docs/03-modules/timekeeping/DOC-05-use-cases.md`
- Confidence: cao

### DEC-REQ-019 — Chốt Prototype timekeeping (DOC-19) khung text · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chọn chốt khung (không chờ HTML). TIM-SCR-001…006.
- Options: A Chốt khung text/mermaid · B Chờ MCP HTML · C Bỏ prototype, nhảy SRS
- Decision: chọn A
- Why (loại B vì HTML hoãn MCP; loại C vì phá A2)
- Consequences: Mở DOC-06. HTML sau không đổi field/luồng đã chốt trừ CR. **Không** tự viết SRS.
- Affects: timekeeping · DOC-19 · DOC-06
- Trace: `docs/03-modules/timekeeping/DOC-19-prototype.md`
- Confidence: cao

### DEC-REQ-020 — Chốt SRS timekeeping (DOC-06) · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt DOC-06. TIM-FR-001…016 + §6.1 BRQ. DOC-07 / DOC-13 còn nợ.
- Options: A **Chốt SRS TIM kèm nợ** · B Sửa FR · C Chờ DOC-07 AC cùng lúc
- Decision: chọn A
- Why (loại B vì anh không yêu cầu sửa FR; loại C vì cổng 4 = SRS)
- Consequences: Mở **DOC-07**. Architecture không tự mở. Sửa FR sau = CR. **Không** tự viết AC.
- Affects: timekeeping · DOC-06 · DOC-07
- Trace: `docs/03-modules/timekeeping/DOC-06-srs.md`
- Confidence: cao

### DEC-REQ-021 — Chốt DOC-07 AC timekeeping · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt AC. TIM-AC-001…016 Gherkin; mã version file = nợ kỹ thuật.
- Options: A Giữ Draft · B **Chốt v0.1 kèm nợ DOC-13 / DOC-16 / version file**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Cổng AC timekeeping đóng. DOC-16/test sau. Architecture không tự mở. Sửa AC = CR.
- Affects: timekeeping · DOC-07
- Trace: `docs/03-modules/timekeeping/DOC-07-acceptance-criteria.md`
- Confidence: cao

### DEC-REQ-022 — Fan-out EMP + LIF; chỉ DOC-04 Draft · [2026-08-25]
- Status: accepted
- Context: Anh: làm cả hai (EMP và LIF) sau TIM AC Chốt. Cổng BRD DOC-03 đã chốt.
- Options: A Fan-out 05–07 luôn · B **Chỉ DOC-04 hai module** · C Một module
- Decision: chọn B
- Why (loại A vì một artifact BR; loại C vì anh chọn cả hai)
- Consequences: `employee-profile/` + `lifecycle/`. **Không** tự 05/19/06. Leave/PAY/TIM đứng. Không sửa trace-matrix (chưa rollup).
- Affects: EMP · LIF · DOC-04
- Trace: `docs/03-modules/employee-profile/DOC-04-business-rules.md` · `docs/03-modules/lifecycle/DOC-04-business-rules.md`
- Confidence: cao

### DEC-REQ-023 — Chốt DOC-04 employee-profile (cổng BR) · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt cả hai. EMP-BR-001…012; unique + đổi LM + hồ sơ mobile.
- Options: A Sửa unique/tái tuyển · B **Chốt v0.1 kèm nợ field master / Ban HR**
- Decision: chọn B
- Why (loại A vì anh không yêu cầu sửa)
- Consequences: Mở EMP DOC-05 + khung 19. **Không** tự viết. Sửa BR = CR.
- Affects: employee-profile · DOC-04
- Trace: `docs/03-modules/employee-profile/DOC-04-business-rules.md`
- Confidence: cao

### DEC-REQ-024 — Chốt DOC-04 lifecycle (cổng BR) · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt cả hai. LIF-BR-001…012; N+3 Git/CRM SP; N = ngày LV cuối.
- Options: A Chờ chốt N+3 lịch vs ngày công · B **Chốt v0.1 kèm nợ: N+3 = ngày lịch (nháp)**
- Decision: chọn B
- Why (loại A vì anh chốt cả hai không trả TBD; nháp lịch ghi nợ DOC-05)
- Consequences: Mở LIF DOC-05 + khung 19. **Không** tự viết. Sai nếu N+3 phải là ngày công — anh sửa LIF-BR-002.
- Affects: lifecycle · DOC-04
- Trace: `docs/03-modules/lifecycle/DOC-04-business-rules.md`
- Confidence: vừa *(N+3 đơn vị ngày)*

### DEC-REQ-025 — Fan-out EMP + LIF DOC-05 UC Draft · [2026-08-25]
- Status: accepted *(superseded by DEC-REQ-026 / 027 chốt)*
- Context: Anh: soạn cả 2 (UC) sau BR Chốt.
- Options: A Chỉ một module · B **Hai DOC-05 Draft** · C Kèm khung 19
- Decision: chọn B
- Why (loại A vì anh chọn cả hai; loại C vì cổng A2: chốt UC rồi mới prototype)
- Consequences: EMP-UC-001…005 · LIF-UC-001…005 dressed. N+3 = ngày lịch nháp trên LIF-UC-003. Đổi LM = **một bậc** (không Matrix). **Không** tự 19/06. Leave/PAY/TIM đứng.
- Affects: EMP · LIF · DOC-05
- Trace: `docs/03-modules/employee-profile/DOC-05-use-cases.md` · `docs/03-modules/lifecycle/DOC-05-use-cases.md`
- Confidence: cao

### DEC-REQ-026 — Chốt DOC-05 UC employee-profile · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt cả hai. EMP-UC-001…005; đổi LM một bậc; không mở phiếu lương.
- Options: A Giữ Draft · B **Chốt v0.1 kèm nợ catalog field / DOC-19 / Ban HR**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Mở khung DOC-19 EMP. **Không** tự viết Prototype/SRS. Sửa UC = CR.
- Affects: employee-profile · DOC-05
- Trace: `docs/03-modules/employee-profile/DOC-05-use-cases.md`
- Confidence: cao

### DEC-REQ-027 — Chốt DOC-05 UC lifecycle · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt cả hai. LIF-UC-001…005; N+3 ngày lịch nháp; không notify CRM sales.
- Options: A Chờ chốt N+3 ngày công · B **Chốt v0.1 kèm nợ N+3 lịch / DOC-19 / Ban HR**
- Decision: chọn B
- Why (loại A vì anh chốt không đổi đơn vị ngày)
- Consequences: Mở khung DOC-19 LIF. **Không** tự viết. Sai nếu N+3 phải là ngày công — anh sửa LIF-UC-003 / LIF-BR-002.
- Affects: lifecycle · DOC-05
- Trace: `docs/03-modules/lifecycle/DOC-05-use-cases.md`
- Confidence: vừa *(N+3 đơn vị ngày)*

### DEC-REQ-028 — Fan-out EMP + LIF DOC-19 khung Draft · [2026-08-25]
- Status: accepted *(superseded by DEC-REQ-029 / 030 chốt khung)*
- Context: Anh: cả 2 (prototype) sau UC Chốt. Cùng kiểu TIM/PAY: text/mermaid, HTML hoãn.
- Options: A Một module · B **Hai DOC-19 Draft khung** · C Chờ HTML MCP
- Decision: chọn B
- Why (loại A vì anh chọn cả hai; loại C vì HTML không chặn khung)
- Consequences: EMP-SCR-001…006 · LIF-SCR-001…006. **Không** tự SRS. Không màn CRM sales / phiếu lương.
- Affects: EMP · LIF · DOC-19
- Trace: `docs/03-modules/employee-profile/DOC-19-prototype.md` · `docs/03-modules/lifecycle/DOC-19-prototype.md`
- Confidence: cao

### DEC-REQ-029 — Chốt Prototype employee-profile (DOC-19) khung text · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh: chốt cả 3 (bộ 04+05+19; 04/05 đã chốt). EMP-SCR-001…006.
- Options: A Giữ Draft · B **Chốt khung text/mermaid** · C Chờ HTML MCP
- Decision: chọn B
- Why (loại A vì anh chốt; loại C vì HTML hoãn, không chặn SRS)
- Consequences: Mở EMP DOC-06. HTML sau không đổi field/luồng đã chốt trừ CR. **Không** tự viết SRS.
- Affects: employee-profile · DOC-19 · DOC-06
- Trace: `docs/03-modules/employee-profile/DOC-19-prototype.md`
- Confidence: cao

### DEC-REQ-030 — Chốt Prototype lifecycle (DOC-19) khung text · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh: chốt cả 3. LIF-SCR-001…006; N+3 ngày lịch nháp trên UI.
- Options: A Giữ Draft · B **Chốt khung** · C Chờ HTML
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Mở LIF DOC-06. **Không** tự viết SRS.
- Affects: lifecycle · DOC-19 · DOC-06
- Trace: `docs/03-modules/lifecycle/DOC-19-prototype.md`
- Confidence: vừa *(N+3 đơn vị ngày)*

### DEC-REQ-031 — Fan-out EMP + LIF DOC-06 SRS Draft · [2026-08-25]
- Status: proposed *(chờ PGD chốt)*
- Context: Anh: tạo DOC-06 (hai module còn thiếu SRS sau 19 Chốt).
- Options: A Một module · B **Hai DOC-06 Draft** · C Kèm DOC-07
- Decision: chọn B
- Why (loại A vì slice đang EMP+LIF; loại C vì cổng SRS trước AC)
- Consequences: EMP-FR-001…016 · LIF-FR-001…016. **Không** tự AC. N+3 lịch nháp trên LIF-FR-005.
- Affects: EMP · LIF · DOC-06
- Trace: `docs/03-modules/employee-profile/DOC-06-srs.md` · `docs/03-modules/lifecycle/DOC-06-srs.md`
- Confidence: cao

### DEC-REQ-032 — EMP DOC-06: FR trình độ học vấn · [2026-08-25]
- Status: accepted *(PGD yêu cầu bổ sung trên Draft SRS)*
- Context: Anh: DOC-06 EMP chưa thấy trình độ học vấn. BRD/DOC-04 không đặt tên field (EMP-BR-012 động).
- Options: A Chỉ nằm trong master ẩn · B **EMP-FR-017 Must, bậc = master**
- Decision: chọn B
- Why (loại A vì anh cần thấy trên SRS)
- Consequences: DOC-06 EMP Draft thêm FR-017. DOC-04/05/19 đã chốt không đổi trừ CR. List bậc học không đóng trên SRS.
- Affects: employee-profile · DOC-06
- Trace: `docs/03-modules/employee-profile/DOC-06-srs.md`
- Confidence: cao

### DEC-REQ-033 — Chốt SRS employee-profile (DOC-06) · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt DOC-06 kèm FR-017. EMP-FR-001…017.
- Options: A Giữ Draft · B **Chốt v0.1 kèm nợ DOC-07 / master bậc học / Ban HR** · C Chốt luôn LIF-06
- Decision: chọn B
- Why (loại A vì anh chốt câu EMP; loại C vì selection chỉ EMP + FR-017)
- Consequences: Mở **EMP DOC-07**. LIF DOC-06 còn Draft. Sửa FR sau = CR. **Không** tự AC.
- Affects: employee-profile · DOC-06 · DOC-07
- Trace: `docs/03-modules/employee-profile/DOC-06-srs.md`
- Confidence: cao

### DEC-REQ-034 — Chốt SRS lifecycle (DOC-06) · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt LIF DOC-06. LIF-FR-001…016; N+3 ngày lịch nháp.
- Options: A Giữ Draft · B **Chốt v0.1 kèm nợ DOC-07 / N+3 lịch / Ban HR**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Mở **LIF DOC-07**. Sửa FR sau = CR. **Không** tự AC.
- Affects: lifecycle · DOC-06 · DOC-07
- Trace: `docs/03-modules/lifecycle/DOC-06-srs.md`
- Confidence: vừa *(N+3 đơn vị ngày)*

### DEC-REQ-035 — Fan-out EMP + LIF DOC-07 AC Draft · [2026-08-25]
- Status: accepted *(superseded by DEC-REQ-036 / 037 chốt)*
- Context: Anh: cả hai (AC) sau SRS Chốt.
- Options: A Một module · B **Hai DOC-07 Draft** · C Kèm DOC-16
- Decision: chọn B
- Why (loại A vì anh chọn cả hai; loại C vì DOC-16 delivery)
- Consequences: EMP-AC-001…017 · LIF-AC-001…016 (011 Should). **Không** tự DOC-16/13. Architecture không mở.
- Affects: EMP · LIF · DOC-07
- Trace: `docs/03-modules/employee-profile/DOC-07-acceptance-criteria.md` · `docs/03-modules/lifecycle/DOC-07-acceptance-criteria.md`
- Confidence: cao

### DEC-REQ-036 — Chốt DOC-07 AC employee-profile · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh: chốt cả 2 (gõ “chốt ca2 2”). EMP-AC-001…017.
- Options: A Giữ Draft · B **Chốt v0.1 kèm nợ DOC-13 / DOC-16 / Ban HR**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Cổng AC EMP đóng. Sửa AC = CR. **Không** tự DOC-13/16 / architecture.
- Affects: employee-profile · DOC-07
- Trace: `docs/03-modules/employee-profile/DOC-07-acceptance-criteria.md`
- Confidence: cao

### DEC-REQ-037 — Chốt DOC-07 AC lifecycle · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt cả hai. LIF-AC-001…016; N+3 lịch nháp trên AC-005/013.
- Options: A Giữ Draft · B **Chốt v0.1 kèm nợ N+3 / DOC-13 / DOC-16**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Cổng AC LIF đóng. Sửa AC = CR. **Không** tự DOC-13/16.
- Affects: lifecycle · DOC-07
- Trace: `docs/03-modules/lifecycle/DOC-07-acceptance-criteria.md`
- Confidence: vừa *(N+3 đơn vị ngày)*

### DEC-REQ-038 — Chốt DOC-13 NFR platform · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt DOC-13 khi file chưa có. Soạn từ DOC-03 CN-002/BRQ-009 + AC-NFR module. Không bịa SLA 99,9%.
- Options: A Draft chờ số liệu IT · B **Chốt v0.1 kèm nợ SLA/RTO/crypto DOC-08**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: `docs/04-platform/DOC-13-nfr.md`. Sửa NFR = CR. **Không** tự SAD/DOC-16. NFR-012 TBD.
- Affects: platform · DOC-13
- Trace: `docs/04-platform/DOC-13-nfr.md`
- Confidence: vừa *(SLA chưa có trên BRD)*

### DEC-REQ-039 — Mở identity DOC-04 Draft · [2026-08-25]
- Status: accepted *(superseded by DEC-REQ-041 chốt)*
- Context: Anh chọn DOC-04 identity sau DOC-13 Chốt.
- Options: A Fan-out 05–07 · B **Chỉ DOC-04 Draft**
- Decision: chọn B
- Why (loại A vì một artifact BR)
- Consequences: `docs/03-modules/identity/`. IAM-BR-001…012. **Không** tự 05/19/06. SSO/MFA không Must trên BR.
- Affects: identity · DOC-04
- Trace: `docs/03-modules/identity/DOC-04-business-rules.md`
- Confidence: cao

### DEC-REQ-040 — Bổ sung IAM DOC-04 v0.2 (đủ ma trận) · [2026-08-25]
- Status: accepted *(superseded by DEC-REQ-041 chốt)*
- Context: Anh: cập nhật đủ DOC-04 (slice identity Draft). URD bảng III không có trong docs.
- Options: A Giữ 001…012 · B **Ma trận distill LEV/PAY/TIM/EMP/LIF + BR-013…017**
- Decision: chọn B
- Why (loại A vì anh yêu cầu đủ)
- Consequences: 5 role MVP; PGD không mặc định lương Cty. PRB/EVT/RPT = gạch. **Không** tự 05.
- Affects: identity · DOC-04
- Trace: `docs/03-modules/identity/DOC-04-business-rules.md`
- Confidence: cao

### DEC-REQ-041 — Chốt DOC-04 identity (cổng BR) · [2026-08-25]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt. IAM-BR-001…017 + ma trận §4 v0.2.
- Options: A Giữ Draft · B **Chốt v0.2 kèm nợ PRB/EVT/RPT / Ban HR**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Mở IAM DOC-05. Sửa BR = CR. **Không** tự UC.
- Affects: identity · DOC-04
- Trace: `docs/03-modules/identity/DOC-04-business-rules.md`
- Confidence: cao

### DEC-REQ-042 — Mở identity DOC-05 UC Draft · [2026-08-25]
- Status: accepted *(superseded by DEC-REQ-043)*
- Context: Anh: soạn DOC-05 sau IAM BR Chốt.
- Options: A Kèm khung 19 · B **Chỉ DOC-05 Draft**
- Decision: chọn B
- Why (loại A vì cổng A2)
- Consequences: IAM-UC-001…005 dressed. **Không** tự 19/06.
- Affects: identity · DOC-05
- Trace: `docs/03-modules/identity/DOC-05-use-cases.md`
- Confidence: cao

### DEC-REQ-043 — Chốt DOC-05 UC identity v0.2 · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh: điều chỉnh rồi chốt. Siết UC-003 (negative, không mọi API); hết phiên → UC-001; UC-004 chỉ disable login; UC-005 chỉ CRM sales.
- Options: A Chốt v0.1 · B **Chốt v0.2 đã siết**
- Decision: chọn B
- Why (loại A vì anh yêu cầu điều chỉnh)
- Consequences: Mở khung DOC-19 IAM. Sửa UC = CR. **Không** tự prototype/SRS.
- Affects: identity · DOC-05
- Trace: `docs/03-modules/identity/DOC-05-use-cases.md`
- Confidence: cao

### DEC-REQ-044 — Mở identity DOC-19 khung Draft · [2026-08-26]
- Status: accepted *(superseded by DEC-REQ-045 chốt khung)*
- Context: Anh: soạn (prototype IAM) sau UC Chốt.
- Options: A Chờ HTML · B **Draft khung text/mermaid**
- Decision: chọn B
- Why (loại A vì HTML hoãn)
- Consequences: IAM-SCR-001…004. Không màn Git/CRM sales. **Không** tự SRS.
- Affects: identity · DOC-19
- Trace: `docs/03-modules/identity/DOC-19-prototype.md`
- Confidence: cao

### DEC-REQ-045 — Chốt Prototype identity (DOC-19) khung text · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt khung. IAM-SCR-001…004.
- Options: A Giữ Draft · B **Chốt khung text/mermaid** · C Chờ HTML
- Decision: chọn B
- Why (loại A vì anh chốt; loại C vì HTML không chặn SRS)
- Consequences: Mở IAM DOC-06. HTML sau không đổi luồng trừ CR. **Không** tự SRS.
- Affects: identity · DOC-19 · DOC-06
- Trace: `docs/03-modules/identity/DOC-19-prototype.md`
- Confidence: cao

### DEC-REQ-046 — Mở identity DOC-06 SRS Draft · [2026-08-26]
- Status: accepted *(superseded by DEC-REQ-047 chốt)*
- Context: Anh: soạn DOC-06 sau khung 19 Chốt.
- Options: A Kèm DOC-07 · B **Chỉ DOC-06 Draft**
- Decision: chọn B
- Why (loại A vì cổng SRS trước AC)
- Consequences: IAM-FR-001…019. **Không** tự AC.
- Affects: identity · DOC-06
- Trace: `docs/03-modules/identity/DOC-06-srs.md`
- Confidence: cao

### DEC-REQ-047 — Chốt SRS identity (DOC-06) · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt DOC-06. IAM-FR-001…019.
- Options: A Giữ Draft · B **Chốt v0.1 kèm nợ DOC-07 / SSO DOC-08 / Ban HR**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Mở **IAM DOC-07**. Sửa FR = CR. **Không** tự AC.
- Affects: identity · DOC-06 · DOC-07
- Trace: `docs/03-modules/identity/DOC-06-srs.md`
- Confidence: cao

### DEC-REQ-048 — Mở identity DOC-07 AC Draft · [2026-08-26]
- Status: accepted *(superseded by DEC-REQ-049 chốt)*
- Context: Anh: soạn AC identity.
- Options: A Kèm DOC-16 · B **Chỉ DOC-07 Draft**
- Decision: chọn B
- Why (loại A vì delivery)
- Consequences: IAM-AC-001…019. **Không** tự DOC-16.
- Affects: identity · DOC-07
- Trace: `docs/03-modules/identity/DOC-07-acceptance-criteria.md`
- Confidence: cao

### DEC-REQ-049 — Chốt DOC-07 AC identity · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt AC. IAM-AC-001…019.
- Options: A Giữ Draft · B **Chốt v0.1 kèm nợ DOC-16 / SSO / Ban HR**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Cổng AC IAM đóng. Sửa AC = CR. **Không** tự DOC-16 / probation.
- Affects: identity · DOC-07
- Trace: `docs/03-modules/identity/DOC-07-acceptance-criteria.md`
- Confidence: cao

### DEC-REQ-050 — Mở probation DOC-04 Draft · [2026-08-26]
- Status: accepted *(superseded by DEC-REQ-051 chốt)*
- Context: Anh: soạn DOC-04 probation sau IAM AC Chốt.
- Options: A Fan-out 05–07 · B **Chỉ DOC-04 Draft**
- Decision: chọn B
- Why (loại A vì một artifact BR)
- Consequences: PRB-BR-001…012. T-15/T-7 ngày lịch nháp. 85% = PAY. **Không** tự 05.
- Affects: probation · DOC-04
- Trace: `docs/03-modules/probation/DOC-04-business-rules.md`
- Confidence: vừa *(đơn vị ngày T-15/T-7)*

### DEC-REQ-051 — Chốt DOC-04 BR probation · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt DOC-04 PRB. PRB-BR-001…012.
- Options: A Giữ Draft · B **Chốt v0.1 kèm nợ ngày lịch T-15/T-7**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Cổng BR PRB đóng. Sửa BR = CR. T-15/T-7 = ngày lịch kèm chốt. **Không** tự DOC-05.
- Affects: probation · DOC-04
- Trace: `docs/03-modules/probation/DOC-04-business-rules.md`
- Confidence: vừa *(nợ ngày công vs lịch)*

### DEC-REQ-052 — Mở probation DOC-05 UC Draft · [2026-08-26]
- Status: accepted *(superseded by DEC-REQ-053 chốt)*
- Context: Anh: soạn DOC-05 probation sau BR Chốt.
- Options: A Fan-out 19+06 · B **Chỉ DOC-05 Draft**
- Decision: chọn B
- Why (loại A vì cổng prototype sau UC)
- Consequences: PRB-UC-001…004. LM đề xuất / HR chốt. **Không** tự DOC-19.
- Affects: probation · DOC-05
- Trace: `docs/03-modules/probation/DOC-05-use-cases.md`
- Confidence: vừa *(catalog IAM khi không LM)*

### DEC-REQ-053 — Chốt DOC-05 UC probation · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt DOC-05 PRB. PRB-UC-001…004.
- Options: A Giữ Draft · B **Chốt v0.1 kèm không LM → task T-7 về HR**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Cổng UC PRB đóng. Sửa UC = CR. **Không** tự DOC-19.
- Affects: probation · DOC-05
- Trace: `docs/03-modules/probation/DOC-05-use-cases.md`
- Confidence: vừa *(catalog IAM chi tiết)*

### DEC-REQ-054 — Mở probation DOC-19 khung Draft · [2026-08-26]
- Status: accepted *(superseded by DEC-REQ-055 chốt khung)*
- Context: Anh: soạn khung DOC-19 probation sau UC Chốt.
- Options: A HTML MCP ngay · B **Chỉ khung text/mermaid Draft**
- Decision: chọn B
- Why (loại A vì HTML hoãn như IAM)
- Consequences: PRB-SCR-001…004. HTML nợ. **Không** tự DOC-06.
- Affects: probation · DOC-19
- Trace: `docs/03-modules/probation/DOC-19-prototype.md`
- Confidence: cao

### DEC-REQ-055 — Chốt khung DOC-19 probation · [2026-08-26]
- Status: accepted *(đã chốt khung — PGD Dư Hùng)*
- Context: Anh chốt khung proto PRB. PRB-SCR-001…004.
- Options: A Giữ Draft · B **Chốt khung; HTML nợ; không date picker KT ảo trên PRB**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Cổng proto khung đóng. HTML MCP nợ, không chặn SRS. **Không** tự DOC-06.
- Affects: probation · DOC-19
- Trace: `docs/03-modules/probation/DOC-19-prototype.md`
- Confidence: cao

### DEC-REQ-056 — Mở probation DOC-06 SRS Draft · [2026-08-26]
- Status: accepted *(superseded by DEC-REQ-057 chốt)*
- Context: Anh: soạn DOC-06 probation sau khung 19 Chốt.
- Options: A Kèm DOC-07 · B **Chỉ DOC-06 Draft**
- Decision: chọn B
- Why (loại A vì AC sau SRS)
- Consequences: PRB-FR-001…017. **Không** tự DOC-07.
- Affects: probation · DOC-06
- Trace: `docs/03-modules/probation/DOC-06-srs.md`
- Confidence: cao

### DEC-REQ-057 — Chốt DOC-06 SRS probation · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt SRS PRB. PRB-FR-001…017.
- Options: A Giữ Draft · B **Chốt v0.1 kèm nợ DOC-07 / HTML / Ban HR**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Cổng SRS PRB đóng. Sửa FR = CR. **Không** tự DOC-07.
- Affects: probation · DOC-06
- Trace: `docs/03-modules/probation/DOC-06-srs.md`
- Confidence: cao

### DEC-REQ-058 — Mở probation DOC-07 AC Draft · [2026-08-26]
- Status: accepted *(superseded by DEC-REQ-059 chốt)*
- Context: Anh: soạn DOC-07 probation sau SRS Chốt.
- Options: A Kèm DOC-16 · B **Chỉ DOC-07 Draft**
- Decision: chọn B
- Why (loại A vì delivery)
- Consequences: PRB-AC-001…017. **Không** tự DOC-16.
- Affects: probation · DOC-07
- Trace: `docs/03-modules/probation/DOC-07-acceptance-criteria.md`
- Confidence: cao

### DEC-REQ-059 — Chốt DOC-07 AC probation · [2026-08-26]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh chốt AC PRB. PRB-AC-001…017.
- Options: A Giữ Draft · B **Chốt v0.1 kèm nợ DOC-16 / HTML MCP / Ban HR**
- Decision: chọn B
- Why (loại A vì anh chốt)
- Consequences: Cổng AC PRB đóng. Slice requirements 7 module Must đã chốt DOC-04…07. Sửa AC = CR. **Không** tự DOC-16 / architecture.
- Affects: probation · DOC-07
- Trace: `docs/03-modules/probation/DOC-07-acceptance-criteria.md`
- Confidence: cao

### DEC-REQ-060 — HTML mockup timekeeping (local, không MCP) · [2026-08-26]
- Status: accepted *(nháp wireframe; khung DOC-19 vẫn Chốt DEC-REQ-019)*
- Context: Anh chọn làm HTML mockup; file đang mở = DOC-19 TIM.
- Options: A Chờ MCP ngoài · B Fan-out 7 module HTML · C **Một slice TIM-SCR-001…006 file HTML local; không pixel Must; không 6 module khác**
- Decision: chọn C
- Why (loại A vì anh yêu cầu HTML; loại B vì một slice)
- Consequences: Link trong DOC-19 §3. Pixel HTML không fail AC (TIM-AC-014). **Không** Figma. **Không** code TIM service.
- Affects: timekeeping DOC-19
- Trace: `docs/03-modules/timekeeping/prototype/tim-mockup.html`
- Confidence: cao
