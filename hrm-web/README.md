# HRM Web — Frontend MVP

Giao diện end-user tối thiểu nối `hrm-backend` local.

## Chạy

```bash
# Terminal 1 — backend (nếu chưa chạy)
cd HRM/hrm-backend/src
ASPNETCORE_ENVIRONMENT=Development dotnet run --project Hrm.Host --urls "http://localhost:5167"

# Terminal 2 — frontend
cd HRM/hrm-web
npm install
npm run dev
```

Mở trình duyệt: **http://localhost:5173**

## Màn hình MVP

| Màn | Route | Mô tả |
|-----|-------|-------|
| Đăng nhập dev | `/login` | Chọn HR hoặc IT (JWT dev) |
| Hồ sơ của tôi | `/profile` | EMP-SCR-003 (NV self-service) |
| Danh sách NV | `/employees` | EMP-SCR-001 (HR/IT) |
| Tạo / sửa hồ sơ | `/employees/new`, `/employees/{id}` | EMP-SCR-002 · gửi đổi LM (SCR-005) |
| Duyệt đổi LM | `/line-manager-changes` | EMP-SCR-006 |
| IAM accounts | `/iam/accounts` | IAM-SCR-003 (HR/IT) |
| Quản lý account | `/iam/accounts/{id}` | Gán/gỡ role · IT disable (SCR-004) |

## Ghi chú

- Vite proxy `/v1` và `/dev` → `http://localhost:5167` (tránh CORS local).
- Production sẽ dùng Lark SSO — chưa tích hợp trong MVP này.
