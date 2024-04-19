using TokoOnline.Enums;

namespace TokoOnline.Data
{
    public class AddProductMedia
    {
        public MediaType Type { get; set; }
        public string Filename { get; set; } = string.Empty;
        public string Media { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
    }
}
