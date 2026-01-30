// Archivo: Entidades/Snaider.cs
namespace Entidades
{
    using System.ComponentModel.DataAnnotations;

    public class Snaider
    {
        [Key]
        public int Id { get; set; }
        
        // Estos campos ahora son opcionales ya que el usuario puede venir solo de Discord
        public int? Ci { get; set; }
        
        [MaxLength(100)]
        public string? Nombre { get; set; }
        
        public int? Edad { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Estado { get; set; } = "Activo";
        
        // Campos para OAuth/Discord (ahora obligatorios)
        [Required]
        [MaxLength(500)]
        public string DiscordId { get; set; }
        
        [MaxLength(2000)]
        public string? DiscordToken { get; set; }
        
        [MaxLength(2000)]
        public string? DiscordRefreshToken { get; set; }
        
        public DateTime? DiscordTokenExpiry { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string DiscordUsername { get; set; }
        
        [MaxLength(200)]
        public string? DiscordEmail { get; set; }
        
        // Nuevos campos para información de Discord
        [MaxLength(500)]
        public string? DiscordAvatar { get; set; }
        
        [MaxLength(50)]
        public string? DiscordLocale { get; set; }
        
        public bool DiscordVerified { get; set; }
        
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        
        public DateTime? UltimoLogin { get; set; }
    }
}