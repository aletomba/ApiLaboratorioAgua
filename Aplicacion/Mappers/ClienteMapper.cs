using Dominio.Entities;
using Infrastructure.Dtos;

namespace Aplicacion.Mappers
{
    public static class ClienteMapper
    {
        public static ClienteResponseDto ToDto(this Cliente cliente, int numeroMuestras = 0)
        {
            return new ClienteResponseDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Telefono = cliente.Telefono,
                Email = cliente.Email,
                NumeroMuestras = numeroMuestras
            };
        }
    }
}
