using System.ComponentModel.DataAnnotations;

namespace ExamenFinal.Entidades
{
    public class VictorCox
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public int Edad { get; set; }

        // Valores esperados: "soltero", "casado", "arrecho"
        public string Estado { get; set; } = string.Empty; 
    }
}