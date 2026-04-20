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
                    GenerarPaginaBacteriologia(container, reporte, bactSamples);
                if (fqSamples.Any())
                    GenerarPaginaFisicoQuimico(container, reporte, fqSamples);
                if (!bactSamples.Any() && !fqSamples.Any())
                    GenerarPaginaVacia(container, reporte);
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
                    int numCols = muestras.Count + 1;
                    col.Item().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(100);
                            for (int i = 0; i < muestras.Count; i++)
                                c.RelativeColumn();
                        });

                        tabla.Cell().ColumnSpan((uint)numCols).Background(Colors.Blue.Darken2).Padding(5)
                            .Text("METADATOS").Bold().FontColor(Colors.White).FontSize(9);

                        tabla.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Campo").Bold().FontSize(8);
                        foreach (var m in muestras)
                            tabla.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignCenter().Text(m.SitioExtraccion).Bold().FontSize(8);

                        tabla.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Muestreador").Bold().FontSize(8);
                        foreach (var m in muestras)
                            tabla.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignCenter().Text(m.NombreMuestreador ?? "-").FontSize(8);

                        tabla.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Hora").Bold().FontSize(8);
                        foreach (var m in muestras)
                            tabla.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignCenter().Text($"{m.HoraExtraccion:hh\\:mm}").FontSize(8);

                        tabla.Cell().ColumnSpan((uint)numCols).Background(Colors.Teal.Darken2).Padding(5)
                            .Text("RESULTADOS").Bold().FontColor(Colors.White).FontSize(9);

                        AgregarFilaBact(tabla, "Coliformes NMP", muestras.Select(m => m.Bacteriologia?.ColiformesNmp ?? "-").ToList());
                        AgregarFilaBact(tabla, "Coliformes Fecales NMP", muestras.Select(m => m.Bacteriologia?.ColiformesFecalesNmp ?? "-").ToList());
                        AgregarFilaBact(tabla, "Colonias Agar", muestras.Select(m => m.Bacteriologia?.ColoniasAgar ?? "-").ToList());
                        AgregarFilaBact(tabla, "Coli Fecales UFC", muestras.Select(m => m.Bacteriologia?.ColiFecalesUfc ?? "-").ToList());
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
                    int numCols = muestras.Count + 1;
                    col.Item().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(100);
                            for (int i = 0; i < muestras.Count; i++)
                                c.RelativeColumn();
                        });

                        tabla.Cell().ColumnSpan((uint)numCols).Background(Colors.Blue.Darken2).Padding(5)
                            .Text("METADATOS").Bold().FontColor(Colors.White).FontSize(9);

                        tabla.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Campo").Bold().FontSize(8);
                        foreach (var m in muestras)
                            tabla.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignCenter().Text(m.SitioExtraccion).Bold().FontSize(8);

                        tabla.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Muestreador").Bold().FontSize(8);
                        foreach (var m in muestras)
                            tabla.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignCenter().Text(m.NombreMuestreador ?? "-").FontSize(8);

                        tabla.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text("Hora").Bold().FontSize(8);
                        foreach (var m in muestras)
                            tabla.Cell().Background(Colors.Grey.Lighten3).Padding(3).AlignCenter().Text($"{m.HoraExtraccion:hh\\:mm}").FontSize(8);

                        tabla.Cell().ColumnSpan((uint)numCols).Background(Colors.Green.Darken2).Padding(5)
                            .Text("RESULTADOS").Bold().FontColor(Colors.White).FontSize(9);

                        AgregarFilaFq(tabla, "pH", muestras.Select(m => m.FisicoQuimico?.Ph ?? "-").ToList());
                        AgregarFilaFq(tabla, "Turbidez (NTU)", muestras.Select(m => m.FisicoQuimico?.Turbidez ?? "-").ToList());
                        AgregarFilaFq(tabla, "Alcalinidad", muestras.Select(m => m.FisicoQuimico?.Alcalinidad ?? "-").ToList());
                        AgregarFilaFq(tabla, "Dureza", muestras.Select(m => m.FisicoQuimico?.Dureza ?? "-").ToList());
                        AgregarFilaFq(tabla, "Nitritos", muestras.Select(m => m.FisicoQuimico?.Nitritos ?? "-").ToList());
                        AgregarFilaFq(tabla, "Cloruros", muestras.Select(m => m.FisicoQuimico?.Cloruros ?? "-").ToList());
                        AgregarFilaFq(tabla, "Calcio", muestras.Select(m => m.FisicoQuimico?.Calcio ?? "-").ToList());
                        AgregarFilaFq(tabla, "Magnesio", muestras.Select(m => m.FisicoQuimico?.Magnesio ?? "-").ToList());
                        AgregarFilaFq(tabla, "DBO5", muestras.Select(m => m.FisicoQuimico?.Dbo5 ?? "-").ToList());
                    });
                });
                page.Footer().AlignCenter().Text($"Generado: {DateTime.Now:yyyy-MM-dd HH:mm}");
            });
        }

        private static void AgregarFilaBact(QuestPDF.Fluent.TableDescriptor tabla, string etiqueta, List<string> valores)
        {
            tabla.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text(etiqueta).Bold().FontSize(8);
            foreach (var v in valores)
                tabla.Cell().Padding(3).AlignCenter().Text(v).FontSize(8);
        }

        private static void AgregarFilaFq(QuestPDF.Fluent.TableDescriptor tabla, string etiqueta, List<string> valores)
        {
            tabla.Cell().Background(Colors.Grey.Lighten3).Padding(3).Text(etiqueta).Bold().FontSize(8);
            foreach (var v in valores)
                tabla.Cell().Padding(3).AlignCenter().Text(v).FontSize(8);
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