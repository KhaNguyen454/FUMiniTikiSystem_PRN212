
using BusinessLogicLayer.DTOs; 
using System.Collections.Generic;
using System.Threading.Tasks; 

namespace BusinessLogicLayer.Services
{
    public interface IProductService
    {
        // Trả về IEnumerable<ProductDTO>
        IEnumerable<ProductDTO> GetAllProductsDTO();

        // Trả về ProductDTO?
        ProductDTO? GetProductDTOById(int id);

        // Nhận ProductDTO để thêm mới
        void AddProduct(ProductDTO productDto);

        // Nhận ProductDTO để cập nhật
        void UpdateProduct(ProductDTO productDto);

        // Vẫn xóa theo ID
        void DeleteProduct(int id);

        // Thêm phương thức tìm kiếm (đề xuất)
        IEnumerable<ProductDTO> SearchProducts(string searchTerm);
    }
}