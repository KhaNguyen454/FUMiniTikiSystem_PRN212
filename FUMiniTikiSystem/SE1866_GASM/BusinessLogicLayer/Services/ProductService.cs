
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using BusinessLogicLayer.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System; 

namespace BusinessLogicLayer.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        private ProductDTO MapToDTO(Product product)
        {
            return new ProductDTO
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                CategoryId = product.CategoryId,
                OrderId = product.OrderId,
                CategoryName = product.Category?.Name
            };
        }

        private Product MapToEntity(ProductDTO productDto)
        {
            return new Product
            {
                ProductId = productDto.ProductId,
                Name = productDto.Name,
                Price = productDto.Price,
                Description = productDto.Description,
                CategoryId = productDto.CategoryId,
                OrderId = productDto.OrderId
            };
        }

        public IEnumerable<ProductDTO> GetAllProductsDTO()
        {
            // Quan trọng: Sử dụng AsNoTracking() nếu bạn chỉ dùng để hiển thị và sau này sẽ tải lại để sửa
            // Hoặc đảm bảo repo của bạn đã dùng AsNoTracking() khi lấy tất cả
            return _repository.GetAll().Select(p => MapToDTO(p)).ToList();
        }

        public ProductDTO? GetProductDTOById(int id)
        {
            var product = _repository.GetById(id);
            return product == null ? null : MapToDTO(product);
        }

        public void AddProduct(ProductDTO productDto)
        {
            var productEntity = MapToEntity(productDto);
            _repository.Add(productEntity);
        }

        public void UpdateProduct(ProductDTO productDto)
        {
            // **THAY ĐỔI QUAN TRỌNG Ở ĐÂY:**
            // 1. Lấy thực thể gốc từ database
            var existingProduct = _repository.GetById(productDto.ProductId);

            if (existingProduct == null)
            {
                // Nếu không tìm thấy sản phẩm, ném ra ngoại lệ hoặc xử lý lỗi
                throw new Exception($"Sản phẩm với ID {productDto.ProductId} không tồn tại để cập nhật.");
            }

            // 2. Cập nhật các thuộc tính của thực thể gốc
            existingProduct.Name = productDto.Name;
            existingProduct.Price = productDto.Price;
            existingProduct.Description = productDto.Description;
            existingProduct.CategoryId = productDto.CategoryId;
            existingProduct.OrderId = productDto.OrderId; // Giữ nguyên OrderId từ DTO

            // 3. Yêu cầu repository lưu các thay đổi của thực thể này
            // DbContext sẽ theo dõi existingProduct, và khi SaveChanges được gọi,
            // nó sẽ tạo câu lệnh UPDATE.
            _repository.Update(existingProduct); // Gọi phương thức update của repository
        }

        public void DeleteProduct(int id)
        {
            _repository.Delete(id);
        }

        public IEnumerable<ProductDTO> SearchProducts(string searchTerm)
        {
            // Đảm bảo tìm kiếm không phân biệt hoa/thường
            var lowerSearchTerm = searchTerm.ToLower();

            var products = _repository.GetAll()
                                      .Where(p => p.Name.ToLower().Contains(lowerSearchTerm) ||
                                                  (p.Description != null && p.Description.ToLower().Contains(lowerSearchTerm)) ||
                                                  (p.Category != null && p.Category.Name.ToLower().Contains(lowerSearchTerm)))
                                      .Select(p => MapToDTO(p))
                                      .ToList();
            return products;
        }
    }
}