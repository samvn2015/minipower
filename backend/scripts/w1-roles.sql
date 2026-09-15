-- ============================================================================
-- ADR-011 W1b — Role + GRANT theo bounded context
-- Yêu cầu: chạy bằng SUPERUSER (role ứng dụng không có rolcreaterole — DEC-ARC-023)
-- Tiền đề: migration W1_SchemaPerBoundedContext đã áp (41 bảng đã vào schema)
--
-- Quyết định áp dụng:
--   ①a  emp_audit_log nằm ở schema `shared`, mọi app role được INSERT
--   ②a  CHỈ LEV đọc `emp`, và chỉ 4 cột không nhạy cảm (siết theo OQ-ARC-015)
--   ③ii migrator role RIÊNG; app role chỉ DML, không DDL (khớp DOC-17 §2.1)
--
-- Đổi :db và mật khẩu trước khi chạy Prod. KHÔNG commit mật khẩu thật.
-- ============================================================================

\set ON_ERROR_STOP on

-- ---------------------------------------------------------------- roles ----
DO $$
DECLARE ctx text;
BEGIN
  FOREACH ctx IN ARRAY ARRAY['iam','emp','lev','tim','pay','prb','lif'] LOOP
    IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'hrm_app_'||ctx) THEN
      EXECUTE format('CREATE ROLE %I LOGIN PASSWORD %L', 'hrm_app_'||ctx, 'CHANGE_ME_'||ctx);
    END IF;
  END LOOP;
  -- ③ii: một migrator cho toàn solution (còn MỘT DbContext).
  -- Tách migrator theo context khi W1c chia DbContext.
  IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'hrm_migrator') THEN
    CREATE ROLE hrm_migrator LOGIN PASSWORD 'CHANGE_ME_migrator';
  END IF;
END $$;

-- ------------------------------------------------------ siết mặc định ----
REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON DATABASE hrm FROM PUBLIC;

GRANT CONNECT ON DATABASE hrm TO
  hrm_app_iam, hrm_app_emp, hrm_app_lev, hrm_app_tim,
  hrm_app_pay, hrm_app_prb, hrm_app_lif, hrm_migrator;

-- ------------------------------------------- app role: schema của mình ----
-- USAGE + DML, KHÔNG có CREATE → không tự đổi lược đồ (③ii)
DO $$
DECLARE ctx text; r text;
BEGIN
  FOREACH ctx IN ARRAY ARRAY['iam','emp','lev','tim','pay','prb','lif'] LOOP
    r := 'hrm_app_'||ctx;
    EXECUTE format('GRANT USAGE ON SCHEMA %I TO %I', ctx, r);
    EXECUTE format('GRANT SELECT,INSERT,UPDATE,DELETE ON ALL TABLES IN SCHEMA %I TO %I', ctx, r);
    EXECUTE format('GRANT USAGE,SELECT ON ALL SEQUENCES IN SCHEMA %I TO %I', ctx, r);
    -- bảng do migrator tạo về sau cũng phải cấp tự động
    EXECUTE format(
      'ALTER DEFAULT PRIVILEGES FOR ROLE hrm_migrator IN SCHEMA %I
         GRANT SELECT,INSERT,UPDATE,DELETE ON TABLES TO %I', ctx, r);
  END LOOP;
END $$;

-- ------------------ ②a siết theo OQ-ARC-015: chỉ LEV, chỉ 4 cột ----
-- v1 cấp SELECT cả schema `emp` cho 6 role ⇒ mọi context đọc được PII
-- (`Cccd`, `TaxId`, `EmailCty`). Đo lại code: **chỉ LEV** truy vấn `emp`
-- (LeaveRequestRepository JOIN để lọc hàng đợi C1/C2 theo line manager),
-- và chỉ cần 4 cột không nhạy cảm. Năm role còn lại không cần gì.
--
-- DOC-11 §4 "EMP là golden record, PRB đọc" vẫn đúng: đọc qua API của EMP
-- (IEmployeeReadRepository chạy trên EmpDbContext/hrm_app_emp), không đọc thẳng bảng.
REVOKE ALL ON ALL TABLES IN SCHEMA emp FROM
  hrm_app_iam, hrm_app_lev, hrm_app_tim, hrm_app_pay, hrm_app_prb, hrm_app_lif;
REVOKE USAGE ON SCHEMA emp FROM
  hrm_app_iam, hrm_app_tim, hrm_app_pay, hrm_app_prb, hrm_app_lif;

GRANT USAGE ON SCHEMA emp TO hrm_app_lev;
GRANT SELECT ("Id", "EmployeeCode", "FullName", "LineManagerEmployeeId")
  ON emp.emp_employee TO hrm_app_lev;

-- ------------------------------------ ①a: audit dùng chung, ai cũng ghi ----
-- DEC-DLV-022/024 giữ nguyên: một bảng audit cho 7 module.
DO $$
DECLARE ctx text; r text;
BEGIN
  FOREACH ctx IN ARRAY ARRAY['iam','emp','lev','tim','pay','prb','lif'] LOOP
    r := 'hrm_app_'||ctx;
    EXECUTE format('GRANT USAGE ON SCHEMA shared TO %I', r);
    EXECUTE format('GRANT SELECT,INSERT ON shared.emp_audit_log TO %I', r);
  END LOOP;
END $$;

-- --------------------------------------------------- ③ii: migrator role ----
-- DDL trên mọi schema + bảng lịch sử migration của EF.
DO $$
DECLARE ctx text;
BEGIN
  FOREACH ctx IN ARRAY ARRAY['iam','emp','lev','tim','pay','prb','lif','shared'] LOOP
    EXECUTE format('GRANT USAGE,CREATE ON SCHEMA %I TO hrm_migrator', ctx);
    EXECUTE format('GRANT ALL ON ALL TABLES IN SCHEMA %I TO hrm_migrator', ctx);
  END LOOP;
END $$;
GRANT USAGE, CREATE ON SCHEMA public TO hrm_migrator;   -- __EFMigrationsHistory
GRANT ALL ON ALL TABLES IN SCHEMA public TO hrm_migrator;
