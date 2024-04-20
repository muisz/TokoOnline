namespace TokoOnline.Data
{
    public class DetailSellerProduct
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public float Price { get; set; }
        public uint Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<DetailProductVariant> Variants { get; set; } = new List<DetailProductVariant>();
        public ICollection<DetailProductMedia> Media { get; set; } = new List<DetailProductMedia>();
    }
}