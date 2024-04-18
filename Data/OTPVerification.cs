using System.ComponentModel.DataAnnotations;

namespace TokoOnline.Data
{
    public class OTPVerification
    {
        [Required]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        public string Identifier { get; set; } = string.Empty;
    }
}