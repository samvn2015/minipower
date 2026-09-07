# Open questions — Architecture

- [x] {OQ-ARC-001} PGD **Accepted** ADR-001 gói F (MS + GW + LBS + SSO) · 2026-08-26 · DEC-ARC-003
- [ ] {OQ-ARC-002} **Số phút RTO / RPO** (pattern 24/7+A/S+DR đã khóa ADR-003) · intent: NFR-012 · 2026-08-26 · chặn: không (hợp đồng SLA)
- [ ] {OQ-ARC-003} Mã hóa at-rest / TLS policy · intent: SAD · hoãn 2026-08-26 · chặn: không
- [x] {OQ-ARC-004} IdP = **Lark** (ADR-007 v0.2 · DEC-DLV-010 / DEC-ARC-015). Login: Google · Apple · `@lhqglobal.vn`. Còn issuer URL / tenant / region do IT (OQ-DLV-001) · không chặn DOC-12 khung
- [ ] {OQ-ARC-005} Broker giữa microservices (TIM→PAY saga) · intent: job · hoãn 2026-08-26 · chặn: không
- [ ] {OQ-ARC-006} Sản phẩm **LBS** · intent: deploy · 2026-08-26 · chặn: không (DOC-17)
- [ ] {OQ-ARC-007} MFA bắt buộc sau SSO? · intent: IAM · hoãn 2026-08-26 · chặn: không
- [x] {OQ-ARC-008} **Lý do gốc chỉ đạo microservices** — **ý kiến khách hàng** (PGD trả lời 2026-09-07, DEC-ARC-016). Không phải lựa chọn kỹ thuật nội bộ → không tự đảo được, muốn đổi phải qua khách. *(nguyên văn câu hỏi:* — kỹ thuật (scale/cô lập) hay yêu cầu từ mInvoice/khách? · intent: ADR mới thay ADR-001 · 2026-09-07 · **chặn: có** (không trả lời thì mọi phương án kiến trúc là đoán) · [deliberation B1](../../brainstorm/2026-09-07-arc-b1-monolith-vs-microservices.md)
- [x] {OQ-ARC-009} **Đội ops cho 7 service + DR hai DC** — **bỏ DR/DC** (PGD 2026-09-07, DEC-ARC-016). Gánh ops giảm mạnh; **đảo ADR-003 → cần ADR mới**. *(nguyên văn câu hỏi:* — đến từ đâu, khi nào? DOC-14 R-01/A-01 ghi chưa có · intent: ADR mới · 2026-09-07 · **chặn: có**
- [ ] {OQ-ARC-010} Tải thật cuối tháng (số NV chấm công đồng thời) — chưa đo, NFR-001 chưa verify trên kiến trúc hiện tại · intent: quyết đơn vị scale · 2026-09-07 · chặn: không
- [ ] {OQ-ARC-011} Tách schema/DB theo bounded context trong **một** deploy có đạt NFR-002 không? (AS-04, tin cậy vừa) · intent: phương án kiến trúc · 2026-09-07 · chặn: không
