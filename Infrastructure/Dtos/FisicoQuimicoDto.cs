using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Dtos
{
    public class FisicoQuimicoDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaLLegada { get; set; }
        public DateTime FechaAnalisis { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string? Procedencia { get; set; }

        [StringLength(50)]
        public string? Ph { get; set; }

        [StringLength(50)]
        public string? Turbidez { get; set; }

        [StringLength(50)]
        public string? Alcalinidad { get; set; }

        [StringLength(50)]
        public string? Dureza { get; set; }

        [StringLength(50)]
        public string? Nitritos { get; set; }

        [StringLength(50)]
        public string? Nitratos { get; set; }

        [StringLength(50)]
        public string? Amonio { get; set; }

        [StringLength(50)]
        public string? Cloruros { get; set; }

        [StringLength(50)]
        public string? Calcio { get; set; }

        [StringLength(50)]
        public string? Magnesio { get; set; }

        [StringLength(50)]
        public string? Dbo5 { get; set; }

        [StringLength(50)]
        public string? Cloro { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MuestraId debe ser mayor que 0.")]
        public int MuestraId { get; set; }

        [StringLength(500)]
        public string? MuestraProcedencia { get; set; }
    }
}
