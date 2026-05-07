using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Dtos
{
    public class BacteriologicoDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaLLegada { get; set; }
        public DateTime FechaAnalisis { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string? Procedencia { get; set; }

        [StringLength(50)]
        public string? ColiformesNmp { get; set; }

        [StringLength(50)]
        public string? ColiformesFecalesNmp { get; set; }

        [StringLength(50)]
        public string? ColoniasAgar { get; set; }

        [StringLength(50)]
        public string? ColiFecalesUfc { get; set; }

        [StringLength(1000)]
        public string? Observaciones { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MuestraId debe ser mayor que 0.")]
        public int MuestraId { get; set; }

        [StringLength(500)]
        public string? MuestraProcedencia { get; set; }
    }
}
