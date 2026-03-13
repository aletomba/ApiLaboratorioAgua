namespace Dominio.Entities
{
    public class Bacteriologico : BaseEntities
    {
        
        public string? ColiformesNmp { get; set; }
        public string? ColiformesFecalesNmp { get; set; }
        public string? ColoniasAgar { get; set; }
        public string? ColiFecalesUfc { get; set; }
        public string? Observaciones { get; set; }       
        public int MuestraId { get; set; }
        public Muestra? Muestra { get; set; }                

        //TODO: Agregar los atributos restantes
    }
}


