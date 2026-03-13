using Infrastructure.Dtos;
using Aplicacion.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiLaboratorioAgua.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly ClienteService _clienteService;

        public ClienteController(ClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        /// <summary>
        /// Registra un nuevo cliente.
        /// </summary>
        /// <param name="clienteDto">Datos del cliente a registrar.</param>
        /// <returns>Confirmación de registro exitoso.</returns>
        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarCliente([FromBody] ClientesDto clienteDto)
        {
            await _clienteService.RegistrarClienteAsync(clienteDto);
            return Ok(new { message = "Cliente registrado con éxito." });
        }

        /// <summary>
        /// Obtiene un cliente por su ID.
        /// </summary>
        /// <param name="id">ID del cliente.</param>
        /// <returns>Detalles del cliente.</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCliente(int id)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            return Ok(cliente);
        }

        /// <summary>
        /// Obtiene todos los clientes.
        /// </summary>
        /// <returns>Lista de clientes.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllClientes()
        {
            var clientes = await _clienteService.GetAllClientesAsync();
            return Ok(clientes);
        }

        /// <summary>
        /// Actualiza un cliente existente.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] ClientesDto clienteDto)
        {
            if (id != clienteDto.Id)
                return BadRequest("El ID de la URL no coincide con el del cuerpo de la solicitud.");

            await _clienteService.UpdateClienteAsync(clienteDto);
            return Ok(new { message = "Cliente actualizado con éxito." });
        }

        /// <summary>
        /// Elimina un cliente por su ID.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            await _clienteService.DeleteClienteAsync(id);
            return Ok(new { message = "Cliente eliminado con éxito." });
        }
    }
}
