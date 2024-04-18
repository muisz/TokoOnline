using System.ComponentModel.DataAnnotations;

namespace TokoOnline.Data
{
    public class RefreshToken
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}
