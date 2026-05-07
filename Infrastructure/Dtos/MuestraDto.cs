using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Dtos
{
    public class MuestraDto
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ClienteId debe ser mayor que 0.")]
        public int ClienteId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 2)]
        public string? SitioExtraccion { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string? NombreMuestreador { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public DateTime FechaExtraccion { get; set; }
        public TimeSpan HoraExtraccion { get; set; }

        [Range(0, 1, ErrorMessage = "TipoMuestra debe ser 0 (Bacteriologica) o 1 (FisicoQuimica).")]
        public TipoDeMuestraDto TipoMuestra { get; set; }
        public PuntoMuestreoDto? PuntoMuestreo { get; set; }
    }

    public enum TipoDeMuestraDto
    {
        Bacteriologica,
        FisicoQuimica
    }

    public enum PuntoMuestreoDto
    {
        AguaNatural,
        Decantada,
        Filtrada,
        Consumo
    }
}
