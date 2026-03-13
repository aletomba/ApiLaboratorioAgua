namespace Dominio.Entities
{
    public class PlanillaDiaria
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string? Operador { get; set; }
        public string? Observaciones { get; set; }

        // Relación 1 a 1 con LibroDeEntrada
        public int LibroEntradaId { get; set; }
        public LibroDeEntrada? LibroEntrada { get; set; }

        // Ensayo de jarras del día
        public EnsayoJarras? EnsayoJarras { get; set; }
    }
}
