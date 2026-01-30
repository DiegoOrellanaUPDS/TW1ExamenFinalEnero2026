namespace Entidades
{
    using System.ComponentModel.DataAnnotations;

    public class SergioVillarrubia
    {
        [Key]
        public int Id {get;set;}
        public string Nombre {get;set;}
        public int Ci {get;set;}
        public int Edad {get;set;}
        public string TokenSesion {get;set;}
        public string Estado {get;set;}
    }
}