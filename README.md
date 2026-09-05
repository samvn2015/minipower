# Hrm — scaffold 4 phần (Jarvis local)

Greenfield dưới `hrm/` theo skill **jarvis-dotnet** + ProjectReference tới repo sibling [`jarvis`](../../jarvis) (**không** NuGet Jarvis).

> Song song với delivery hiện có (`hrm-backend` / `hrm-web`). Tree này là scaffold chuẩn Jarvis.

## Cấu trúc

```text
hrm/
├── Hrm.sln                 ← một solution duy nhất
├── backend/src/            ← Domain.Shared → Domain → Application → Infrastructure → Host
├── frontend/               ← Vite + React (dashboard smoke)
├── autotest/               ← Playwright API + UI
└── unittest/               ← xUnit Domain + Application
```

## Backend (Swagger · OTEL · Health · CORS)

| Endpoint | Kỳ vọng |
|----------|---------|
| `GET /api/ping` | 200 |
| `GET /health/live` | 200 |
| `http://localhost:5287/swagger` | UI Swagger |

```bash
cd hrm
dotnet build Hrm.sln
dotnet run --project backend/src/Hrm.Host --launch-profile http
```

## Frontend

```bash
cd hrm/frontend
npm install
npm run dev
# http://127.0.0.1:5283
```

## Unittest

```bash
cd hrm
dotnet test Hrm.sln
```

## Autotest (cần backend + frontend đang chạy)

```bash
cd hrm/autotest
npm install
npm test                 # API + UI
npm run test:api
npm run test:ui
```

Env tùy chọn: `API_BASE`, `WEB_BASE`.

## Jarvis

Path tương đối từ `backend/src/*`: `../../../../../jarvis/...`
