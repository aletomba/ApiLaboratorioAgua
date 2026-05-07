using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Dtos
{
    // DTO para crear/actualizar una planilla diaria
    public class PlanillaDiariaDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string? Operador { get; set; }

        [StringLength(1000)]
        public string? Observaciones { get; set; }

        // Análisis fisicoquímico por punto de muestreo
        [Required]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un punto de análisis.")]
        public List<AnalisisPuntoDto> AnalisisPorPunto { get; set; } = new();

        // Ensayo de jarras
        public EnsayoJarrasDto? EnsayoJarras { get; set; }
    }

    // Un análisis por punto de muestreo (reutiliza campos de FisicoQuimico)
    public class AnalisisPuntoDto
    {
        [Range(0, 3, ErrorMessage = "PuntoMuestreo debe ser 0 (AguaNatural), 1 (Decantada), 2 (Filtrada) o 3 (Consumo).")]
        public PuntoMuestreoDto PuntoMuestreo { get; set; }

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
        public string? Cloruros { get; set; }

        [StringLength(50)]
        public string? Calcio { get; set; }

        [StringLength(50)]
        public string? Magnesio { get; set; }

        [StringLength(50)]
        public string? Dbo5 { get; set; }

        [StringLength(50)]
        public string? Cloro { get; set; }
    }

    // DTO del ensayo de jarras
    public class EnsayoJarrasDto
    {
        public int Id { get; set; }

        [Range(0, 500)]
        public double? Dosis1 { get; set; }

        [Range(0, 500)]
        public double? Dosis2 { get; set; }

        [Range(0, 500)]
        public double? Dosis3 { get; set; }

        [Range(0, 500)]
        public double? Dosis4 { get; set; }

        [Range(0, 500)]
        public double? Dosis5 { get; set; }

        [Range(0, 500)]
        public double? DosisSeleccionada { get; set; }

        [Range(0, 200)]
        public double? PreCal { get; set; }

        [Range(0, 200)]
        public double? PostCal { get; set; }

        [StringLength(20)]
        public string UnidadMedida { get; set; } = "mg/L";
    }

    // DTO de respuesta con datos completos
    public class PlanillaDiariaResponseDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? Operador { get; set; }
        public string? Observaciones { get; set; }
        public int LibroEntradaId { get; set; }
        public List<AnalisisPuntoDto> AnalisisPorPunto { get; set; } = new();
        public EnsayoJarrasDto? EnsayoJarras { get; set; }
    }
}
