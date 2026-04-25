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

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarMuestra([FromBody] MuestraDto muestraDto)
        {
            var result = await _muestraService.RegistrarMuestraAsync(muestraDto);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok(result.Value);
        }

        [HttpGet("por-cliente/{clienteId}")]
        public async Task<IActionResult> GetMuestrasPorCliente(int clienteId)
        {
            var result = await _muestraService.GetMuestrasPorClienteAsync(clienteId);
            if (!result.IsSuccess)
                return NotFound(result.Error);
            return Ok(result.Value);
        }
    }
}
