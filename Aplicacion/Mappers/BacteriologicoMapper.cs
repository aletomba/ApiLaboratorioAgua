using Dominio.Entities;
using Infrastructure.Dtos;

namespace Aplicacion.Mappers
{
    public static class BacteriologicoMapper
    {
        public static BacteriologicoDto ToDto(this Bacteriologico b)
        {
            return new BacteriologicoDto
            {
                Id = b.Id,
                Fecha = b.Fecha,
                FechaLLegada = b.FechaLLegada,
                FechaAnalisis = b.FechaAnalisis,
                Procedencia = b.Procedencia,
                ColiformesNmp = b.ColiformesNmp,
                ColiformesFecalesNmp = b.ColiformesFecalesNmp,
                ColoniasAgar = b.ColoniasAgar,
                ColiFecalesUfc = b.ColiFecalesUfc,
                Observaciones = b.Observaciones,
                MuestraId = b.MuestraId
            };
        }
    }
}
