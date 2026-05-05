namespace Infrastructure.Dtos
{
    public class LibroDeEntradaDto:BaseEntitiesDto
    {
        public string? NroAnalisis { get; set; }
        public string? Observaciones { get; set; }
        public List<MuestraDto> Muestras { get; set; } = new List<MuestraDto>();
    }
}
