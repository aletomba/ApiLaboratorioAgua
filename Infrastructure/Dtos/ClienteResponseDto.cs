namespace Infrastructure.Dtos
{
    public class ClienteResponseDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }        
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public int NumeroMuestras { get; set; }
    }
}
