using Infrastructure.Dtos;
using Infrastructure.MyExeptions;
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

        public async Task<ClienteResponseDto> GetClienteByIdAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
            {
                throw new NotFoundException($"Cliente con ID {id} no encontrado.");
            }

            var muestras = await _muestraRepository.GetByClienteIdAsync(id);
            return new ClienteResponseDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,             
                Telefono = cliente.Telefono,
                Email = cliente.Email,
                NumeroMuestras = muestras?.Count ?? 0
            };
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
        public async Task UpdateClienteAsync(ClientesDto clienteDto)
        {
            var cliente = await _clienteRepository.GetByIdAsync(clienteDto.Id);
            if (cliente == null)
                throw new NotFoundException($"Cliente con ID {clienteDto.Id} no encontrado.");

            cliente.Nombre = clienteDto.Nombre;
            cliente.Telefono = clienteDto.Telefono;
            cliente.Email = clienteDto.Email;

            await _clienteRepository.UpdateAsync(cliente);
        }

        public async Task DeleteClienteAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                throw new NotFoundException($"Cliente con ID {id} no encontrado.");

            await _clienteRepository.DeleteAsync(id);
        }
    }
}
