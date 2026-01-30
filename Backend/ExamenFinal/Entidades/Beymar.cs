using System.ComponentModel.DataAnnotations;

namespace Entidades
{
    public class Beymar
    {
        [Key]
        public int Id { get; set; } //
        
        [Required]
        public string Nombre { get; set; } = "Beymar Vasquez"; //
        
        public int Edad { get; set; } = 25; //
        
        public string Estado { get; set; } = "Activo"; //
        
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}