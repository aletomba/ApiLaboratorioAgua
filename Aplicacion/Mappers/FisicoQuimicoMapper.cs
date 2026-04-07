using Dominio.Entities;
using Infrastructure.Dtos;

namespace Aplicacion.Mappers
{
    public static class FisicoQuimicoMapper
    {
        public static FisicoQuimicoDto ToDto(this FisicoQuimico fq)
        {
            return new FisicoQuimicoDto
            {
                Id = fq.Id,
                Fecha = fq.Fecha,
                FechaLLegada = fq.FechaLLegada,
                FechaAnalisis = fq.FechaAnalisis,
                Procedencia = fq.Procedencia,
                Ph = fq.Ph,
                Turbidez = fq.Turbidez,
                Alcalinidad = fq.Alcalinidad,
                Dureza = fq.Dureza,
                Nitritos = fq.Nitritos,
                Cloruros = fq.Cloruros,
                Calcio = fq.Calcio,
                Magnesio = fq.Magnesio,
                Dbo5 = fq.Dbo5,
                Cloro = fq.Cloro,
                MuestraId = fq.MuestraId
            };
        }
    }
}
