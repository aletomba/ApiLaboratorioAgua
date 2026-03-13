using Infrastructure.Dtos;
using Aplicacion.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiLaboratorioAgua.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MuestraController : ControllerBase
    {
        private readonly MuestraService _muestraService;

        public MuestraController(MuestraService muestraService)
        {
            _muestraService = muestraService;
        }

        /// <summary>
        /// Registra una nueva muestra, creando automáticamente un libro de entrada
        /// y una entrada en LibroBacteriologia o LibroFisicoQuimico según el tipo de muestra.
        /// </summary>
        /// <param name="muestraDto">Datos de la muestra a registrar.</param>
        /// <returns>Confirmación de registro exitoso.</returns>
        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarMuestra([FromBody] MuestraDto muestraDto)
        {
            await _muestraService.RegistrarMuestraAsync(muestraDto);
            return Ok("Muestra registrada con éxito.");
        }

        /// <summary>
        /// Obtiene todas las muestras asociadas a un cliente.
        /// </summary>
        /// <param name="clienteId">ID del cliente.</param>
        /// <returns>Lista de muestras.</returns>
        [HttpGet("por-cliente/{clienteId}")]
        public async Task<IActionResult> GetMuestrasPorCliente(int clienteId)
        {
            var muestras = await _muestraService.GetMuestrasPorClienteAsync(clienteId);
            return Ok(muestras);
        }
    }
}
