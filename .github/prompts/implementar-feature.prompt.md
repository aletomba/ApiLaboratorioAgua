---
description: "Implementar una feature o fix en ApiLaboratorioAgua o AppPlanillaPlantaPot siguiendo el git flow del AGENTS.md: feat branch → Context7 → código → build → commit → Code Reviewer → PRs → deploy"
name: "Implementar Feature (AGENTS.md)"
argument-hint: "Descripción del cambio a implementar"
agent: "agent"
---

Implementá el siguiente cambio siguiendo **estrictamente** el flujo definido en [AGENTS.md](../../AGENTS.md):

**Tarea:** $input

---

## Pasos obligatorios

### 1. Preparar rama
- `git checkout develop && git pull`
- Crear rama `feat/<nombre-corto>` desde `develop`

### 2. Consultar Context7 MCP (OBLIGATORIO antes de escribir código)
- `context7_resolve-library-id` para las librerías involucradas
- `context7_query-docs` para consultar la API actualizada
- Aplica a: EF Core, ASP.NET, QuestPDF, Serilog, xUnit, Tkinter, ReportLab, requests

### 3. Implementar el cambio
- Leer los archivos relevantes antes de modificar
- Aplicar solo lo que fue pedido (no agregar features extra, no refactorizar lo no solicitado)
- Si es API (.NET): respetar Clean Architecture (Domain → Infrastructure → Aplicacion → ApiLaboratorioAgua)
- Si es Frontend (Python): respetar estructura `dto.py` / `service.py` / `view.py` por módulo

### 4. Verificar
- **API**: `dotnet build ApiLaboratorioAgua.sln` → debe terminar con 0 errores
- **Frontend**: verificar imports y sintaxis Python

### 5. Commit y push
```
git add <archivos>
git commit -m "feat: <descripción concisa>"
git push -u origin feat/<nombre>
```

### 6. Code Reviewer
Invocá: `@Code Reviewer revisá los cambios de esta rama antes del PR`
Aplicá todas las mejoras sugeridas y volvé a hacer commit/push.

### 7. PR feat → develop
```
gh pr create --base develop --title "<título>" --body "<descripción>"
gh pr merge <número> --squash --delete-branch
```

### 8. PR develop → main
```
gh pr create --base main --head develop --title "<título>" --body "Merge develop to main"
gh pr merge <número> --squash --admin
```

### 9. Dejar en develop
```
git checkout develop && git pull
```

### 10. Deploy a producción

**API (.NET):**
```powershell
dotnet publish ApiLaboratorioAgua/ApiLaboratorioAgua.csproj -c Release -o publish_out
xcopy /E /Y ".\publish_out\*" "C:\Users\tomba\OneDrive\Escritorio\LaboratorioAgua_NEW\Api\"
Stop-Process -Name "ApiLaboratorioAgua" -Force -ErrorAction SilentlyContinue
Start-Process "C:\Users\tomba\OneDrive\Escritorio\LaboratorioAgua_NEW\Api\ApiLaboratorioAgua.exe" -WorkingDirectory "C:\Users\tomba\OneDrive\Escritorio\LaboratorioAgua_NEW\Api"
```

**Frontend (Python):**
```powershell
cd "C:\Users\tomba\OneDrive\Escritorio\AppPlanillaPlantaPot"
.\Update.bat
```
