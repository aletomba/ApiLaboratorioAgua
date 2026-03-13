using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Dtos
{
    public class MuestraResponseDto
    {
        public int Id { get; set; }
        public string Procedencia { get; set; }
        public string NombreMuestreador { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public DateTime FechaExtraccion { get; set; }
        public TimeSpan HoraExtraccion { get; set; }
        public TipoDeMuestraDto TipoMuestra { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public int LibroEntradaId { get; set; }
    }
}
