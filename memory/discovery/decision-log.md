# Decision Log — Discovery

> ID: `DEC-DIS-NNN`

### DEC-DIS-001 — PROCEED discovery từ URD HRM; không gộp CRM sản phẩm · [2026-08-24]
- Status: accepted *(Dư Hùng PGD / BA — Draft; baseline khi ký DOC-03)*
- Context: URD 8 phân hệ + IAM + UAT. Đau rõ, chưa đo as-is. URD-02 ghi “CRM” khi duyệt phép.
- Options: A Coi URD = SRS đã ký, nhảy requirements · B STOP chờ số as-is · C PROCEED draft DOC-01–03; URD = elicitation; xác nhận công thức lương + X ngày phép
- Decision: chọn C
- Why (loại A vì công thức lương/X ngày/pháp lý chưa chốt; loại B vì đủ phân hệ + actor + UAT để viết scope)
- Consequences: chưa distill `docs/` đến khi anh accepted. Thông báo phép = Email/App/HRM, không phải CRM khách hàng.
- Affects: DOC-01–03 · EMP LEV TIM PAY PRB EVT LIF RPT IAM
- Trace: URD HRM `.docx` · `brainstorm/2026-08-24-urd-hrm.md`
- Confidence: cao

### DEC-DIS-002 — N_tính không cộng kép phép hưởng; N+3; mobile MVP; A = PGD Dư Hùng · [2026-08-24]
- Status: accepted *(elicitation)*
- Context: Anh accepted distill; N_thực đã gồm phép hưởng lương; 85% quy chế; 1 mẫu CC; khóa Git/CRM N+3; mobile in MVP; A = Mr. Dư Hùng PGD; URD-02 không có chữ CRM; X và danh mục PC/thưởng trả “đúng”.
- Options: giữ URD N_tính = N_thực − KHL + phép hưởng / **bỏ cộng phép hưởng**
- Decision: BR-001 = N_thực − N_KHL, không + N_phép_hưởng. X = tham số cấu hình (chưa số). Phụ cấp = cả HĐ và nhập tháng (danh mục DOC-04).
- Why (loại công thức URD gốc vì cộng kép khi N_thực đã gồm phép hưởng)
- Consequences: DOC-01–03 v0.1. Không tích hợp thông báo CRM bán hàng.
- Affects: PAY, LEV, LIF, IAM · DOC-03
- Trace: chat 2026-08-24 · DOC-03 BR-001
- Confidence: cao *(A-001: KHL có trong N_thực hay không — vừa)*

### DEC-DIS-003 — Nộp phép ≥3 ngày liên tiếp trước ≥ 3 ngày làm việc · [2026-08-24]
- Status: accepted
- Context: Anh chốt X: người xin nghỉ phải xin ít nhất trước 3 ngày làm việc.
- Options: X cấu hình chưa số / **X = 3 NLĐ**
- Decision: Nghỉ ≥ 3 ngày liên tiếp → submit ≥ **3 ngày làm việc** trước ngày bắt đầu nghỉ.
- Why (loại để trống vì anh đã nêu số và loại ngày = làm việc, không phải ngày lịch)
- Consequences: BR-007, BRQ-008 Must. Ngoại lệ: **DEC-DIS-004** (HR duyệt đột xuất).
- Affects: LEV · DOC-03 v0.2
- Trace: chat 2026-08-24
- Confidence: cao

### DEC-DIS-004 — Ngoại lệ 3 NLĐ: HR phê duyệt nghỉ đột xuất · [2026-08-24]
- Status: accepted
- Context: Anh không miễn loại phép cứng; để HR phê duyệt nghỉ đột xuất.
- Options: A Chặn submit trễ, không ngoại lệ · B Miễn ốm tự động · C Đơn đột xuất, **chỉ HR** duyệt ngoại lệ
- Decision: chọn C
- Why (loại A vì thực tế có đột xuất; loại B vì không tự bypass)
- Consequences: BR-008. LM không chốt một mình đơn trễ 3 NLĐ. Trừ quỹ sau duyệt HR.
- Affects: LEV · DOC-03 v0.3
- Trace: chat 2026-08-24 · DEC-DIS-003
- Confidence: cao

