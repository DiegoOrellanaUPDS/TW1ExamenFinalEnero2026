using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TuProyecto.Entities
{
    [Table("MiguelAngelColqueCalizaya")]
    public class MiguelAngelColqueCalizaya
    {
        [Key]
        
        public int IdMiguelColque { get; set; }


        public string? NombreMiguelColque { get; set; }

        public int EdadMiguelColque { get; set; }


        public string? EstadoMiguelColque { get; set; }
        public string? tokenMiguelColque {get; set;}


    }
}