namespace Entidades
{
    using System.ComponentModel.DataAnnotations;

    public class RodrigoPorcel
    {
        [Key]
        public int Id { get; set; }
	
	[Required]
        public int Ci { get; set; }
	        
        [Required]
        public string Nombre { get; set; }
        
        [Required]
        public int Edad { get; set; }
        
        [Required]
        public string Estado { get; set; }
	
	public string? DiscordId { get; set; }
        public string? DiscordUsername { get; set; }
    }
}
