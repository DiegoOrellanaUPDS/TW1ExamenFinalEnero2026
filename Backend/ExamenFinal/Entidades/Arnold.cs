using System.ComponentModel.DataAnnotations;

namespace Entidades
{
    public class Arnold
    {
        [Key]
        public int Id{get;set;}
        public string Nombre{get;set;}=string.Empty;
        public int Edad{get;set;}
        public bool Estado{get;set;}=true;
        public string Token{get;set;}=string.Empty;
    }
}