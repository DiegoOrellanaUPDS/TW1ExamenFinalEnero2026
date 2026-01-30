using System.ComponentModel.DataAnnotations;

namespace Entidades
{
    public class Brandon
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public string Estado { get; set; }
    }
}
