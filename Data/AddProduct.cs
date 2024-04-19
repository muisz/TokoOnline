using System.ComponentModel.DataAnnotations;

namespace TokoOnline.Data
{
    public class AddProduct
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        [Required]
        public float Price { get; set; }
        
        [Required]
        public uint Stock { get; set; }
        public ICollection<AddProductVariant> Variants { get; set; } = new List<AddProductVariant>();
        public ICollection<AddProductMedia> Media { get; set; } = new List<AddProductMedia>();
    }
}