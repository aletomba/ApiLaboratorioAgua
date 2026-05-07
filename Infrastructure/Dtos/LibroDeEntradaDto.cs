using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Dtos
{
    public class LibroDeEntradaDto:BaseEntitiesDto
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string? NroAnalisis { get; set; }

        [StringLength(1000)]
        public string? Observaciones { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Debe incluir al menos una muestra.")]
        public List<MuestraDto> Muestras { get; set; } = new List<MuestraDto>();
    }
}
