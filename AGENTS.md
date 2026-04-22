# AGENTS.md — LaboratorioAgua

## Antes de cada modificación

1. `git checkout develop && git pull`
2. `git checkout -b feat/xxx` desde develop
3. **Consultar Context7 MCP** — SIEMPRE antes de escribir código
4. **Escribir código**
5. `dotnet build ApiLaboratorioAgua.sln`
6. `dotnet test` (si existen)
7. `git commit`
8. **Code Reviewer** — SIEMPRE: `@Code Reviewer revisá los cambios`
9. `git push -u origin feat/xxx`
10. `gh pr create --base develop`
11. **Merge en GitHub Web** (vos approves)
12. `gh pr view` — verificar merge
13. **Cerrar issue** #XX
14. `git checkout develop && git pull`

## Deploy a producción (manual, cuando vos lo decidas)

```powershell
dotnet publish ApiLaboratorioAgua/ApiLaboratorioAgua.csproj -c Release -o publish_out
xcopy /E /Y ".\publish_out\*" "C:\Users\tomba\OneDrive\Escritorio\LaboratorioAgua_NEW\Api\"
Stop-Process -Name "ApiLaboratorioAgua" -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 1
Start-Process "C:\Users\tomba\OneDrive\Escritorio\LaboratorioAgua_NEW\Api\ApiLaboratorioAgua.exe"
```

## Convenciones de ramas

| Prefijo | Uso |
|--------|-----|
| `feat/` | Nuevas funcionalidades |
| `fix/` | Bug fixes |
| `refactor/` | Refactors de código |

## Convenciones de código

### API
- Clean Architecture: Domain → Infrastructure → Aplicacion → ApiLaboratorioAgua
- Fechas: `DateTime`, query `yyyy-MM-dd`
- Paginación: `{ items, totalCount, page, pageSize, totalPages, hasNextPage, hasPreviousPage }`
- Result Pattern para errores

### Frontend
- Módulos: `dto.py`, `service.py`, `view.py`
- Tkinter + ttk
- Fechas: `yyyy-MM-dd`
- Paginación: `_prev_page()`, `_next_page()`, `_buscando_por_fecha`