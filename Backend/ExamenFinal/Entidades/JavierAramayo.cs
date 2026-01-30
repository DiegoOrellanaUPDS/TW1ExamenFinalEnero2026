using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Entidades
{
    public class JavierAramayo
    {
        [Key]
        public int javierId {get;set;}
        public string ci {get;set;}
        public string nombre {get;set;}
        public string usuarioDc{get;set;}
        public string tokenDc {get;set;}
        public int edad {get;set;}
        public string estado {get;set;}
    }
}