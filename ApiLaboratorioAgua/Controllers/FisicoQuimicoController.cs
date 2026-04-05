using Infrastructure.Dtos;
using Aplicacion.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiLaboratorioAgua.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FisicoQuimicoController : ControllerBase
    {
        private readonly FisicoQuimicoService _service;

        public FisicoQuimicoController(FisicoQuimicoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (page < 1 || pageSize < 1 || pageSize > 200)
                return BadRequest("page debe ser >= 1 y pageSize entre 1 y 200.");
            var result = await _service.GetAllPagedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("por-fecha")]
        public async Task<IActionResult> GetPorFecha(
            [FromQuery] DateTime desde,
            [FromQuery] DateTime hasta,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (desde > hasta)
                return BadRequest("La fecha 'desde' no puede ser mayor que 'hasta'.");
            if (page < 1 || pageSize < 1 || pageSize > 200)
                return BadRequest("page debe ser >= 1 y pageSize entre 1 y 200.");
            var result = await _service.GetByFechaRangoPagedAsync(desde, hasta, page, pageSize);
            return Ok(result);
        }

        [HttpGet("por-cliente/{clienteId:int}")]
        public async Task<IActionResult> GetPorCliente(
            int clienteId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (page < 1 || pageSize < 1 || pageSize > 200)
                return BadRequest("page debe ser >= 1 y pageSize entre 1 y 200.");
            var result = await _service.GetByClienteIdPagedAsync(clienteId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null)
                return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FisicoQuimicoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] FisicoQuimicoEditDto dto)
        {
            if (id != dto.Id)
                return BadRequest("El id de la url no coincide con el cuerpo");

            await _service.UpdateAsync(dto);
            return Ok(new { message = "FisicoQuimico actualizado" });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
