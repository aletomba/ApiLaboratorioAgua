namespace Infrastructure.Dtos
{
    // DTO para crear/actualizar una planilla diaria
    public class PlanillaDiariaDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? Operador { get; set; }
        public string? Observaciones { get; set; }

        // Análisis fisicoquímico por punto de muestreo
        public List<AnalisisPuntoDto> AnalisisPorPunto { get; set; } = new();

        // Ensayo de jarras
        public EnsayoJarrasDto? EnsayoJarras { get; set; }
    }

    // Un análisis por punto de muestreo (reutiliza campos de FisicoQuimico)
    public class AnalisisPuntoDto
    {
        public PuntoMuestreoDto PuntoMuestreo { get; set; }
        public string? Ph { get; set; }
        public string? Turbidez { get; set; }
        public string? Alcalinidad { get; set; }
        public string? Dureza { get; set; }
        public string? Nitritos { get; set; }
        public string? Cloruros { get; set; }
        public string? Calcio { get; set; }
        public string? Magnesio { get; set; }
        public string? Dbo5 { get; set; }
        public string? Cloro { get; set; }
    }

    // DTO del ensayo de jarras
    public class EnsayoJarrasDto
    {
        public int Id { get; set; }
        public double? Dosis1 { get; set; }
        public double? Dosis2 { get; set; }
        public double? Dosis3 { get; set; }
        public double? Dosis4 { get; set; }
        public double? Dosis5 { get; set; }
        public double? DosisSeleccionada { get; set; }
        public double? PreCal { get; set; }
        public double? PostCal { get; set; }
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
