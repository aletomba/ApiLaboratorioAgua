namespace Dominio.Entities
{
    public class FisicoQuimico:BaseEntities
    {       
        public int MuestraId { get; set; }
        public Muestra? Muestra { get; set; }
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
}
