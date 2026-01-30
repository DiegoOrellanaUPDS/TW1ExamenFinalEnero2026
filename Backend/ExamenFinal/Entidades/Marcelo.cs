using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entidades
{
    [Index(nameof(Token), IsUnique = true)]
    public class Marcelo
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int Edad { get; set; }
        [NotMapped]
        public string? Password { get; set; }
        public string Rol { get; set; } = "user";
        public string Token { get; set; } = "-1";
        public string Estado { get; set; } = "Activo";
    }
}
