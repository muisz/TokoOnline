using TokoOnline.Enums;

namespace TokoOnline.Data
{
    public class DetailProductMedia
    {
        public int Id { get; set; }
        public MediaType Type { get; set; }
        public string Media { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
