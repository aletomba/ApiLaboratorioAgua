

namespace Dominio.Entities
{
    public class Muestra
    {
        public int Id { get; set; }
        public string? Procedencia { get; set; }
        public string? NombreMuestreador { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public DateTime FechaExtraccion { get; set; }
        public TimeSpan HoraExtraccion { get; set; }
        public TipoMuestra TipoMuestra { get; set; }
        public PuntoMuestreo? PuntoMuestreo { get; set; }
        public int LibroEntradaId { get; set; }
        public LibroDeEntrada? LibroEntrada { get; set; }
        public Bacteriologico? Bacteriologia { get; set; }
        public FisicoQuimico? FisicoQuimico { get; set; }
        public int ClienteId { get; set; } // Relación con Cliente
        public Cliente? Cliente { get; set; }

    }

    public enum TipoMuestra
    {
        Bacteriologica,
        FisicoQuimica
    }

    public enum PuntoMuestreo
    {
        AguaNatural,
        Decantada,
        Filtrada,
        Consumo
    }
}
