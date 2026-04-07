using Dominio.Entities;
using Infrastructure.Dtos;

namespace Aplicacion.Mappers
{
    public static class LibroDeEntradaMapper
    {
        public static LibroDeEntradaResponseDto ToDto(this LibroDeEntrada le)
        {
            return new LibroDeEntradaResponseDto
            {
                Id = le.Id,
                FechaRegistro = le.Fecha,
                FechaLlegada = le.FechaLLegada,
                FechaAnalisis = le.FechaAnalisis,
                Procedencia = le.Procedencia,
                SitioExtraccion = le.SitioExtraccion,
                Observaciones = le.Observaciones,
                Muestras = le.Muestras?.Select(m => m.ToDto()).ToList()
                            ?? new List<MuestraResponseDto>()
            };
        }
    }
}
