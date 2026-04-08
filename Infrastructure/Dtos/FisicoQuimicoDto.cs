namespace Infrastructure.Dtos
{
    public class FisicoQuimicoDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaLLegada { get; set; }
        public DateTime FechaAnalisis { get; set; }
        public string? Procedencia { get; set; }
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
        public int MuestraId { get; set; }
        public string? MuestraProcedencia { get; set; }
    }
}
