# ApiLaboratorioAgua

API REST para gestión de análisis de calidad del agua potable desarrollada con **ASP.NET Core 8** y **Entity Framework Core** con SQLite.

## Funcionalidades

- Gestión de **clientes** (municipios / entes)
- **Libro de entradas** de muestras con soporte multi-muestra (bacteriológicas y físicoquímicas)
- **Análisis físicoquímico** con campo Cloro y paginación por fecha / cliente
- **Análisis bacteriológico** con paginación por fecha / cliente
- **Planilla diaria** de planta potabilizadora:
  - 4 puntos de muestreo
  - Ensayo de jarras con campos Pre-Cal y Post-Cal (mg/L)
  - Paginación por fecha
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

   La API quedará disponible en `http://localhost:5261`.

### Swagger / OpenAPI

Navegá a `http://localhost:5261/swagger` para explorar todos los endpoints.

## Endpoints principales

### Libro de Entradas

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/libroEntrada` | Listado paginado (`page`, `pageSize`) |
| GET | `/libroEntrada/{id}` | Obtener por ID |
| GET | `/libroEntrada/por-fecha` | Filtrar por rango de fechas (`desde`, `hasta`) |
| GET | `/libroEntrada/por-procedencia` | Filtrar por procedencia |
| POST | `/libroEntrada/registrar` | Registrar nuevo libro (con una o más muestras) |
| PUT | `/libroEntrada/{id}` | Actualizar (agrega / edita / elimina muestras) |
| DELETE | `/libroEntrada/{id}` | Eliminar |
| GET | `/libroEntrada/{id}/reporte` | Descargar PDF |

### Análisis Físicoquímico

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/FisicoQuimico` | Listado paginado |
| GET | `/FisicoQuimico/por-fecha` | Filtrar por rango de fechas |
| GET | `/FisicoQuimico/por-cliente/{clienteId}` | Filtrar por cliente |

### Análisis Bacteriológico

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/Bacteriologico` | Listado paginado |
| GET | `/Bacteriologico/por-fecha` | Filtrar por rango de fechas |
| GET | `/Bacteriologico/por-cliente/{clienteId}` | Filtrar por cliente |

### Planilla Diaria

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/PlanillaDiaria` | Listado paginado |
| GET | `/PlanillaDiaria/por-fecha` | Filtrar por rango de fechas |
| POST | `/PlanillaDiaria/registrar` | Registrar planilla diaria |
| PUT | `/PlanillaDiaria/{id}` | Actualizar |
| DELETE | `/PlanillaDiaria/{id}` | Eliminar |

### Clientes

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/Cliente` | Listado de clientes |
| POST | `/Cliente` | Crear cliente |
| PUT | `/Cliente/{id}` | Actualizar cliente |
| DELETE | `/Cliente/{id}` | Eliminar cliente |

### Respuesta paginada

Todos los endpoints de listado devuelven:

```json
{
  "items": [...],
  "totalCount": 42,
  "page": 1,
  "pageSize": 50,
  "totalPages": 1,
  "hasNextPage": false,
  "hasPreviousPage": false
}
```

## Historial de cambios relevantes

| Versión | Cambio |
|---|---|
| — | Paginación en LibroEntrada, FisicoQuimico, Bacteriologico y PlanillaDiaria |
| — | Campo `Cloro` en análisis físicoquímico |
| — | Campos `PreCal` y `PostCal` (mg/L) en Ensayo de Jarras |
| — | Fix: al añadir varias muestras nuevas en update de LibroEntrada, solo persistía la última |

## Licencia

MIT
