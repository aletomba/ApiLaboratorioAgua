using Dominio.Entities;
using Infrastructure.Dtos;

namespace Aplicacion.Factories
{
    public static class BacteriologicoFactory
    {
        public static Bacteriologico Create(BacteriologicoDto dto)
            => new Bacteriologico
            {
                Fecha = dto.Fecha == default ? DateTime.Now : dto.Fecha,
                FechaLLegada = dto.FechaLLegada == default ? DateTime.Now : dto.FechaLLegada,
                FechaAnalisis = dto.FechaAnalisis == default ? DateTime.Now : dto.FechaAnalisis,
                Procedencia = dto.Procedencia,
                ColiformesNmp = dto.ColiformesNmp,
                ColiformesFecalesNmp = dto.ColiformesFecalesNmp,
                ColoniasAgar = dto.ColoniasAgar,
                ColiFecalesUfc = dto.ColiFecalesUfc,
                Observaciones = dto.Observaciones,
                MuestraId = dto.MuestraId
            };

        public static void Update(Bacteriologico entity, BacteriologicoDto dto)
        {
            entity.ColiformesNmp = dto.ColiformesNmp;
            entity.ColiformesFecalesNmp = dto.ColiformesFecalesNmp;
            entity.ColoniasAgar = dto.ColoniasAgar;
            entity.ColiFecalesUfc = dto.ColiFecalesUfc;
            entity.Observaciones = dto.Observaciones;
        }
    }
}
