using System.ComponentModel.DataAnnotations;

namespace Entidades
{
    public class Herberth
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Edad { get; set; }
        public string? Token { get; set; }
        public string Estado { get; set; }
    }
}