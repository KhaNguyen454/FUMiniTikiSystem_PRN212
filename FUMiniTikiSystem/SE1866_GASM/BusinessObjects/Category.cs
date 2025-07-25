using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects
{
    public class Category
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        public string? Picture { get; set; } // Nullable
        public string? Description { get; set; } // Nullable

        // Navigation property for Products
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}