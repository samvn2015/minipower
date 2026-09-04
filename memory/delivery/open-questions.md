# Open questions — Delivery (thực thi)

> Hoãn khi mở code (DEC-DLV-008). **Chặn: có** = artifact là *nháp* đến khi trả lời.

- [ ] {OQ-DLV-001} Issuer OIDC + JWKS URL (IT) · **IdP: Lark** (DEC-DLV-010 · `@lhqglobal.vn`) · cần: tenant Lark, region (CN/Global), App ID/secret, discovery URL · intent: viết code · **PGD tạm bỏ qua 2026-09-04 (DEC-DLV-011):** không chặn Must/UAT local — dùng `/dev/token` + symmetric key Development; **Prod/go-live vẫn chặn** đến khi IT trả JWKS · chặn: không *(DEV/UAT)* / có *(Prod)*
- [ ] {OQ-DLV-002} Sản phẩm LBS / host · intent: triển khai · hoãn 2026-08-26 · chặn: không
- [x] {OQ-DLV-003} DB engine · **PostgreSQL** (ADR-009 · DEC-ARC-014) · **local DEV 2026-08-27:** Postgres.app **16.8** · `localhost:5432` · DB `hrm` · user `admin` · User Secrets · **prod template 2026-08-29:** `appsettings.Production.json` + DOC-17 §2.1 · **còn:** host/user/password Prod do IT · intent: triển khai · chặn: có *(prod host)*
- [ ] {OQ-DLV-004} RTO/RPO phút · intent: go-live · hoãn 2026-08-26 · chặn: có *(cutover)*
- [x] {OQ-DLV-005} JarvisRoot · `/Users/Hung/Documents/Học AI/jarvis` · **đã gắn** ProjectReference IAM Host · 2026-08-26
- [x] {OQ-DLV-006} SDK .NET 9 trên máy build · **có** 9.0.317 (`/usr/local/share/dotnet`) · kiểm tra 2026-08-26
- [x] {OQ-DLV-007} MFA / vendor IdP brand · **Lark** (DEC-DLV-010) · login: Google + Apple + `@lhqglobal.vn` · **còn:** MFA bắt buộc? Apple hide-email? federation Google/Apple trên Lark · intent: viết code · chặn: không *(brand)* / có *(policy MFA)*
- [ ] {OQ-DLV-008} EVT/RPT SRS · intent: viết code · hoãn 2026-08-26 · chặn: không *(ngoài 7 Must)*
- [x] {OQ-DLV-009} Ban HR ký BO · intent: viết code · hoãn — · chặn: không · **đã ký 2026-08-26** (PGD)
