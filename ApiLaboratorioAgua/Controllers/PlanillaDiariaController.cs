using Aplicacion.Services;
using Infrastructure.Dtos;
using Microsoft.AspNetCore.Mvc;
using ApiLaboratorioAgua.Filters;

namespace ApiLaboratorioAgua.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanillaDiariaController : ControllerBase
    {
        private readonly PlanillaDiariaService _service;

        public PlanillaDiariaController(PlanillaDiariaService service)
        {
            _service = service;
        }

        [HttpGet]
        [ValidatePagination]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var result = await _service.GetAllPagedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result.IsFailure)
                return NotFound(new { error = result.Error });
            return Ok(result.Value);
        }

        [HttpGet("por-fecha")]
        public async Task<IActionResult> GetByFecha([FromQuery] DateTime fecha)
        {
            var result = await _service.GetByFechaAsync(fecha);
            if (result.IsFailure)
                return NotFound(new { error = result.Error });
            return Ok(result.Value);
        }

        [HttpGet("por-rango")]
        [ValidatePagination]
        public async Task<IActionResult> GetByFechaRango(
            [FromQuery] DateTime desde,
            [FromQuery] DateTime hasta,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (desde > hasta)
                return BadRequest("La fecha 'desde' no puede ser mayor que 'hasta'.");
            var result = await _service.GetByFechaRangoAsync(desde, hasta, page, pageSize);
            return Ok(result);
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar(
            [FromBody] PlanillaDiariaDto dto,
            [FromQuery] int clienteId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.RegistrarAsync(dto, clienteId);
            return Ok(new { message = "Planilla registrada con éxito.", data = result });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PlanillaDiariaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(id, dto);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });
            return Ok(new { message = "Planilla actualizada con éxito." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (result.IsFailure)
                return NotFound(new { error = result.Error });
            return NoContent();
        }
    }
}
