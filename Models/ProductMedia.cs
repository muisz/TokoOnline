using TokoOnline.Enums;

namespace TokoOnline.Models
{
    public class ProductMedia
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public MediaType Type { get; set; }
        public string Media { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
