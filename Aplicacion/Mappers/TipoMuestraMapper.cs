using Dominio.Entities;
using Infrastructure.Dtos;

namespace Aplicacion.Mappers
{
    public static class TipoMuestraMapper
    {
        public static TipoMuestra ToDomain(this TipoDeMuestraDto dto)
        {
            return dto switch
            {
                TipoDeMuestraDto.Bacteriologica => TipoMuestra.Bacteriologica,
                TipoDeMuestraDto.FisicoQuimica => TipoMuestra.FisicoQuimica,
                _ => throw new ArgumentException("Tipo de muestra no válido.")
            };
        }

        public static TipoDeMuestraDto ToDto(this TipoMuestra domain)
        {
            return domain switch
            {
                TipoMuestra.Bacteriologica => TipoDeMuestraDto.Bacteriologica,
                TipoMuestra.FisicoQuimica => TipoDeMuestraDto.FisicoQuimica,
                _ => throw new ArgumentException("Tipo de muestra no válido.")
            };
        }
    }
}