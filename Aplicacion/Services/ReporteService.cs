using Aplicacion.Mappers;
using Infrastructure.Dtos;
using Dominio.IRepository;
using Dominio.Exceptions;
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

        public async Task<List<ReporteResumenLibroDto>> GenerarReporteMultipleAsync(List<int> libroIds)
        {
            var libros = await _libroEntradaRepository.GetByIdsAsync(libroIds);
            if (libros.Count == 0)
                throw new NotFoundException("Ningún libro de entrada encontrado con los IDs proporcionados.");

            return libros.Select(libro =>
            {
                var tipos = libro.Muestras?
                    .Select(m => m.TipoMuestra == Dominio.Entities.TipoMuestra.Bacteriologica ? "Bacteriológica" : "Fisicoquímica")
                    .Distinct()
                    .OrderBy(t => t)
                    .ToList() ?? new List<string>();

                return new ReporteResumenLibroDto
                {
                    LibroId = libro.Id,
                    FechaAnalisis = libro.FechaAnalisis,
                    Procedencia = libro.Procedencia,
                    TiposAnalisis = tipos
                };
            }).ToList();
        }

        public async Task<byte[]> GenerarPdfMultipleBytesAsync(List<int> libroIds)
        {
            var reportes = await GenerarReporteMultipleAsync(libroIds);
            return GeneratePdfMultipleBytes(reportes);
        }

        private static byte[] GeneratePdfBytes(ReporteLibroDto reporte)
        {
            var muestras = reporte.Muestras.ToList();
            var bactSamples = muestras.Where(m => m.Bacteriologia != null).ToList();
            var fqSamples = muestras.Where(m => m.FisicoQuimico != null).ToList();

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Column(h =>
                    {
                        h.Item().Text($"REPORTE LIBRO #{reporte.LibroId}").FontSize(14).Bold().AlignCenter();
                        h.Item().Text($"Fecha llegada: {reporte.FechaLlegada:yyyy-MM-dd} | Fecha análisis: {(reporte.FechaAnalisis.HasValue ? reporte.FechaAnalisis.Value.ToString("yyyy-MM-dd") : "-")} | Procedencia: {reporte.Procedencia}").FontSize(10).AlignCenter();
                        if (!string.IsNullOrEmpty(reporte.NroAnalisis))
                            h.Item().Text($"N° Análisis: {reporte.NroAnalisis}").FontSize(10).Bold().AlignCenter();
                        if (!string.IsNullOrEmpty(reporte.Observaciones))
                            h.Item().Text($"Observaciones: {reporte.Observaciones}").FontSize(9).AlignCenter();
                    });

                    page.Content().Column(col =>
                    {
                        if (bactSamples.Any())
                        {
                            col.Item().Text("ANÁLISIS BACTERIOLÓGICO").FontSize(12).Bold().AlignCenter();
                            col.Item().PaddingVertical(5).Table(tabla =>
                            {
                                int numCols = bactSamples.Count + 1;
                                tabla.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(100);
                                    for (int i = 0; i < bactSamples.Count; i++)
                                        c.RelativeColumn();
                                });

                                tabla.Cell().ColumnSpan((uint)numCols).Background(Colors.Grey.Lighten3).Padding(3)
                                    .Text("METADATOS").Bold().FontSize(8);

                                tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).Text("Sitio Extracción").Bold().FontSize(8);
                                foreach (var m in bactSamples)
                                    tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).AlignCenter().Text(m.SitioExtraccion).FontSize(8);

                                tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).Text("Muestreador").Bold().FontSize(8);
                                foreach (var m in bactSamples)
                                    tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).AlignCenter().Text(m.NombreMuestreador ?? "-").FontSize(8);

                                tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).Text("Hora Extracción").Bold().FontSize(8);
                                foreach (var m in bactSamples)
                                    tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).AlignCenter().Text($"{m.HoraExtraccion:hh\\:mm}").FontSize(8);

                                var hasObs = bactSamples.Any(m => !string.IsNullOrEmpty(m.Bacteriologia?.Observaciones));
                                if (hasObs)
                                {
                                    tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).Text("Observaciones").Bold().FontSize(8);
                                    foreach (var m in bactSamples)
                                        tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).AlignCenter().Text(m.Bacteriologia?.Observaciones ?? "-").FontSize(8);
                                }

                                tabla.Cell().ColumnSpan((uint)numCols).Background(Colors.Blue.Darken2).Padding(5)
                                    .Text("RESULTADOS BACTERIOLÓGICOS").Bold().FontColor(Colors.White).FontSize(9);

                                AgregarFilaBact(tabla, "Coliformes NMP", bactSamples.Select(m => m.Bacteriologia?.ColiformesNmp ?? "-").ToList());
                                AgregarFilaBact(tabla, "Coliformes Fecales NMP", bactSamples.Select(m => m.Bacteriologia?.ColiformesFecalesNmp ?? "-").ToList());
                                AgregarFilaBact(tabla, "Colonias Agar", bactSamples.Select(m => m.Bacteriologia?.ColoniasAgar ?? "-").ToList());
                                AgregarFilaBact(tabla, "Coli Fecales UFC", bactSamples.Select(m => m.Bacteriologia?.ColiFecalesUfc ?? "-").ToList());
                            });
                        }

                        if (fqSamples.Any())
                        {
                            col.Item().PaddingTop(10).Text("ANÁLISIS FÍSICOQUÍMICO").FontSize(12).Bold().AlignCenter();
                            col.Item().PaddingVertical(5).Table(tabla =>
                            {
                                int numCols = fqSamples.Count + 1;
                                tabla.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(100);
                                    for (int i = 0; i < fqSamples.Count; i++)
                                        c.RelativeColumn();
                                });

                                tabla.Cell().ColumnSpan((uint)numCols).Background(Colors.Grey.Lighten3).Padding(3)
                                    .Text("METADATOS").Bold().FontSize(8);

                                tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).Text("Sitio Extracción").Bold().FontSize(8);
                                foreach (var m in fqSamples)
                                    tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).AlignCenter().Text(m.SitioExtraccion).FontSize(8);

                                tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).Text("Muestreador").Bold().FontSize(8);
                                foreach (var m in fqSamples)
                                    tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).AlignCenter().Text(m.NombreMuestreador ?? "-").FontSize(8);

                                tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).Text("Hora Extracción").Bold().FontSize(8);
                                foreach (var m in fqSamples)
                                    tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).AlignCenter().Text($"{m.HoraExtraccion:hh\\:mm}").FontSize(8);

                                tabla.Cell().ColumnSpan((uint)numCols).Background(Colors.Green.Darken2).Padding(5)
                                    .Text("RESULTADOS FÍSICOQUÍMICOS").Bold().FontColor(Colors.White).FontSize(9);

                                AgregarFilaFq(tabla, "pH", fqSamples.Select(m => m.FisicoQuimico?.Ph ?? "-").ToList());
                                AgregarFilaFq(tabla, "Turbidez (NTU)", fqSamples.Select(m => m.FisicoQuimico?.Turbidez ?? "-").ToList());
                                AgregarFilaFq(tabla, "Alcalinidad", fqSamples.Select(m => m.FisicoQuimico?.Alcalinidad ?? "-").ToList());
                                AgregarFilaFq(tabla, "Dureza", fqSamples.Select(m => m.FisicoQuimico?.Dureza ?? "-").ToList());
                                AgregarFilaFq(tabla, "Nitritos", fqSamples.Select(m => m.FisicoQuimico?.Nitritos ?? "-").ToList());
                                AgregarFilaFq(tabla, "Nitratos", fqSamples.Select(m => m.FisicoQuimico?.Nitratos ?? "-").ToList());
                                AgregarFilaFq(tabla, "Amonio", fqSamples.Select(m => m.FisicoQuimico?.Amonio ?? "-").ToList());
                                AgregarFilaFq(tabla, "Cloruros", fqSamples.Select(m => m.FisicoQuimico?.Cloruros ?? "-").ToList());
                                AgregarFilaFq(tabla, "Calcio", fqSamples.Select(m => m.FisicoQuimico?.Calcio ?? "-").ToList());
                                AgregarFilaFq(tabla, "Magnesio", fqSamples.Select(m => m.FisicoQuimico?.Magnesio ?? "-").ToList());
                                AgregarFilaFq(tabla, "DBO5", fqSamples.Select(m => m.FisicoQuimico?.Dbo5 ?? "-").ToList());
                                AgregarFilaFq(tabla, "Cloro", fqSamples.Select(m => m.FisicoQuimico?.Cloro ?? "-").ToList());
                            });
                        }

                        if (!bactSamples.Any() && !fqSamples.Any())
                        {
                            col.Item().PaddingTop(20).Text("Sin muestras con resultados.").Italic();
                        }
                    });

                    page.Footer().AlignCenter().Text($"Generado: {DateTime.Now:yyyy-MM-dd HH:mm}");
                });
            });

            return doc.GeneratePdf();
        }

        private static byte[] GeneratePdfMultipleBytes(List<ReporteResumenLibroDto> reportes)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Column(h =>
                    {
                        h.Item().Text("REPORTE MÚLTIPLE DE LIBROS DE ENTRADA").FontSize(14).Bold().AlignCenter();
                        h.Item().PaddingBottom(5).Text($"Total de libros seleccionados: {reportes.Count}").FontSize(10).AlignCenter();
                    });

                    page.Content().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(40);
                            c.ConstantColumn(120);
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });

                        tabla.Cell().ColumnSpan(4).Background(Colors.Grey.Lighten3).Padding(5)
                            .Text("RESUMEN").Bold().FontSize(10);

                        tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).Text("#").Bold().FontSize(8);
                        tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).Text("Fecha Análisis").Bold().FontSize(8);
                        tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).Text("Procedencia").Bold().FontSize(8);
                        tabla.Cell().Background(Colors.Grey.Lighten1).Padding(3).Text("Tipo(s) de Análisis").Bold().FontSize(8);

                        int index = 1;
                        foreach (var r in reportes)
                        {
                            var bg = index % 2 == 0 ? Colors.Grey.Lighten5 : Colors.White;
                            tabla.Cell().Background(bg).Padding(3).AlignCenter().Text(index.ToString()).FontSize(8);
                            tabla.Cell().Background(bg).Padding(3).Text(r.FechaAnalisis?.ToString("yyyy-MM-dd") ?? "-").FontSize(8);
                            tabla.Cell().Background(bg).Padding(3).Text(r.Procedencia ?? "-").FontSize(8);
                            tabla.Cell().Background(bg).Padding(3).Text(string.Join(", ", r.TiposAnalisis)).FontSize(8);
                            index++;
                        }
                    });

                    page.Footer().AlignCenter().Text($"Generado: {DateTime.Now:yyyy-MM-dd HH:mm}");
                });
            });

            return doc.GeneratePdf();
        }

        private static void AgregarFilaBact(QuestPDF.Fluent.TableDescriptor tabla, string etiqueta, List<string> valores)
            => AgregarFila(tabla, etiqueta, valores);

        private static void AgregarFilaFq(QuestPDF.Fluent.TableDescriptor tabla, string etiqueta, List<string> valores)
            => AgregarFila(tabla, etiqueta, valores);

        private static void AgregarFila(QuestPDF.Fluent.TableDescriptor tabla, string etiqueta, List<string> valores)
        {
            tabla.Cell()
                .Background(Colors.Grey.Lighten3)
                .BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(5).PaddingHorizontal(6)
                .Text(etiqueta).Bold().FontSize(8);
            foreach (var v in valores)
                tabla.Cell()
                    .BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                    .PaddingVertical(5).PaddingHorizontal(6)
                    .AlignCenter().Text(v).FontSize(8);
        }
    }
}