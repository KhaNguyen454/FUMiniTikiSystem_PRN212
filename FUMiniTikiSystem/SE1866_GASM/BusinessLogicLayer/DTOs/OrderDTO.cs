using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public decimal OrderAmount { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; } // Ví dụ để hiển thị
        public System.Collections.Generic.ICollection<ProductDTO> Products { get; set; } = new System.Collections.Generic.List<ProductDTO>();
    }
}
