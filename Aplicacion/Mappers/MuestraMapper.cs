using Dominio.Entities;
using Infrastructure.Dtos;

namespace Aplicacion.Mappers
{
    public static class MuestraMapper
    {
        public static MuestraResponseDto ToDto(this Muestra m)
        {
            return new MuestraResponseDto
            {
                Id = m.Id,
                Procedencia = m.Procedencia,
                NombreMuestreador = m.NombreMuestreador,
                Latitud = m.Latitud,
                Longitud = m.Longitud,
                FechaExtraccion = m.FechaExtraccion,
                HoraExtraccion = m.HoraExtraccion,
                TipoMuestra = m.TipoMuestra.ToDto(),
                ClienteId = m.ClienteId,
                ClienteNombre = m.Cliente?.Nombre,
                LibroEntradaId = m.LibroEntradaId
            };
        }
    }
}
