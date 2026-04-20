# Copilot Instructions — LaboratorioAgua (API)

## Arquitectura
Clean Architecture con 4 capas:
- **Domain** (`Dominio.csproj`): Entidades e interfaces `IRepository`
- **Infrastructure** (`Infrastructure.csproj`): Repositorios EF Core, DbContext, Migrations, DTOs
- **Aplicacion** (`Aplicacion.csproj`): Servicios de aplicación, `Mappers/` (entity→DTO), `Factories/` (DTO→entity)
- **ApiLaboratorioAgua** (`ApiLaboratorioAgua.csproj`): Controllers ASP.NET 8, Program.cs

## Entidades principales
`LibroEntrada`, `PlanillaDiaria`, `LibroFisico`, `LibroBacteriologia`, `Cliente`, `Muestra`

## Base de datos
- **SQLite en todos los environments** (no se usa InMemory)
- Development: `LabAgua_Dev.db` (datos de seed, persistentes entre reinicios)
- Production / Docker: `LabAgua.db` (o la connection string inyectada vía env var)
- Connection string en `appsettings.json`: `Data Source=LabAgua_Db`
- Migraciones en `Infrastructure/Migrations/`, se aplican automáticamente al arranque en **todos** los environments
- Seed de datos demo solo en Development, condicional (`if (!db.Clientes.Any())`)
- Comando para agregar migración: `dotnet ef migrations add NombreMigracion --project Infrastructure --startup-project ApiLaboratorioAgua`

## Build y Test
```powershell
cd "c:\Users\tomba\source\repos\ApiLaboratorioAgua"
dotnet build ApiLaboratorioAgua.sln          # compilar solución
dotnet test                                  # correr tests
```
> Si `dotnet publish` falla con **MSB3492** (GlobalUsings.g.cs bloqueado), ejecutar `dotnet clean` primero y luego reintentar.

## Convenciones
- **Siempre** consultar **Context7 MCP** antes de escribir o modificar código. Sin excepciones.
  - Resolver la librería con `resolve-library-id` y luego consultar docs con `query-docs`
  - Aplica para: EF Core, ASP.NET, xUnit, Moq, SQLite, Serilog, QuestPDF, y cualquier otra dependencia
  - Incluso si se cree conocer la API, consultar igual — el training data puede estar desactualizado
- Fechas siempre `DateTime`, formato query string `yyyy-MM-dd`
- Endpoints paginados retornan: `{ items, totalCount, page, pageSize, totalPages, hasNextPage, hasPreviousPage }`
- Repositorios implementan `IRepository<T>` genérico en `Domain/IRepository/`
- Inyección de dependencias registrada en `Program.cs`

## Carpetas relevantes
- **Fuente**: `c:\Users\tomba\source\repos\ApiLaboratorioAgua\`
- **Producción API**: `C:\Users\tomba\OneDrive\Escritorio\LaboratorioAgua_NEW\Api\`
- **API corre en**: `http://localhost:5261` (producción vía START.bat)

## Git Flow
Todo el flujo se ejecuta con **GitHub MCP** (`mcp_github_*`). Respetar este orden siempre:
1. Usar **GitHub MCP** para crear rama `feat/nombre-feature` desde `develop`
2. Commit + push de la rama feat (vía terminal/git local)
3. Invocar el agente **Code Reviewer** (`@Code Reviewer revisá los cambios de esta rama antes del PR`) y aplicar las mejoras sugeridas
4. Usar **GitHub MCP** para crear PR `feat` → `develop`, mergearlo y borrar la rama feat
5. Usar **GitHub MCP** para crear PR `develop` → `main` y mergearlo
6. Repos GitHub: `aletomba/ApiLaboratorioAgua` y `aletomba/AppPlanillaPlantaPot`
7. **Al finalizar el flujo completo, siempre hacer `git checkout develop` para dejar el repo posicionado en `develop`.**
8. **Cerrar en GitHub las issues resueltas al terminar cada feature/fix.**

> El token del MCP está configurado en `settings.json` local (nunca en este archivo).

## Deploy API
```powershell
cd "c:\Users\tomba\source\repos\ApiLaboratorioAgua"
dotnet publish ApiLaboratorioAgua/ApiLaboratorioAgua.csproj -c Release -o publish_out
xcopy /E /Y ".\publish_out\*" "C:\Users\tomba\OneDrive\Escritorio\LaboratorioAgua_NEW\Api\"
Stop-Process -Name "ApiLaboratorioAgua" -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 1
Start-Process "C:\Users\tomba\OneDrive\Escritorio\LaboratorioAgua_NEW\Api\ApiLaboratorioAgua.exe" -WorkingDirectory "C:\Users\tomba\OneDrive\Escritorio\LaboratorioAgua_NEW\Api"
```

## Docker
La API está dockerizada con multi-stage build (`Dockerfile`, `docker-compose.yml`, `.dockerignore`).
```powershell
cd "c:\Users\tomba\source\repos\ApiLaboratorioAgua"
docker compose up -d              # levantar API en contenedor (puerto 5261)
docker compose down               # detener y remover contenedor
docker compose up -d --build      # rebuild tras cambios de código
docker logs laboratorio-agua-api  # ver logs del contenedor
```
- **Imagen base**: `mcr.microsoft.com/dotnet/aspnet:8.0` (runtime), `sdk:8.0` (build)
- **Puerto**: `5261` (mismo que producción standalone)
- **SQLite**: persistida en volumen Docker `labagua-data` montado en `/app/data/LabAgua.db`
- **Connection string** inyectada vía variable de entorno `ConnectionStrings__defaultConnection`
- **Migraciones**: se aplican automáticamente al arranque (en todos los environments)
