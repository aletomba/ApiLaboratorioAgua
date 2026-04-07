# ── Stage 1: Build ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar .sln y .csproj primero (mejor cache de layers en restore)
COPY ApiLaboratorioAgua.sln ./
COPY ApiLaboratorioAgua/ApiLaboratorioAgua.csproj ApiLaboratorioAgua/
COPY Aplicacion/Aplicacion.csproj Aplicacion/
COPY Domain/Dominio.csproj Domain/
COPY Infrastructure/Infrastructure.csproj Infrastructure/

RUN dotnet restore

# Copiar el resto del código y publicar
COPY . .
RUN dotnet publish ApiLaboratorioAgua/ApiLaboratorioAgua.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Stage 2: Runtime ───────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Directorio para la base de datos SQLite (montado como volumen)
RUN mkdir -p /app/data

COPY --from=build /app/publish .

EXPOSE 5261

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5261
ENV ConnectionStrings__defaultConnection="Data Source=/app/data/LabAgua.db"

ENTRYPOINT ["dotnet", "ApiLaboratorioAgua.dll"]
