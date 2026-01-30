namespace Entidades
{
    using System.ComponentModel.DataAnnotations;

    public class HoracioZenteno
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public string Estado { get; set; }
        public string DiscordId { get; set; }
        public string Token { get; set; }
    }
}