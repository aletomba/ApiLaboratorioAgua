using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Dtos
{
    public class ClientesDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string? Nombre { get; set; }

        [EmailAddress]
        [StringLength(200)]
        public string? Email { get; set; }

        [Phone]
        [StringLength(50)]
        public string? Telefono { get; set; }
        //public List<MuestraDto>? Muestras { get; set; }
    }
}
