using Infrastructure.Dtos;
using Aplicacion.Services;
using Microsoft.AspNetCore.Mvc;
using ApiLaboratorioAgua.Filters;

namespace ApiLaboratorioAgua.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BacteriologicoController : ControllerBase
    {
        private readonly BacteriologicoService _service;

        public BacteriologicoController(BacteriologicoService service)
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

        [HttpGet("por-fecha")]
        [ValidatePagination]
        public async Task<IActionResult> GetPorFecha(
            [FromQuery] DateTime desde,
            [FromQuery] DateTime hasta,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (desde > hasta)
                return BadRequest("La fecha 'desde' no puede ser mayor que 'hasta'.");
            var result = await _service.GetByFechaRangoPagedAsync(desde, hasta, page, pageSize);
            return Ok(result);
        }

        [HttpGet("por-cliente/{clienteId:int}")]
        [ValidatePagination]
        public async Task<IActionResult> GetPorCliente(
            int clienteId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var result = await _service.GetByClienteIdPagedAsync(clienteId, page, pageSize);
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BacteriologicoDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BacteriologicoDto dto)
        {
            if (id != dto.Id) return BadRequest("El id de la url no coincide con el cuerpo");
            var result = await _service.UpdateAsync(dto);
            if (result.IsFailure)
                return BadRequest(new { error = result.Error });
            return Ok(new { message = "Bacteriologico actualizado" });
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
