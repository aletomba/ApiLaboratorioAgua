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
        private readonly ILibroEntradaQueryRepository _libroEntradaRepository;

        public ReporteService(ILibroEntradaQueryRepository libroEntradaRepository)
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
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text($"Reporte Libro #{reporte.LibroId}")
                        .SemiBold().FontSize(20).AlignCenter();

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Text($"Fecha registro: {reporte.FechaRegistro:yyyy-MM-dd}");
                        col.Item().Text($"Fecha llegada: {reporte.FechaLlegada:yyyy-MM-dd}");
                        col.Item().Text($"Fecha análisis: {(reporte.FechaAnalisis.HasValue ? reporte.FechaAnalisis.Value.ToString("yyyy-MM-dd") : "-")}");
                        col.Item().Text($"Procedencia: {reporte.Procedencia}");
                        col.Item().Text($"Observaciones: {reporte.Observaciones}");

                        col.Item().PaddingTop(10).Text("Muestras:").Bold();

                        foreach (var m in reporte.Muestras)
                        {
                            col.Item().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Column(c2 =>
                            {
                                c2.Item().Text($"Muestra: {m.SitioExtraccion} - Tipo: {m.TipoMuestra}").Bold();
                                AddField(c2, $"Muestreador: {m.NombreMuestreador}");
                                AddField(c2, $"Hora Extracción: {m.HoraExtraccion:hh\\:mm}");
                                AddField(c2, $"Cliente: {m.ClienteNombre} (ID {m.ClienteId})");

                                if (m.Bacteriologia != null)
                                {
                                    c2.Item().PaddingTop(3).Text("-- Bacteriología --").Bold();
                                    AddField(c2, $"Coliformes NMP: {m.Bacteriologia.ColiformesNmp}");
                                    AddField(c2, $"Coliformes Fecales NMP: {m.Bacteriologia.ColiformesFecalesNmp}");
                                    AddField(c2, $"Colonias Agar: {m.Bacteriologia.ColoniasAgar}");
                                    AddField(c2, $"Coli Fecales UFC: {m.Bacteriologia.ColiFecalesUfc}");
                                    AddField(c2, $"Observaciones: {m.Bacteriologia.Observaciones}");
                                }

                                if (m.FisicoQuimico != null)
                                {
                                    c2.Item().PaddingTop(3).Text("-- Fisicoquímico --").Bold();
                                    AddField(c2, $"pH: {m.FisicoQuimico.Ph}");
                                    AddField(c2, $"Turbidez: {m.FisicoQuimico.Turbidez}");
                                    AddField(c2, $"Alcalinidad: {m.FisicoQuimico.Alcalinidad}");
                                    AddField(c2, $"Dureza: {m.FisicoQuimico.Dureza}");
                                    AddField(c2, $"Nitritos: {m.FisicoQuimico.Nitritos}");
                                    AddField(c2, $"Cloruros: {m.FisicoQuimico.Cloruros}");
                                    AddField(c2, $"Calcio: {m.FisicoQuimico.Calcio}");
                                    AddField(c2, $"Magnesio: {m.FisicoQuimico.Magnesio}");
                                    AddField(c2, $"DBO5: {m.FisicoQuimico.Dbo5}");
                                    AddField(c2, $"Cloro: {m.FisicoQuimico.Cloro}");
                                }
                            });
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generado el ");
                        x.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    });
                });
            });

            return doc.GeneratePdf();
        }

        private static void AddField(ColumnDescriptor column, string text)
        {
            column.Item().Text(text);
            column.Item()
                .PaddingVertical(2)
                .LineHorizontal(1)
                .LineColor(Colors.Grey.Lighten3);
        }
    }
}