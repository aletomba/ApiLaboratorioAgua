namespace Dominio.Entities
{
    //clase que contiene los atributos que comparten algunas entidades
    public class BaseEntities
    {
        public int Id { get; set; }
        public DateTime FechaLLegada { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaAnalisis { get; set; }
        public string? Procedencia { get; set; }
        public string? SitioExtraccion { get; set; }
    }
}
