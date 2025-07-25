using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        [Required, MaxLength(255)]
        public string Name { get; set; }
        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string? Description { get; set; } // Nullable

        // Foreign Key
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        // [ForeignKey("Order")] // This seems incorrect based on typical e-commerce, Product is part of an OrderItem, not directly linked to Order like this.
        // However, if the ERD explicitly states OrderID here, we'll follow it for now but note it's unusual.
        // Based on the ERD, Product has an OrderID, implying a direct relationship, which is unusual for many-to-many via order items.
        // Let's assume for now that a product *can* be directly linked to an order for simplicity, but it's more common to have an OrderDetail/OrderItem table.
        // Given the ERD, OrderID is a foreign key here.
        public int? OrderId { get; set; } // Nullable because a product might not be in any order yet.

        // Navigation properties
        public virtual Category Category { get; set; }
        public virtual Order? Order { get; set; } // Nullable navigation property
    }
}