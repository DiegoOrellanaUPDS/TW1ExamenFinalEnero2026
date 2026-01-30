using System.ComponentModel.DataAnnotations;
using Data;

namespace Entidades
{
    public class AlejandroRivera
    {
        [Key]
        public int Ci { get; set; }   // CI como clave primaria

        public string Nombre { get; set; } = string.Empty;

        public int Edad { get; set; }

        public string Estado { get; set; } = string.Empty;
    }
}
