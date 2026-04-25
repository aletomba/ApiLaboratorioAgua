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

        /// <summary>Obtiene todas las planillas paginadas.</summary>
        [HttpGet]
        [ValidatePagination]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var result = await _service.GetAllPagedAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>Obtiene una planilla por ID.</summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(new { error = $"Planilla con ID {id} no encontrada." });
            return Ok(result);
        }

        /// <summary>Obtiene la planilla de una fecha específica (yyyy-MM-dd).</summary>
        [HttpGet("por-fecha")]
        public async Task<IActionResult> GetByFecha([FromQuery] DateTime fecha)
        {
            var result = await _service.GetByFechaAsync(fecha);
            return Ok(result);
        }

        /// <summary>Busca planillas por rango de fechas (inclusivo).</summary>
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

        /// <summary>
        /// Registra una nueva planilla diaria.
        /// Crea automáticamente el LibroDeEntrada con una muestra por punto de muestreo.
        /// </summary>
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

        /// <summary>Actualiza una planilla existente (análisis y ensayo de jarras).</summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PlanillaDiariaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpdateAsync(id, dto);
            return Ok(new { message = "Planilla actualizada con éxito." });
        }

        /// <summary>Elimina una planilla.</summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new { message = "Planilla eliminada con éxito." });
        }
    }
}
