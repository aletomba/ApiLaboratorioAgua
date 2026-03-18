# Copilot Instructions — LaboratorioAgua (API)

## Arquitectura
Clean Architecture con 4 capas:
- **Domain** (`Dominio.csproj`): Entidades e interfaces `IRepository`
- **Infrastructure** (`Infrastructure.csproj`): Repositorios EF Core, DbContext, Migrations, DTOs
- **Aplicacion** (`Aplicacion.csproj`): Servicios de aplicación
- **ApiLaboratorioAgua** (`ApiLaboratorioAgua.csproj`): Controllers ASP.NET 8, Program.cs

## Entidades principales
`LibroEntrada`, `PlanillaDiaria`, `LibroFisico`, `LibroBacteriologia`, `Cliente`, `Muestra`

## Base de datos
- SQLite, connection string: `Data Source=LabAgua_Db`
- Migraciones en `Infrastructure/Migrations/`
- Comando: `dotnet ef migrations add NombreMigracion --project Infrastructure --startup-project ApiLaboratorioAgua`

## Convenciones
- Fechas siempre `DateTime`, formato query string `yyyy-MM-dd`
- Endpoints paginados retornan: `{ items, totalCount, page, pageSize, totalPages, hasNextPage, hasPreviousPage }`
- Repositorios implementan `IRepository<T>` genérico en `Domain/IRepository/`
- Inyección de dependencias registrada en `Program.cs`

## Carpetas relevantes
- **Fuente**: `c:\Users\tomba\source\repos\ApiLaboratorioAgua\`
- **Producción API**: `C:\Users\tomba\OneDrive\Escritorio\LaboratorioAgua_NEW\Api\`
- **API corre en**: `http://localhost:5261` (producción vía START.bat)

## Git Flow
1. Usar **GitHub MCP** para crear rama `feat/nombre-feature` desde `develop`
2. Commit + push de la rama feat (vía terminal/git local)
3. Usar **GitHub MCP** para crear PR `feat` → `develop`, mergearlo y borrar la rama feat
4. Usar **GitHub MCP** para crear PR `develop` → `main` y mergearlo
5. Repos GitHub: `aletomba/ApiLaboratorioAgua` y `aletomba/AppPlanillaPlantaPot`

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
