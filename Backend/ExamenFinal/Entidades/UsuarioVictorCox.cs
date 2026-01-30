using System.ComponentModel.DataAnnotations;

namespace ExamenFinal.Entidades
{
    public class UsuarioVictorCox
    {
        [Key]
        public int Id { get; set; }
        
        public string Nombre { get; set; } = string.Empty;
        
        public string Correo { get; set; } = string.Empty;
        
        public string Rol { get; set; } = "Admin"; 
        
        public string TokenSesion { get; set; } = string.Empty; // Token para validar sesión
        
        public string Estado { get; set; } = "Activo";
    }
}
