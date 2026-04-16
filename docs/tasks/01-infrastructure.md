# Task 01 — Project Infrastructure Setup

**GitHub Issue Title**: `[Task-01] Project Infrastructure Setup`  
**Labels**: `agent-task`, `infrastructure`  
**Priority**: 🔴 Critical (all other tasks depend on this)

---

## Objective

Bootstrap the complete project structure with Docker Compose, .NET 10 AOT solution, Vue 3 frontend scaffold, and PostgreSQL database — so all subsequent tasks can build on a working foundation.

---

## Deliverables

### Repository Structure to Create

```
src/
├── backend/
│   ├── DentalChart.sln
│   ├── DentalChart.Api/               ← .NET 10 Minimal API + AOT
│   │   ├── DentalChart.Api.csproj
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── Properties/launchSettings.json
│   ├── DentalChart.Core/              ← Domain models, interfaces (no EF dependency)
│   │   └── DentalChart.Core.csproj
│   ├── DentalChart.Infrastructure/    ← EF Core, PostgreSQL, repositories
│   │   └── DentalChart.Infrastructure.csproj
│   └── DentalChart.Tests/             ← xUnit tests
│       └── DentalChart.Tests.csproj
├── frontend/
│   ├── package.json                   ← Vue 3 + TS + Vite
│   ├── vite.config.ts
│   ├── tsconfig.json
│   ├── .eslintrc.cjs
│   └── src/
│       ├── main.ts
│       ├── App.vue
│       ├── router/index.ts
│       ├── stores/                    ← Pinia
│       ├── services/                  ← axios API client
│       ├── components/
│       ├── views/
│       └── types/
docker/
├── docker-compose.yml
├── docker-compose.prod.yml
├── Dockerfile.backend
├── Dockerfile.frontend
└── nginx.conf
database/
├── migrations/
│   └── 001_initial_schema.sql
└── seeds/
    └── 001_procedure_codes.sql
.env.example
```

### Backend Requirements

- .NET 10 with `PublishAot = true` in `DentalChart.Api.csproj`
- Use `Microsoft.AspNetCore.OpenApi` (built-in .NET 9+)
- EF Core 9 with `Npgsql.EntityFrameworkCore.PostgreSQL`
- `System.Text.Json` source generation context configured
- Health check endpoint: `GET /health` returns `{"status":"healthy","timestamp":"..."}`
- CORS configured for frontend origin

**DentalChart.Api.csproj must include**:
```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
  <PublishAot>true</PublishAot>
  <InvariantGlobalization>true</InvariantGlobalization>
</PropertyGroup>
```

### Frontend Requirements

- Vue 3 + TypeScript + Vite scaffold (`npm create vue@latest`)
- Dependencies:
  ```
  vue-router@4, pinia, axios, element-plus,
  @vueuse/core, echarts, vue-echarts,
  jspdf, html2canvas, vue-draggable-next
  ```
- Dev dependencies: `vitest`, `@vitest/coverage-v8`, `@vue/test-utils`, `eslint`
- `vite.config.ts`: proxy `/api` → `http://localhost:8080`
- Base layout: `AppLayout.vue` with sidebar nav + main content area

### Docker Compose Requirements

`docker/docker-compose.yml`:
```yaml
services:
  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_DB: dentalchart
      POSTGRES_USER: dentalchart
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:-devpassword}
    ports: ["5432:5432"]
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ../database/migrations:/docker-entrypoint-initdb.d

  backend:
    build: { context: .., dockerfile: docker/Dockerfile.backend }
    environment:
      ConnectionStrings__DefaultConnection: "Host=postgres;..."
      Jwt__SecretKey: ${JWT_SECRET_KEY:-dev-secret-key-change-in-production}
    ports: ["8080:8080"]
    depends_on: [postgres]

  frontend:
    build: { context: .., dockerfile: docker/Dockerfile.frontend }
    ports: ["3000:80"]
    depends_on: [backend]
```

### `.env.example`
```
POSTGRES_PASSWORD=changeme
JWT_SECRET_KEY=change-this-to-a-long-random-secret
CORS_ALLOWED_ORIGINS=http://localhost:3000
```

---

## Acceptance Criteria

- [ ] `docker compose up` starts all 3 services without errors
- [ ] `GET http://localhost:8080/health` returns HTTP 200
- [ ] `GET http://localhost:3000` serves the Vue app
- [ ] `dotnet build src/backend/DentalChart.sln` succeeds with no errors
- [ ] `npm run build` in `src/frontend` succeeds
- [ ] `npm run test` in `src/frontend` runs (even with 0 tests initially)
- [ ] `.env.example` file present; `.env` in `.gitignore`
