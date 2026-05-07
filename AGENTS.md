# AGENTS.md — ApiLaboratorioAgua

## Build & Test

```powershell
dotnet build ApiLaboratorioAgua.sln
dotnet test                          # test project uses .NET 9 (not 8)
```
> If `dotnet publish` fails with MSB3492, run `dotnet clean` first.

## Before any code change

1. **`@Git Flow start`** — crear issue en GitHub y rama `feat/xxx` desde `develop`
2. **Consult Context7 MCP** — ALWAYS before writing code
   - `context7_resolve-library-id` then `context7_query-docs`
   - Applies to: EF Core, ASP.NET, SQLite, Serilog, QuestPDF
3. Write code
4. **`@Code Reviewer revisá los cambios`** — apply suggestions
5. **`@Git Flow finish`** — commit + push + PRs feat→develop→main + close issue

> Skills: `~/.agents/skills/git-flow/SKILL.md`

## Docker

```powershell
docker compose up -d              # levantar API (puerto 5261)
docker compose up -d --build      # rebuild tras cambios de código
docker compose down               # detener
docker logs laboratorio-agua-api  # ver logs
```
- SQLite persistida en volumen `labagua-data` → `/app/data/LabAgua.db`
- Connection string inyectada vía `ConnectionStrings__defaultConnection`

## Deploy to production

Usar el skill **`@Deploy`** cuando se decide ir a producción (desacoplado del merge):
- `@Deploy API` — publica y reinicia el proceso en `LaboratorioAgua_NEW\Api\`
- `@Deploy frontend` — corre `Update.bat` en AppPlanillaPlantaPot
- `@Deploy ambos` — ambos en orden

> Skill: `~/.agents/skills/deploy/SKILL.md`

## Architecture

Clean Architecture: **Domain → Infrastructure → Aplicacion → ApiLaboratorioAgua**
- `Domain/`: Entities, `IRepository<T>` interfaces
- `Infrastructure/`: EF Core, repositories, DTOs, `Migrations/` (apply auto in all envs)
- `Aplicacion/`: Services, `Mappers/` (entity→DTO), `Factories/` (DTO→entity)
- `ApiLaboratorioAgua/`: Controllers, `Program.cs` (DI registration)

## Database

- **SQLite in all environments** (no InMemory)
- Dev: `LabAgua_Dev.db` (seeded with demo data on first run)
- Prod: `LabAgua.db` or connection string from env var
- Migrations apply automatically on startup (all environments)

## Code conventions

- Dates: `DateTime`, query format `yyyy-MM-dd`
- Pagination: `{ items, totalCount, page, pageSize, totalPages, hasNextPage, hasPreviousPage }`
- **Result Pattern** for errors (no exceptions for flow control)
- `QuestPDF.Settings.License = LicenseType.Community`
- Structured logging with **Serilog** → `logs/api-.log`

## MCP & Tools

- Context7 MCP: `@upstash/context7-mcp@latest` (configured in `.opencode/mcp.json`)
- GitHub MCP: `@modelcontextprotocol/server-github` (requires `GITHUB_TOKEN`)
- Excalidraw MCP: `C:\Users\tomba\excalidraw-mcp\dist\server.js`
- Code Reviewer skill: `~/.agents/skills/code-reviewer/SKILL.md`
- Git Flow skill: `~/.agents/skills/git-flow/SKILL.md`
- Deploy skill: `~/.agents/skills/deploy/SKILL.md`

## CI

GitHub Actions workflow en `.github/workflows/ci.yml`:
- Corre en cada push/PR a `develop` y `main`
- Steps: `dotnet build` + `dotnet test` (SDK 8 para API, SDK 9 para test project)

## Workspace

| Component | Path | Port |
|-----------|------|------|
| API | `ApiLaboratorioAgua/` | 5261 |
| Frontend (Python/Tkinter) | `C:\Users\tomba\OneDrive\Escritorio\AppPlanillaPlantaPot` | — |
| Production | `C:\Users\tomba\OneDrive\Escritorio\LaboratorioAgua_NEW\Api\` | — |

## GitHub repos

- `aletomba/ApiLaboratorioAgua` (backend)
- `aletomba/AppPlanillaPlantaPot` (frontend)