### DEC-DIS-005 — Chốt DOC-01 tầm nhìn (cổng vision) · [2026-08-24]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh bảo chốt DOC-01. ROI/as-is vẫn TBD. Ban NS chưa ký dòng Owner.
- Options: A Giữ Draft · B Chốt tầm nhìn kèm nợ ROI · C Snapshot `02-baseline/` ngay
- Decision: chọn B
- Why (loại A vì anh chốt; loại C vì chưa ký DOC-03 / chưa ROI — không đóng băng repo)
- Consequences: DOC-01 Status **Chốt**. Không mở architecture. A **BRD** vẫn DOC-03. KPI UAT (0đ, &lt;5s) giữ; tiền/as-is nợ.
- Affects: DOC-01
- Trace: `docs/01-project/DOC-01-vision-business-case.md`
- Confidence: cao

### DEC-DIS-006 — Chốt DOC-02 stakeholder (cổng RACI) · [2026-08-24]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh bảo chốt DOC-02. RACI sơ bộ; SH-003/008 không có cột. Ban NS chưa ký Owner.
- Options: A Giữ Draft · B Chốt register + RACI kèm nợ · C Snapshot baseline
- Decision: chọn B
- Why (loại A vì anh chốt; loại C vì chưa DOC-03)
- Consequences: A BRD = SH-001. RACI theo deliverable giữ nguyên. Không fan-out module mới.
- Affects: DOC-02
- Trace: `docs/01-project/DOC-02-stakeholder-analysis.md`
- Confidence: cao

### DEC-DIS-007 — Quy mô chi phí Cty cho DOC-01 §6 (chưa ROI dự án) · [2026-08-24]
- Status: accepted
- Context: Anh: quỹ lương ~3 tỷ/tháng; lương TB 20 triệu/LĐ/tháng; tổng chi phí Cty ~4 tỷ/tháng (cố định + biến đổi).
- Options: A Bịa % 4 tỷ = lợi ích HRM · B Ghi bối cảnh Cty; đơn giá giờ suy ra; CAPEX/ROI dự án TBD
- Decision: chọn B
- Why (loại A vì 4 tỷ là opex Cty, không phải ngân sách phần mềm)
- Consequences: DOC-01 §3, §6. Headcount ~150. Đơn giá ~113.600 đ/giờ (176h). Thiếu ngân sách Y1 và giờ C&B.
- Affects: DOC-01
- Trace: chat 2026-08-24
- Confidence: cao *(176h — giả định)*

### DEC-DIS-008 — CAPEX HRM Năm 1 ~ 1 tỷ đồng (2026) · [2026-08-24]
- Status: accepted
- Context: Anh chốt ngân sách xây HRM khoảng 1 tỷ trong 2026.
- Options: A Coi 4 tỷ/tháng Cty là CAPEX · B **~1 tỷ CAPEX 2026**
- Decision: chọn B
- Why (loại A vì đã giải thích CAPEX ≠ opex Cty)
- Consequences: Hòa vốn Y1 ≈ 4,2 FTE C&B hoặc ~8.800 giờ. ROI % vẫn chờ giờ tiết kiệm. OPEX Y2 TBD.
- Affects: DOC-01 §6.2B, 6.3
- Trace: chat 2026-08-24
- Confidence: vừa *(1 tỷ ước; OPEX có thể nằm trong hoặc ngoài)*

### DEC-DIS-009 — Mảng HR = 3 nhân sự; trần lợi ích giờ · [2026-08-24]
- Status: accepted
- Context: Anh: mảng HR có 3 người (trả lời giờ C&B).
- Options: A Coi 150 LĐ là C&B · B **3 FTE HR**; trần 720 triệu/năm; không bịa % thời gian lương/công/phép
- Decision: chọn B
- Why (loại A vì C&B ≠ toàn Cty)
- Consequences: Trần 720tr/năm. Với DEC-DIS-010, trần này chỉ đếm từ 2027.
- Affects: DOC-01 §3, §6
- Trace: chat 2026-08-24
- Confidence: cao

### DEC-DIS-010 — CAPEX 2026; đưa vào sử dụng từ 2027 · [2026-08-24]
- Status: accepted
- Context: Anh: năm 2026 đầu tư 1 tỷ làm HRM; bắt đầu sử dụng từ 2027.
- Options: A Dùng và thu lợi trong 2026 · B **Xây 2026, dùng từ 2027**
- Decision: chọn B
- Why (loại A vì lịch PGD: năm xây ≠ năm vận hành)
- Consequences: Lợi ích giờ HR = 0 trong 2026. Payback đếm từ 2027; trần 720tr/năm → >12 tháng sau go-live nếu chỉ lương 3 HR.
- Affects: DOC-01 §1, §6.2B, §6.3, C-006, A-009
- Trace: chat 2026-08-24
- Confidence: cao

