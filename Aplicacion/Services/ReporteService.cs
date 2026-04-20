using System.Globalization;
using Aplicacion.Mappers;
using Infrastructure.Dtos;
using Dominio.IRepository;
using Dominio.Exceptions;
using Dominio.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Aplicacion.Services
{
    public class ReporteService
    {
        private readonly ILibroEntradaRepository _libroEntradaRepository;

        public ReporteService(ILibroEntradaRepository libroEntradaRepository)
        {
            _libroEntradaRepository = libroEntradaRepository;
        }

        public async Task<ReporteLibroDto> GenerarReportePorLibroIdAsync(int libroId)
        {
            var libro = await _libroEntradaRepository.GetByIdAsync(libroId);
            if (libro == null)
                throw new NotFoundException($"Libro de entrada con ID {libroId} no encontrado.");

            return libro.ToReporteLibroDto();
        }

        public async Task<byte[]> GenerarPdfBytesAsync(int libroId)
        {
            var reporte = await GenerarReportePorLibroIdAsync(libroId);
            return GeneratePdfBytes(reporte);
        }

        private static byte[] GeneratePdfBytes(ReporteLibroDto reporte)
        {
            var muestras = reporte.Muestras.ToList();
            var bactSamples = muestras.Where(m => m.Bacteriologia != null).ToList();
            var fqSamples = muestras.Where(m => m.FisicoQuimico != null).ToList();

            var doc = Document.Create(container =>
            {
                if (bactSamples.Any())
                {
                    GenerarPaginaBacteriologia(container, reporte, bactSamples);
                }

                if (fqSamples.Any())
                {
                    GenerarPaginaFisicoQuimico(container, reporte, fqSamples);
                }

                if (!bactSamples.Any() && !fqSamples.Any())
                {
                    GenerarPaginaVacia(container, reporte);
                }
            });

            return doc.GeneratePdf();
        }

        private static void GenerarPaginaBacteriologia(IDocumentContainer container, ReporteLibroDto reporte, List<ReporteMuestraDto> muestras)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(15);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Column(h =>
                {
                    h.Item().Text($"ANÁLISIS BACTERIOLÓGICO - Libro #{reporte.LibroId}").FontSize(14).Bold().AlignCenter();
                    h.Item().Text($"Fecha: {reporte.FechaRegistro:yyyy-MM-dd} | Procedencia: {reporte.Procedencia}").FontSize(10).AlignCenter();
                });

                page.Content().Column(col =>
                {
                    col.Item().PaddingTop(10).Text("MUESTRAS").Bold().FontSize(10);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c => { c.ConstantColumn(120); c.RelativeColumn(); });
                        table.Header(h =>
                        {
                            h.Cell().Background(Colors.Grey.Darken2).Padding(5).Text("Sitio").FontColor(Colors.White).Bold();
                            h.Cell().Background(Colors.Grey.Darken2).Padding(5).Text("Resultado").FontColor(Colors.White).Bold();
                        });

                        foreach (var m in muestras)
                        {
                            var b = m.Bacteriologia;
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Column(cx =>
                            {
                                cx.Item().Text(m.SitioExtraccion).Bold();
                                cx.Item().Text($"Muestreador: {m.NombreMuestreador}").FontSize(8);
                                cx.Item().Text($"Hora: {m.HoraExtraccion:hh\\:mm}").FontSize(8);
                            });
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Column(cx =>
                            {
                                cx.Item().Text($"Coliformes NMP: {b?.ColiformesNmp ?? "-"}");
                                cx.Item().Text($"Coliformes Fecales: {b?.ColiformesFecalesNmp ?? "-"}");
                                cx.Item().Text($"Colonias Agar: {b?.ColoniasAgar ?? "-"}");
                                cx.Item().Text($"Coli Fecales UFC: {b?.ColiFecalesUfc ?? "-"}");
                            });
                        }
                    });
                });

                page.Footer().AlignCenter().Text($"Generado: {DateTime.Now:yyyy-MM-dd HH:mm}");
            });
        }

        private static void GenerarPaginaFisicoQuimico(IDocumentContainer container, ReporteLibroDto reporte, List<ReporteMuestraDto> muestras)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(15);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Column(h =>
                {
                    h.Item().Text($"ANÁLISIS FÍSICOQUÍMICO - Libro #{reporte.LibroId}").FontSize(14).Bold().AlignCenter();
                    h.Item().Text($"Fecha: {reporte.FechaRegistro:yyyy-MM-dd} | Procedencia: {reporte.Procedencia}").FontSize(10).AlignCenter();
                });

                page.Content().Column(col =>
                {
                    col.Item().PaddingTop(10).Text("MUESTRAS").Bold().FontSize(10);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c => { c.ConstantColumn(120); c.RelativeColumn(); });
                        table.Header(h =>
                        {
                            h.Cell().Background(Colors.Grey.Darken2).Padding(5).Text("Sitio").FontColor(Colors.White).Bold();
                            h.Cell().Background(Colors.Grey.Darken2).Padding(5).Text("Resultado").FontColor(Colors.White).Bold();
                        });

                        foreach (var m in muestras)
                        {
                            var f = m.FisicoQuimico;
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Column(cx =>
                            {
                                cx.Item().Text(m.SitioExtraccion).Bold();
                                cx.Item().Text($"Muestreador: {m.NombreMuestreador}").FontSize(8);
                                cx.Item().Text($"Hora: {m.HoraExtraccion:hh\\:mm}").FontSize(8);
                            });
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Column(cx =>
                            {
                                cx.Item().Text($"pH: {f?.Ph ?? "-"}");
                                cx.Item().Text($"Turbidez: {f?.Turbidez ?? "-"} NTU");
                                cx.Item().Text($"Cloro: {f?.Cloro ?? "-"} mg/L");
                                cx.Item().Text($"Alcalinidad: {f?.Alcalinidad ?? "-"} mg/L");
                                cx.Item().Text($"Dureza: {f?.Dureza ?? "-"} mg/L");
                                cx.Item().Text($"Nitritos: {f?.Nitritos ?? "-"} mg/L");
                                cx.Item().Text($"Cloruros: {f?.Cloruros ?? "-"} mg/L");
                                cx.Item().Text($"Calcio: {f?.Calcio ?? "-"} mg/L");
                                cx.Item().Text($"Magnesio: {f?.Magnesio ?? "-"} mg/L");
                                cx.Item().Text($"DBO5: {f?.Dbo5 ?? "-"} mg/L");
                            });
                        }
                    });
                });

                page.Footer().AlignCenter().Text($"Generado: {DateTime.Now:yyyy-MM-dd HH:mm}");
            });
        }

        private static void GenerarPaginaVacia(IDocumentContainer container, ReporteLibroDto reporte)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Text($"Libro #{reporte.LibroId}").FontSize(20).Bold().AlignCenter();

                page.Content().Column(col =>
                {
                    col.Item().Text($"Fecha: {reporte.FechaRegistro:yyyy-MM-dd}");
                    col.Item().Text($"Procedencia: {reporte.Procedencia}");
                    col.Item().PaddingTop(20).Text("Sin muestras.").Italic();
                });
            });
        }
    }
}