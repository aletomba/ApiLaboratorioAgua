using System;
using System.Collections.Generic;

namespace Infrastructure.Dtos
{
    public class ReporteMultipleRequestDto
    {
        public List<int> LibroIds { get; set; } = new();
    }

    public class ReporteResumenLibroDto
    {
        public int LibroId { get; set; }
        public DateTime? FechaAnalisis { get; set; }
        public string? Procedencia { get; set; }
        public List<string> TiposAnalisis { get; set; } = new();
    }


    public class ReporteLibroDto
    {
        public int LibroId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaLlegada { get; set; }
        public DateTime? FechaAnalisis { get; set; }
        public string? Procedencia { get; set; }
        public string? NroAnalisis { get; set; }
        public string? Observaciones { get; set; }
        public List<ReporteMuestraDto> Muestras { get; set; } = new List<ReporteMuestraDto>();
    }

    public class ReporteMuestraDto
    {
        public int MuestraId { get; set; }
        public string? Procedencia { get; set; }
        public string? SitioExtraccion { get; set; }
        public string? NombreMuestreador { get; set; }
        public TimeSpan HoraExtraccion { get; set; }
        public TipoDeMuestraDto TipoMuestra { get; set; }
        public int ClienteId { get; set; }
        public string? ClienteNombre { get; set; }
        public BacteriologicoDto? Bacteriologia { get; set; }
        public FisicoQuimicoDto? FisicoQuimico { get; set; }
    }
}