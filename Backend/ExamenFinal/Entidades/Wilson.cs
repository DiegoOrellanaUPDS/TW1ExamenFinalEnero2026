using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExamenFinal.Entidades
{
    public class Wilson
    {
        [Key]
        public int id { get; set; }
        public string nombre { get; set; }
        public int edad { get; set; }
        public string estado { get; set; }
        public string token { get; set; }
    }
}