using Dominio.Entities;
using Infrastructure.Dtos;

namespace Aplicacion.Factories
{
    public static class PlanillaDiariaFactory
    {
        public static PuntoMuestreo ParsePuntoMuestreo(PuntoMuestreoDto dto) => dto switch
        {
            PuntoMuestreoDto.AguaNatural => PuntoMuestreo.AguaNatural,
            PuntoMuestreoDto.Decantada   => PuntoMuestreo.Decantada,
            PuntoMuestreoDto.Filtrada    => PuntoMuestreo.Filtrada,
            PuntoMuestreoDto.Consumo     => PuntoMuestreo.Consumo,
            _ => throw new ArgumentException("PuntoMuestreo no válido.")
        };

        public static FisicoQuimico CreateFisicoQuimico(AnalisisPuntoDto analisis, DateTime fecha)
            => new FisicoQuimico
            {
                Fecha = fecha,
                FechaLLegada = fecha,
                FechaAnalisis = fecha,
                Procedencia = analisis.PuntoMuestreo.ToString(),
                SitioExtraccion = analisis.PuntoMuestreo.ToString(),
                Ph = analisis.Ph,
                Turbidez = analisis.Turbidez,
                Alcalinidad = analisis.Alcalinidad,
                Dureza = analisis.Dureza,
                Nitritos = analisis.Nitritos,
                Cloruros = analisis.Cloruros,
                Calcio = analisis.Calcio,
                Magnesio = analisis.Magnesio,
                Dbo5 = analisis.Dbo5,
                Cloro = analisis.Cloro,
            };

        public static Muestra CreateMuestra(AnalisisPuntoDto analisis, PlanillaDiariaDto dto, int clienteId)
        {
            var puntoEntity = ParsePuntoMuestreo(analisis.PuntoMuestreo);
            return new Muestra
            {
                Procedencia = analisis.PuntoMuestreo.ToString(),
                NombreMuestreador = dto.Operador ?? string.Empty,
                Latitud = 0,
                Longitud = 0,
                FechaExtraccion = dto.Fecha,
                HoraExtraccion = TimeSpan.Zero,
                TipoMuestra = TipoMuestra.FisicoQuimica,
                PuntoMuestreo = puntoEntity,
                ClienteId = clienteId,
                FisicoQuimico = CreateFisicoQuimico(analisis, dto.Fecha)
            };
        }

        public static LibroDeEntrada CreateLibroEntrada(PlanillaDiariaDto dto, List<Muestra> muestras)
            => new LibroDeEntrada
            {
                Fecha = dto.Fecha,
                FechaLLegada = dto.Fecha,
                FechaAnalisis = dto.Fecha,
                Procedencia = "Planta Potabilizadora",
                SitioExtraccion = "Planta",
                Observaciones = dto.Observaciones ?? string.Empty,
                Muestras = muestras
            };

        public static EnsayoJarras CreateEnsayoJarras(EnsayoJarrasDto dto)
            => new EnsayoJarras
            {
                Dosis1 = dto.Dosis1,
                Dosis2 = dto.Dosis2,
                Dosis3 = dto.Dosis3,
                Dosis4 = dto.Dosis4,
                Dosis5 = dto.Dosis5,
                DosisSeleccionada = dto.DosisSeleccionada,
                PreCal = dto.PreCal,
                PostCal = dto.PostCal,
                UnidadMedida = "mg/L"
            };

        public static PlanillaDiaria CreatePlanilla(PlanillaDiariaDto dto, int libroEntradaId, EnsayoJarras? ensayo)
            => new PlanillaDiaria
            {
                Fecha = dto.Fecha.Date,
                Operador = dto.Operador,
                Observaciones = dto.Observaciones,
                LibroEntradaId = libroEntradaId,
                EnsayoJarras = ensayo
            };

        public static void UpdateFisicoQuimico(FisicoQuimico fq, AnalisisPuntoDto analisis)
        {
            fq.Ph = analisis.Ph;
            fq.Turbidez = analisis.Turbidez;
            fq.Alcalinidad = analisis.Alcalinidad;
            fq.Dureza = analisis.Dureza;
            fq.Nitritos = analisis.Nitritos;
            fq.Cloruros = analisis.Cloruros;
            fq.Calcio = analisis.Calcio;
            fq.Magnesio = analisis.Magnesio;
            fq.Dbo5 = analisis.Dbo5;
            fq.Cloro = analisis.Cloro;
        }

        public static void UpdateEnsayoJarras(EnsayoJarras ensayo, EnsayoJarrasDto dto)
        {
            ensayo.Dosis1 = dto.Dosis1;
            ensayo.Dosis2 = dto.Dosis2;
            ensayo.Dosis3 = dto.Dosis3;
            ensayo.Dosis4 = dto.Dosis4;
            ensayo.Dosis5 = dto.Dosis5;
            ensayo.DosisSeleccionada = dto.DosisSeleccionada;
            ensayo.PreCal = dto.PreCal;
            ensayo.PostCal = dto.PostCal;
        }
    }
}
