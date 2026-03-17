using Infrastructure.Dtos;
using Dominio;
using Dominio.Entities;
using Dominio.IRepository;

namespace Aplicacion.Services
{
    public class ClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IMuestraRepository _muestraRepository;

        public ClienteService(
            IClienteRepository clienteRepository,
            IMuestraRepository muestraRepository)
        {
            _clienteRepository = clienteRepository;
            _muestraRepository = muestraRepository;
        }

        public async Task RegistrarClienteAsync(ClientesDto clienteDto)
        {
            var cliente = new Cliente
            {
                Nombre = clienteDto.Nombre,                
                Telefono = clienteDto.Telefono,
                Email = clienteDto.Email
            };

            await _clienteRepository.AddAsync(cliente);
        }

        public async Task<Result<ClienteResponseDto>> GetClienteByIdAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                return Result<ClienteResponseDto>.Failure($"Cliente con ID {id} no encontrado.");

            var muestras = await _muestraRepository.GetByClienteIdAsync(id);
            return Result<ClienteResponseDto>.Success(new ClienteResponseDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Telefono = cliente.Telefono,
                Email = cliente.Email,
                NumeroMuestras = muestras?.Count ?? 0
            });
        }

        public async Task<List<ClienteResponseDto>> GetAllClientesAsync()
        {
            var clientes = await _clienteRepository.GetAllAsync();
            var result = new List<ClienteResponseDto>();

            foreach (var cliente in clientes)
            {
                var muestras = await _muestraRepository.GetByClienteIdAsync(cliente.Id);
                result.Add(new ClienteResponseDto
                {
                    Id = cliente.Id,
                    Nombre = cliente.Nombre,                    
                    Telefono = cliente.Telefono,
                    Email = cliente.Email,
                    NumeroMuestras = muestras?.Count ?? 0
                });
            }

            return result;
        }
        public async Task<Result<string>> UpdateClienteAsync(ClientesDto clienteDto)
        {
            var cliente = await _clienteRepository.GetByIdAsync(clienteDto.Id);
            if (cliente == null)
                return Result<string>.Failure($"Cliente con ID {clienteDto.Id} no encontrado.");

            cliente.Nombre = clienteDto.Nombre;
            cliente.Telefono = clienteDto.Telefono;
            cliente.Email = clienteDto.Email;

            await _clienteRepository.UpdateAsync(cliente);
            return Result<string>.Success("Cliente actualizado con éxito.");
        }

        public async Task<Result<string>> DeleteClienteAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                return Result<string>.Failure($"Cliente con ID {id} no encontrado.");

            await _clienteRepository.DeleteAsync(id);
            return Result<string>.Success("Cliente eliminado con éxito.");
        }
    }
}