### DEC-DIS-011 — Chốt DOC-01 v0.2 (gồm DEC-007–010, kèm nợ) · [2026-08-24]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh xác nhận chốt DOC-01 sau khi đủ khung tầm nhìn + lịch 2026/2027; biết nợ OPEX, % giờ 3 HR, Ban NS, as-is chưa đo.
- Options: A Giữ Draft đến khi Ban NS + ROI % · B **Chốt v0.2 kèm nợ** · C Snapshot `02-baseline/`
- Decision: chọn B
- Why (loại A vì PGD đã đủ tầm nhìn/KPI/CAPEX; loại C vì cổng pipeline = DOC-03, chưa đóng băng repo)
- Consequences: Status **Chốt** v0.2. G-002 khớp DEC-DIS-003 (3 NLĐ liền). Không mở architecture từ DOC-01. Sửa nội dung vision sau này = CR hoặc DEC mới.
- Affects: DOC-01
- Trace: `docs/01-project/DOC-01-vision-business-case.md`
- Confidence: cao

### DEC-DIS-012 — BA dự án = Trịnh Yên · [2026-08-24]
- Status: accepted
- Context: Anh: BA sẽ là Trịnh Yên.
- Options: A PGD kiêm BA · B **BA = Trịnh Yên**; PGD chỉ A
- Decision: chọn B
- Why (loại A vì tách A/R, giảm R-003)
- Consequences: SH-009 = Trịnh Yên; SH-010 = Dev/Tester. R soạn DOC = SH-009. Không đổi A BRD (SH-001).
- Confidence: cao

### DEC-DIS-013 — Ngưỡng phép dài = ≥3 ngày công chuẩn liền · [2026-08-24]
- Status: accepted
- Context: Anh: “≥3 ngày liên tiếp” = **ngày công chuẩn**, không phải ngày lịch.
- Options: A 3 ngày lịch · B **3 ngày công chuẩn liền**
- Decision: chọn B
- Why (loại A vì T7/CN/lễ làm sai ngưỡng)
- Consequences: BR-007, BRQ-008, thuật ngữ, A-004. Hạn nộp cũng đếm ngày công chuẩn (DEC-DIS-003).
- Affects: DOC-03 v0.5
- Trace: chat 2026-08-24
- Confidence: cao

### DEC-DIS-014 — Master vận hành khai báo động theo quy chế Cty · [2026-08-24]
- Status: accepted
- Context: Anh: các mục “chốt kèm nợ” AS-IS, D-001/D-004, A-002/D-003, rationale BRQ = động theo chính sách Cty, không TBD trên BRD.
- Options: A Giữ TBD/nợ · B **Khai báo động theo quy chế**; BRD chốt nguyên tắc
- Decision: chọn B
- Why (loại A vì đóng băng mẫu/lịch/PC trên BRD sẽ lệch quy chế)
- Consequences: CN-006, A-002/A-005, D-001…004 động. Không đính file mẫu / list PC lúc ký BRD.
- Confidence: cao

### DEC-DIS-015 — Chốt DOC-03 BRD v0.6 (kèm nợ Ban NS) · [2026-08-24]
- Status: accepted *(đã chốt — PGD Dư Hùng)*
- Context: Anh: sửa rồi chốt DOC-03. Giấy đã đủ BO-002, ngày công chuẩn, DEC-010/012, CN-004/005, master động 014.
- Options: A Giữ Draft đến Ban NS ký · B **Chốt BRD A kèm nợ Owner** · C Snapshot `02-baseline/`
- Decision: chọn B
- Why (loại A vì PGD là cổng A BRD; loại C vì chưa đóng băng repo / Ban NS chưa ký)
- Consequences: Cổng 1 Minipower **mở** (fan-out BR theo module còn thiếu). Leave đã có DOC-04. Không tự baseline. Sửa in/out sau = CR.
- Affects: DOC-03
### DEC-DIS-016 — Phụ lục A DOC-03: BRQ → BO · [2026-08-24]
- Status: accepted
- Context: Anh: đưa bảng mapping BRQ–BO vào phụ lục.
- Options: A Chỉ chat · B **Phụ lục A**, không đổi in/out
- Decision: chọn B
- Why (loại A vì mất trace trên BRD)
- Consequences: DOC-03 v0.7. Cổng chốt 015 giữ. Không snapshot baseline.
- Affects: DOC-03 Phụ lục A
- Trace: chat 2026-08-24
- Confidence: cao


