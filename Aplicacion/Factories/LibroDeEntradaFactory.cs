using Dominio.Entities;
using Infrastructure.Dtos;
using Aplicacion.Mappers;

namespace Aplicacion.Factories
{
    public static class LibroDeEntradaFactory
    {
        public static TipoMuestra ParseTipoMuestra(TipoDeMuestraDto tipoDto)
            => tipoDto.ToDomain();

        public static Muestra CreateMuestra(MuestraDto muestraDto, LibroDeEntradaDto libroDto, string? procedencia)
        {
            var tipoMuestra = muestraDto.TipoMuestra.ToDomain();
            var muestra = new Muestra
            {
                Procedencia = procedencia,
                NombreMuestreador = muestraDto.NombreMuestreador,
                Latitud = muestraDto.Latitud,
                Longitud = muestraDto.Longitud,
                FechaExtraccion = muestraDto.FechaExtraccion,
                HoraExtraccion = muestraDto.HoraExtraccion,
                TipoMuestra = tipoMuestra,
                ClienteId = muestraDto.ClienteId,
                PuntoMuestreo = muestraDto.PuntoMuestreo.HasValue
                    ? (PuntoMuestreo)muestraDto.PuntoMuestreo.Value
                    : null
            };

            if (tipoMuestra == TipoMuestra.Bacteriologica)
                muestra.Bacteriologia = CreateBacteriologico(libroDto, muestraDto, muestra);
            else
                muestra.FisicoQuimico = CreateFisicoQuimico(libroDto, muestraDto, muestra);

            return muestra;
        }

        public static Bacteriologico CreateBacteriologico(LibroDeEntradaDto libroDto, MuestraDto muestraDto, Muestra muestra)
            => new Bacteriologico
            {
                Fecha = libroDto.Fecha,
                FechaAnalisis = libroDto.FechaAnalisis,
                FechaLLegada = libroDto.FechaLLegada,
                Procedencia = libroDto.Procedencia,
                SitioExtraccion = muestraDto.SitioExtraccion ?? string.Empty,
                ColiformesNmp = string.Empty,
                ColiformesFecalesNmp = string.Empty,
                ColoniasAgar = string.Empty,
                ColiFecalesUfc = string.Empty,
                Observaciones = string.Empty,
                Muestra = muestra
            };

        public static FisicoQuimico CreateFisicoQuimico(LibroDeEntradaDto libroDto, MuestraDto muestraDto, Muestra muestra)
            => new FisicoQuimico
            {
                Fecha = libroDto.Fecha,
                FechaAnalisis = libroDto.FechaAnalisis,
                FechaLLegada = libroDto.FechaLLegada,
                Procedencia = libroDto.Procedencia,
                SitioExtraccion = muestraDto.SitioExtraccion ?? string.Empty,
                Ph = string.Empty,
                Turbidez = string.Empty,
                Alcalinidad = string.Empty,
                Dureza = string.Empty,
                Nitritos = string.Empty,
                Cloruros = string.Empty,
                Calcio = string.Empty,
                Magnesio = string.Empty,
                Dbo5 = string.Empty,
                Cloro = string.Empty,
                Muestra = muestra
            };

        public static LibroDeEntrada CreateLibroEntrada(LibroDeEntradaDto dto, List<Muestra> muestras)
        {
            var libro = new LibroDeEntrada
            {
                Fecha = dto.Fecha,
                FechaLLegada = dto.FechaLLegada,
                FechaAnalisis = dto.FechaAnalisis,
                Procedencia = dto.Procedencia,
                SitioExtraccion = dto.SitioExtraccion ?? string.Empty,
                Observaciones = dto.Observaciones,
                Muestras = muestras
            };

            foreach (var muestra in muestras)
                muestra.LibroEntrada = libro;

            return libro;
        }
    }
}
