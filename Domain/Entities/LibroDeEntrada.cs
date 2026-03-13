namespace Dominio.Entities
{
    public class LibroDeEntrada:BaseEntities
    {        
        public string? Observaciones { get; set; }
        public List<Muestra> Muestras { get; set; } = new List<Muestra>();
    }
}
