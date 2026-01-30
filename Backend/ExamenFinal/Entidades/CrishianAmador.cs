using System.ComponentModel.DataAnnotations;

namespace Entidades
{
    public class CristhianAmador
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public int Ci { get; set; }
        public int Edad { get; set; }
        public string Token { get; set; }
        public bool Estado { get; set; } = true;
    }
}