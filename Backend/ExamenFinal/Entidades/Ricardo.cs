namespace Entidades
{
    using System.ComponentModel.DataAnnotations;
    public class Ricardo
    {
        [Key]
        public int Id {get;set;}
        public string Nombre {get;set;}
        public int Edad {get;set;}
        public string Estado {get;set;}
    }
}