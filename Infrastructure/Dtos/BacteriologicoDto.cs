namespace Infrastructure.Dtos
{
    public class BacteriologicoDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaLLegada { get; set; }
        public DateTime FechaAnalisis { get; set; }
        public string? Procedencia { get; set; }
        public string? ColiformesNmp { get; set; }
        public string? ColiformesFecalesNmp { get; set; }
        public string? ColoniasAgar { get; set; }
        public string? ColiFecalesUfc { get; set; }
        public string? Observaciones { get; set; }
        public int MuestraId { get; set; }
        public string? MuestraProcedencia { get; set; }
    }
}
