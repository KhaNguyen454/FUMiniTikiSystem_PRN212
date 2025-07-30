using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal OrderAmount { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }

        public string Status { get; set; }

        // Foreign Key
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        // Navigation properties
        public virtual Customer Customer { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>(); // Products in this order
    }
}