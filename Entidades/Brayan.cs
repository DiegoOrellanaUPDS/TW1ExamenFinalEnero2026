using System.ComponentModel.DataAnnotations;

namespace Entidades {
    public class Brayan {
        [Key] public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}