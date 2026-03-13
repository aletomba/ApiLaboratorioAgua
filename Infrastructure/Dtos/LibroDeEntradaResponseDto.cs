using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Dtos
{
    public class LibroDeEntradaResponseDto
    {
        public int Id { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaLlegada { get; set; }
        public DateTime? FechaAnalisis { get; set; }
        public string? Procedencia { get; set; }     
        public string? SitioExtraccion { get; set; }    
        public string? Observaciones { get; set; }
        public List<MuestraResponseDto> Muestras { get; set; } = new List<MuestraResponseDto>();
    }
}
