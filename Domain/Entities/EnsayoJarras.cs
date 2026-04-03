namespace Dominio.Entities
{
    public class EnsayoJarras
    {
        public int Id { get; set; }

        // Las 5 dosis ensayadas (mg/L)
        public double? Dosis1 { get; set; }
        public double? Dosis2 { get; set; }
        public double? Dosis3 { get; set; }
        public double? Dosis4 { get; set; }
        public double? Dosis5 { get; set; }

        // La dosis seleccionada (la de mejor turbidez)
        public double? DosisSeleccionada { get; set; }

        // Cal pre y post ensayo (mg/L)
        public double? PreCal { get; set; }
        public double? PostCal { get; set; }

        // Unidad de medida: siempre mg/L
        public string UnidadMedida { get; set; } = "mg/L";

        // Relación con PlanillaDiaria
        public int PlanillaDiariaId { get; set; }
        public PlanillaDiaria? PlanillaDiaria { get; set; }
    }
}
