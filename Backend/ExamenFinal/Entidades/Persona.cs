namespace ExamenFinal.Entidades
{
    using System.ComponentModel.DataAnnotations;

    public class Persona
    {
        [Key]
        public int Id {get;set;}
        public int Ci {get;set;}
        public string Nombre {get;set;}
        public string Apellido {get;set;}
        public DateOnly FechaNacimiento {get;set;}
        public string Estado {get;set;}
    }
}