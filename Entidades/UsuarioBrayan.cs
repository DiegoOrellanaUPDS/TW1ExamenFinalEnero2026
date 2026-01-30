using System.ComponentModel.DataAnnotations;

namespace Entidades
{
    public class UsuarioBrayan
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string TokenSesion { get; set; } = string.Empty; // Aquí guardaremos el "token"
    }
}