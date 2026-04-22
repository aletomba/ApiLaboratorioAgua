using Infrastructure.Dtos;
using Aplicacion.Services;
using Dominio.IRepository;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using QuestPDF;
using QuestPDF.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.File(
        path: "logs/api-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Configure QuestPDF license to Community to disable license validation exception
QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar DbContext — SQLite en todos los environments
builder.Services.AddDbContext<LabAguaDbContext>(options =>
{
    var configured = builder.Configuration.GetConnectionString("defaultConnection");
    var fallbackDb = builder.Environment.IsDevelopment() ? "LabAgua_Dev.db" : "LabAgua.db";
    var dbPath = Path.Combine(AppContext.BaseDirectory, fallbackDb);

    var conn = string.IsNullOrWhiteSpace(configured)
        ? $"Data Source={dbPath}"
        : configured;

    // Si la connection string es relativa, anclala a la carpeta del exe
    if (conn.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
    {
        var value = conn.Substring("Data Source=".Length).Trim();
        var isRelative = !Path.IsPathRooted(value);
        if (isRelative)
            conn = $"Data Source={Path.Combine(AppContext.BaseDirectory, value)}";
    }

    options.UseSqlite(conn);
});


builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IMuestraRepository, MuestraRepository>();
builder.Services.AddScoped<ILibroEntradaRepository, LibroDeEntradaRepository>();
builder.Services.AddScoped<ILibroBacteriologiaRepository, BacteriologicoRepository>();
builder.Services.AddScoped<ILibroFisicoQuimicoRepository, FisicoQuimicoRepository>();
builder.Services.AddScoped<IPlanillaDiariaRepository, PlanillaDiariaRepository>();
builder.Services.AddScoped<MuestraService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<LibroDeEntradaService>();
builder.Services.AddScoped<FisicoQuimicoService>();
builder.Services.AddScoped<BacteriologicoService>();
builder.Services.AddScoped<ReporteService>();
builder.Services.AddScoped<PlanillaDiariaService>();
var app = builder.Build();

// Aplicar migraciones automáticas (SQLite en todos los environments)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<LabAguaDbContext>();
    db.Database.Migrate();
}

// Seed datos solo en Development (condicional: solo si la base está vacía)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<LabAguaDbContext>();
        if (!db.Clientes.Any())
        {
        var clienteService = scope.ServiceProvider.GetRequiredService<ClienteService>();
        var libroDeEntradaService = scope.ServiceProvider.GetRequiredService<LibroDeEntradaService>();

        // Crear usuario si no existe
        var clienteDto = new ClientesDto
        {
            Nombre = "Usuario Demo",
            Email = "demo@correo.com",
            Telefono = "123456789"
        };
        await clienteService.RegistrarClienteAsync(clienteDto);

        // Crear libro de entrada con 2 muestras
        var libroEntradaDto = new LibroDeEntradaDto
        {
            Procedencia = "Inicial",
            Observaciones = "Libro de entrada creado automáticamente",
            SitioExtraccion = "Sitio de prueba",
            Muestras = new List<MuestraDto>
            {
                new MuestraDto
                {
                    ClienteId = 1,
                    SitioExtraccion = "Río Paraná",
                    NombreMuestreador = "Juan Pérez",
                    Latitud = -34.6037,
                    Longitud = -58.3816,
                    FechaExtraccion = DateTime.Now.Date,
                    HoraExtraccion = new TimeSpan(10, 0, 0),
                    TipoMuestra = TipoDeMuestraDto.Bacteriologica
                },
                new MuestraDto
                {
                    ClienteId = 1,
                    SitioExtraccion = "Laguna Setúbal",
                    NombreMuestreador = "Ana López",
                    Latitud = -31.6333,
                    Longitud = -60.7000,
                    FechaExtraccion = DateTime.Now.Date,
                    HoraExtraccion = new TimeSpan(11, 30, 0),
                    TipoMuestra = TipoDeMuestraDto.FisicoQuimica
                }
            }
        };
        await libroDeEntradaService.RegistrarLibroEntradaAsync(libroEntradaDto);
        }
    }
}

// Manejo global de excepciones - devuelve JSON en vez de página HTML
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var ex = exceptionFeature?.Error;

        int statusCode = 500;
        string message = "Error interno del servidor.";

        if (ex is Dominio.Exceptions.NotFoundException)
        {
            statusCode = 404;
            message = ex.Message;
        }
        else if (ex is ArgumentException)
        {
            statusCode = 400;
            message = ex.Message;
        }
        else if (ex != null && app.Environment.IsDevelopment())
        {
            message = ex.Message;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        // innerErrors solo en Development para no exponer detalles internos en producción (OWASP)
        if (app.Environment.IsDevelopment())
        {
            var innerMessages = new List<string>();
            var inner = ex?.InnerException;
            while (inner != null)
            {
                innerMessages.Add(inner.Message);
                inner = inner.InnerException;
            }
            await context.Response.WriteAsJsonAsync(new {
                status = statusCode,
                error = message,
                innerErrors = innerMessages
            });
        }
        else
        {
            await context.Response.WriteAsJsonAsync(new {
                status = statusCode,
                error = message
            });
        }
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Solo redirigir a HTTPS si hay endpoint HTTPS configurado
var aspnetcoreUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")
                    ?? builder.Configuration["ASPNETCORE_URLS"];
var hasHttpsEndpoint = !string.IsNullOrWhiteSpace(aspnetcoreUrls)
                       && aspnetcoreUrls.Contains("https://", StringComparison.OrdinalIgnoreCase);

if (hasHttpsEndpoint)
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
