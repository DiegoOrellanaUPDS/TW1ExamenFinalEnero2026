namespace Entidades
{
    using System.ComponentModel.DataAnnotations;

    public class Snaider
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int Ci { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }
        
        [Required]
        [Range(0, 150)]
        public int Edad { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Estado { get; set; } = "Activo";
        
        // Campos para OAuth/Discord
        [MaxLength(500)]
        public string? DiscordId { get; set; }
        
        [MaxLength(2000)]
        public string? DiscordToken { get; set; }
        
        [MaxLength(2000)]
        public string? DiscordRefreshToken { get; set; }
        
        public DateTime? DiscordTokenExpiry { get; set; }
        
        [MaxLength(100)]
        public string? DiscordUsername { get; set; }
        
        [MaxLength(200)]
        public string? DiscordEmail { get; set; }
    }
}