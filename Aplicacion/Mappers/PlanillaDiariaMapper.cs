using Dominio.Entities;
using Infrastructure.Dtos;

namespace Aplicacion.Mappers
{
    public static class PlanillaDiariaMapper
    {
        public static PuntoMuestreoDto ToDto(this PuntoMuestreo punto) => punto switch
        {
            PuntoMuestreo.AguaNatural => PuntoMuestreoDto.AguaNatural,
            PuntoMuestreo.Decantada   => PuntoMuestreoDto.Decantada,
            PuntoMuestreo.Filtrada    => PuntoMuestreoDto.Filtrada,
            PuntoMuestreo.Consumo     => PuntoMuestreoDto.Consumo,
            _ => throw new ArgumentException($"PuntoMuestreo '{punto}' no tiene mapeo DTO.")
        };

        public static EnsayoJarrasDto? ToDto(this EnsayoJarras? e)
        {
            if (e == null) return null;
            return new EnsayoJarrasDto
            {
                Id = e.Id,
                Dosis1 = e.Dosis1,
                Dosis2 = e.Dosis2,
                Dosis3 = e.Dosis3,
                Dosis4 = e.Dosis4,
                Dosis5 = e.Dosis5,
                DosisSeleccionada = e.DosisSeleccionada,
                PreCal = e.PreCal,
                PostCal = e.PostCal,
                UnidadMedida = e.UnidadMedida
            };
        }

        public static PlanillaDiariaResponseDto ToDto(this PlanillaDiaria p)
        {
            var analisis = p.LibroEntrada?.Muestras?
                .Where(m => m.PuntoMuestreo.HasValue && m.FisicoQuimico != null)
                .Select(m => new AnalisisPuntoDto
                {
                    PuntoMuestreo = m.PuntoMuestreo!.Value.ToDto(),
                    Ph = m.FisicoQuimico!.Ph,
                    Turbidez = m.FisicoQuimico.Turbidez,
                    Alcalinidad = m.FisicoQuimico.Alcalinidad,
                    Dureza = m.FisicoQuimico.Dureza,
                    Nitritos = m.FisicoQuimico.Nitritos,
                    Cloruros = m.FisicoQuimico.Cloruros,
                    Calcio = m.FisicoQuimico.Calcio,
                    Magnesio = m.FisicoQuimico.Magnesio,
                    Dbo5 = m.FisicoQuimico.Dbo5,
                    Cloro = m.FisicoQuimico.Cloro,
                }).ToList() ?? new List<AnalisisPuntoDto>();

            return new PlanillaDiariaResponseDto
            {
                Id = p.Id,
                Fecha = p.Fecha,
                Operador = p.Operador,
                Observaciones = p.Observaciones,
                LibroEntradaId = p.LibroEntradaId,
                AnalisisPorPunto = analisis,
                EnsayoJarras = p.EnsayoJarras.ToDto()
            };
        }
    }
}
