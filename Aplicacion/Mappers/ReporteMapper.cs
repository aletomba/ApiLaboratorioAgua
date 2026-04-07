using Dominio.Entities;
using Infrastructure.Dtos;

namespace Aplicacion.Mappers
{
    public static class ReporteMapper
    {
        public static ReporteMuestraDto ToReporteMuestraDto(this Muestra m)
        {
            return new ReporteMuestraDto
            {
                MuestraId = m.Id,
                Procedencia = m.Procedencia,
                SitioExtraccion = m.Procedencia,
                NombreMuestreador = m.NombreMuestreador,
                TipoMuestra = m.TipoMuestra switch
                {
                    TipoMuestra.Bacteriologica => TipoDeMuestraDto.Bacteriologica,
                    TipoMuestra.FisicoQuimica => TipoDeMuestraDto.FisicoQuimica,
                    _ => throw new ArgumentException("Tipo de muestra no válido.")
                },
                ClienteId = m.ClienteId,
                ClienteNombre = m.Cliente?.Nombre,
                Bacteriologia = m.Bacteriologia?.ToDto(),
                FisicoQuimico = m.FisicoQuimico?.ToDto()
            };
        }

        public static ReporteLibroDto ToReporteLibroDto(this LibroDeEntrada libro)
        {
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
                reporte.Muestras.AddRange(libro.Muestras.Select(m => m.ToReporteMuestraDto()));

            return reporte;
        }
    }
}
