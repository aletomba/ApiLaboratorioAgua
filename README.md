# ApiLaboratorioAgua

API REST para gestión de análisis de calidad del agua potable desarrollada con **ASP.NET Core 8** y **Entity Framework Core** con SQLite.

## Funcionalidades

- Gestión de **clientes** (municipios / entes)
- **Libro de entradas** de muestras con análisis bacteriológicos y físicoquímicos
- **Planilla diaria** de planta potabilizadora (4 puntos de muestreo + ensayo de jarras)
- Generación de **reportes PDF** con QuestPDF
- Logs estructurados con **Serilog**

## Tecnologías

| Tecnología | Versión |
|---|---|
| .NET | 8 |
| ASP.NET Core | 8 |
| Entity Framework Core | 8 |
| SQLite | - |
| QuestPDF | 2024.x |
| Serilog | - |

## Estructura del proyecto

```
ApiLaboratorioAgua/          ← Web API (Controllers, Program.cs)
Aplicacion/                  ← Servicios de aplicación
Domain/                      ← Entidades y contratos (IRepository)
Infrastructure/              ← EF Core, Repositorios, DTOs, Migraciones
ApiLaboratorioAgua.test/     ← Tests unitarios
```

## Primeros pasos

### Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Configuración

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/aletomba/ApiLaboratorioAgua.git
   cd ApiLaboratorioAgua
   ```

2. Copiar el archivo de configuración de ejemplo:
   ```bash
   cp ApiLaboratorioAgua/appsettings.example.json ApiLaboratorioAgua/appsettings.json
   ```
   Editá `appsettings.json` si querés cambiar el nombre/ruta de la base de datos SQLite.

3. Aplicar migraciones y arrancar:
   ```bash
   cd ApiLaboratorioAgua
   dotnet ef database update
   dotnet run
   ```

   La API quedará disponible en `http://localhost:5000`.

### Swagger / OpenAPI

Navegá a `http://localhost:5000/swagger` para explorar todos los endpoints.

## Endpoints principales

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/libroEntrada` | Listado paginado |
| POST | `/api/libroEntrada/registrar` | Registrar nuevo libro |
| PUT | `/api/libroEntrada/{id}` | Actualizar |
| DELETE | `/api/libroEntrada/{id}` | Eliminar |
| GET | `/api/libroEntrada/{id}/reporte` | Descargar PDF |
| POST | `/api/PlanillaDiaria/registrar` | Registrar planilla diaria |
| GET | `/api/Bacteriologico` | Análisis bacteriológicos |
| GET | `/api/FisicoQuimico` | Análisis físicoquímicos |

## Licencia

MIT
