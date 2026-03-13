using System;
using System.Collections.Generic;

namespace Infrastructure.Dtos
{
    public class ReporteLibroDto
    {
        public int LibroId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaLlegada { get; set; }
        public DateTime? FechaAnalisis { get; set; }
        public string? Procedencia { get; set; }
        public string? Observaciones { get; set; }
        public List<ReporteMuestraDto> Muestras { get; set; } = new List<ReporteMuestraDto>();
    }

    public class ReporteMuestraDto
    {
        public int MuestraId { get; set; }
        public string? Procedencia { get; set; }
        public string? NombreMuestreador { get; set; }
        public TipoDeMuestraDto TipoMuestra { get; set; }
        public int ClienteId { get; set; }
        public string? ClienteNombre { get; set; }
        public BacteriologicoDto? Bacteriologia { get; set; }
        public FisicoQuimicoDto? FisicoQuimico { get; set; }
    }
}
