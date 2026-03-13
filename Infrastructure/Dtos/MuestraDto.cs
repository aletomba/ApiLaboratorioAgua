namespace Infrastructure.Dtos
{
    public class MuestraDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }       
        public string SitioExtraccion { get; set; } 
        public string NombreMuestreador { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public DateTime FechaExtraccion { get; set; }
        public TimeSpan HoraExtraccion { get; set; }
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
