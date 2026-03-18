using Infrastructure.Dtos;
using Dominio.IRepository;
using Infrastructure.MyExeptions;
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

            var reporte = new ReporteLibroDto
            {
                LibroId = libro.Id,
                FechaRegistro = libro.Fecha,
                FechaLlegada = libro.FechaLLegada,
                FechaAnalisis = libro.FechaAnalisis,
                Procedencia = libro.Procedencia,
                Observaciones = libro.Observaciones
            };

            if (libro.Muestras != null)
            {
                foreach (var muestraCompleta in libro.Muestras)
                {
                    var muestraDto = new ReporteMuestraDto
                    {
                        MuestraId = muestraCompleta.Id,
                        Procedencia = muestraCompleta.Procedencia,
                        SitioExtraccion = muestraCompleta.SitioExtraccion,
                        NombreMuestreador = muestraCompleta.NombreMuestreador,
                        TipoMuestra = muestraCompleta.TipoMuestra switch
                        {
                            TipoMuestra.Bacteriologica => TipoDeMuestraDto.Bacteriologica,
                            TipoMuestra.FisicoQuimica => TipoDeMuestraDto.FisicoQuimica,
                            _ => throw new ArgumentException("Tipo de muestra no v�lido.")
                        },
                        ClienteId = muestraCompleta.ClienteId,
                        ClienteNombre = muestraCompleta.Cliente?.Nombre
                    };

                    // Adjuntar resultados si existen
                    if (muestraCompleta.Bacteriologia != null)
                    {
                        muestraDto.Bacteriologia = new BacteriologicoDto
                        {
                            Id = muestraCompleta.Bacteriologia.Id,
                            Fecha = muestraCompleta.Bacteriologia.Fecha,
                            FechaLLegada = muestraCompleta.Bacteriologia.FechaLLegada,
                            FechaAnalisis = muestraCompleta.Bacteriologia.FechaAnalisis,
                            Procedencia = muestraCompleta.Bacteriologia.Procedencia,
                            ColiformesNmp = muestraCompleta.Bacteriologia.ColiformesNmp,
                            ColiformesFecalesNmp = muestraCompleta.Bacteriologia.ColiformesFecalesNmp,
                            ColoniasAgar = muestraCompleta.Bacteriologia.ColoniasAgar,
                            ColiFecalesUfc = muestraCompleta.Bacteriologia.ColiFecalesUfc,
                            Observaciones = muestraCompleta.Bacteriologia.Observaciones,
                            MuestraId = muestraCompleta.Bacteriologia.MuestraId
                        };
                    }

                    if (muestraCompleta.FisicoQuimico != null)
                    {
                        muestraDto.FisicoQuimico = new FisicoQuimicoDto
                        {
                            Id = muestraCompleta.FisicoQuimico.Id,
                            Fecha = muestraCompleta.FisicoQuimico.Fecha,
                            FechaLLegada = muestraCompleta.FisicoQuimico.FechaLLegada,
                            FechaAnalisis = muestraCompleta.FisicoQuimico.FechaAnalisis,
                            Procedencia = muestraCompleta.FisicoQuimico.Procedencia,
                            Ph = muestraCompleta.FisicoQuimico.Ph,
                            Turbidez = muestraCompleta.FisicoQuimico.Turbidez,
                            Alcalinidad = muestraCompleta.FisicoQuimico.Alcalinidad,
                            Dureza = muestraCompleta.FisicoQuimico.Dureza,
                            Nitritos = muestraCompleta.FisicoQuimico.Nitritos,
                            Cloruros = muestraCompleta.FisicoQuimico.Cloruros,
                            Calcio = muestraCompleta.FisicoQuimico.Calcio,
                            Magnesio = muestraCompleta.FisicoQuimico.Magnesio,
                            Dbo5 = muestraCompleta.FisicoQuimico.Dbo5,
                            MuestraId = muestraCompleta.FisicoQuimico.MuestraId
                        };
                    }

                    reporte.Muestras.Add(muestraDto);
                }
            }

            return reporte;
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
                                c2.Item().Text($"Muestreador: {m.NombreMuestreador}");
                                c2.Item().Text($"Cliente: {m.ClienteNombre} (ID {m.ClienteId})");

                                if (m.Bacteriologia != null)
                                {
                                    c2.Item().Text("-- Bacteriología --");
                                    c2.Item().Text($"Coliformes NMP: {m.Bacteriologia.ColiformesNmp}");
                                    c2.Item().Text($"Coliformes Fecales NMP: {m.Bacteriologia.ColiformesFecalesNmp}");
                                    c2.Item().Text($"Colonias Agar: {m.Bacteriologia.ColoniasAgar}");
                                    c2.Item().Text($"Coli Fecales UFC: {m.Bacteriologia.ColiFecalesUfc}");
                                    c2.Item().Text($"Observaciones: {m.Bacteriologia.Observaciones}");
                                }

                                if (m.FisicoQuimico != null)
                                {
                                    c2.Item().Text("-- Fisicoquímico --");
                                    c2.Item().Text($"pH: {m.FisicoQuimico.Ph}");
                                    c2.Item().Text($"Turbidez: {m.FisicoQuimico.Turbidez}");
                                    c2.Item().Text($"Alcalinidad: {m.FisicoQuimico.Alcalinidad}");
                                    c2.Item().Text($"Dureza: {m.FisicoQuimico.Dureza}");
                                    c2.Item().Text($"Nitritos: {m.FisicoQuimico.Nitritos}");
                                    c2.Item().Text($"Cloruros: {m.FisicoQuimico.Cloruros}");
                                    c2.Item().Text($"Calcio: {m.FisicoQuimico.Calcio}");
                                    c2.Item().Text($"Magnesio: {m.FisicoQuimico.Magnesio}");
                                    c2.Item().Text($"DBO5: {m.FisicoQuimico.Dbo5}");
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
    }
}