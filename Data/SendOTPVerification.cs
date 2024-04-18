using System.ComponentModel.DataAnnotations;

namespace TokoOnline.Data
{
    public class SendOTPVerification
    {
        [Required]
        public string Identifier { get; set; } = string.Empty;
    }
}
