using Dominio.Entities;
using Infrastructure.Dtos;

namespace Aplicacion.Factories
{
    public static class FisicoQuimicoFactory
    {
        public static FisicoQuimico Create(FisicoQuimicoDto dto)
            => new FisicoQuimico
            {
                Fecha = dto.Fecha == default ? DateTime.Now : dto.Fecha,
                FechaLLegada = dto.FechaLLegada == default ? DateTime.Now : dto.FechaLLegada,
                FechaAnalisis = dto.FechaAnalisis == default ? DateTime.Now : dto.FechaAnalisis,
                Procedencia = dto.Procedencia,
                Ph = dto.Ph,
                Turbidez = dto.Turbidez,
                Alcalinidad = dto.Alcalinidad,
                Dureza = dto.Dureza,
                Nitritos = dto.Nitritos,
                Cloruros = dto.Cloruros,
                Calcio = dto.Calcio,
                Magnesio = dto.Magnesio,
                Dbo5 = dto.Dbo5,
                Cloro = dto.Cloro,
                MuestraId = dto.MuestraId
            };

        public static void Update(FisicoQuimico entity, FisicoQuimicoEditDto dto)
        {
            entity.Ph = dto.Ph;
            entity.Turbidez = dto.Turbidez;
            entity.Alcalinidad = dto.Alcalinidad;
            entity.Dureza = dto.Dureza;
            entity.Nitritos = dto.Nitritos;
            entity.Cloruros = dto.Cloruros;
            entity.Calcio = dto.Calcio;
            entity.Magnesio = dto.Magnesio;
            entity.Dbo5 = dto.Dbo5;
            entity.Cloro = dto.Cloro;
        }
    }
}
