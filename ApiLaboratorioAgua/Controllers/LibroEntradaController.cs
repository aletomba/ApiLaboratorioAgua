using Infrastructure.Dtos;
using Aplicacion.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiLaboratorioAgua.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibroEntradaController : ControllerBase
    {
        private readonly LibroDeEntradaService _libroEntradaService;
        private readonly ReporteService _reporteService;

        public LibroEntradaController(LibroDeEntradaService libroEntradaService, ReporteService reporteService)
        {
            _libroEntradaService = libroEntradaService;
            _reporteService = reporteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLibroEntradas(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 50)
        {
            var result = await _libroEntradaService.GetAllLibroEntradasAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Busca libros de entrada por procedencia con paginación
        /// </summary>
        [HttpGet("por-procedencia")]
        public async Task<IActionResult> GetLibroEntradasPorProcedencia(
            [FromQuery] string procedencia,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (string.IsNullOrWhiteSpace(procedencia))
                return BadRequest("El parámetro 'procedencia' es requerido.");

            var result = await _libroEntradaService.GetLibroEntradasByProcedenciaPagedAsync(procedencia, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Busca libros de entrada por rango de fechas (fecha de registro)
        /// </summary>
        [HttpGet("por-fecha")]
        public async Task<IActionResult> GetLibroEntradasPorFecha(
            [FromQuery] DateTime desde,
            [FromQuery] DateTime hasta,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (desde > hasta)
                return BadRequest("La fecha 'desde' no puede ser mayor que 'hasta'.");

            var result = await _libroEntradaService.GetLibroEntradasByFechaRangoAsync(desde, hasta, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Registra un nuevo libro de entrada con sus muestras asociadas,
        /// creando automáticamente entradas en LibroBacteriologia o LibroFisicoQuimico.
        /// </summary>
        /// <param name="libroEntradaDto">Datos del libro de entrada y sus muestras.</param>
        /// <returns>Confirmación de registro exitoso.</returns>
        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarLibroEntrada([FromBody] LibroDeEntradaDto libroEntradaDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            await _libroEntradaService.RegistrarLibroEntradaAsync(libroEntradaDto);
            return Ok(new {message ="Libro de entrada registrado con éxito." });
        }

        /// <summary>
        /// Obtiene un libro de entrada por su ID.
        /// </summary>
        /// <param name="id">ID del libro de entrada.</param>
        /// <returns>Detalles del libro de entrada.</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetLibroEntrada(int id)
        {
            var libroEntrada = await _libroEntradaService.GetLibroEntradaByIdAsync(id);
            return Ok(libroEntrada);
        }

        /// <summary>
        /// Obtiene todos los libros de entrada asociados a una muestra.
        /// </summary>
        /// <param name="muestraId">ID de la muestra.</param>
        /// <returns>Lista de libros de entrada.</returns>
        [HttpGet("por-muestra/{muestraId}")]
        public async Task<IActionResult> GetLibroEntradasPorMuestra(int muestraId)
        {
            var libroEntradas = await _libroEntradaService.GetLibroEntradasByMuestraIdAsync(muestraId);
            return Ok(libroEntradas);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateLibroEntrada(int id, [FromBody] LibroDeEntradaDto libroEntradaDto)
        {
            if (id != libroEntradaDto.Id)
                return BadRequest("El ID de la URL no coincide con el del cuerpo de la solicitud.");

            await _libroEntradaService.UpdateLibroEntradaAsync(libroEntradaDto);
            return Ok(new { message = "Libro de entrada actualizado con éxito." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLibroEntrada(int id)
        {
            await _libroEntradaService.DeleteLibroEntradaAsync(id);
            return Ok(new { message = "Libro de entrada eliminado con éxito." });
        }

        [HttpGet("{id:int}/reporte")]
        public async Task<IActionResult> GetReportePdf(int id)
        {
            var pdfBytes = await _reporteService.GenerarPdfBytesAsync(id);
            return File(pdfBytes, "application/pdf", $"reporte_libro_{id}.pdf");
        }
    }
}
