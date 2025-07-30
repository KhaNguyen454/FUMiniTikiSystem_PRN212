using BusinessLogicLayer.Services;
using BusinessLogicLayer.DTOs;
using DataAccessLayer.Entities; // Vẫn cần cho Category entity (hoặc tạo CategoryDTO nếu bạn muốn nhất quán)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel; // Để sử dụng ObservableCollection

namespace GASMWPF.Admin
{
    /// <summary>
    /// Interaction logic for ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private List<Category> _categories; // Vẫn dùng Category entity cho ComboBox
        private ProductDTO? _selectedProductDTO;
        private ObservableCollection<ProductDTO> _allProducts; // Lưu trữ tất cả sản phẩm
        private ObservableCollection<ProductDTO> _filteredProducts; // Lưu trữ sản phẩm đã lọc

        public ProductWindow(IProductService productService, ICategoryService categoryService)
        {
            InitializeComponent();
            _productService = productService;
            _categoryService = categoryService;

            _allProducts = new ObservableCollection<ProductDTO>();
            _filteredProducts = new ObservableCollection<ProductDTO>();
            ProductGrid.ItemsSource = _filteredProducts; // Gán ItemsSource cho DataGrid là danh sách đã lọc

            LoadCategories();
            LoadProducts();
        }

        private void LoadCategories()
        {
            _categories = new List<Category>(_categoryService.GetAll());
            cmbCategory.ItemsSource = _categories;
        }

        private void LoadProducts()
        {
            _allProducts.Clear();
            foreach (var productDto in _productService.GetAllProductsDTO())
            {
                _allProducts.Add(productDto);
            }
            FilterProducts(); // Tải xong thì lọc ngay (ban đầu sẽ hiển thị tất cả)
        }

        // PHƯƠNG THỨC MỚI: Lọc sản phẩm
        private void FilterProducts()
        {
            _filteredProducts.Clear();
            string searchTerm = txtSearch.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // Nếu ô tìm kiếm trống, hiển thị tất cả sản phẩm
                foreach (var product in _allProducts)
                {
                    _filteredProducts.Add(product);
                }
            }
            else
            {
                // Lọc theo Tên, Mô tả hoặc Tên Danh mục
                var results = _allProducts.Where(p =>
                    p.Name.ToLower().Contains(searchTerm) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm)) ||
                    (p.CategoryName != null && p.CategoryName.ToLower().Contains(searchTerm))
                );

                foreach (var product in results)
                {
                    _filteredProducts.Add(product);
                }
            }
        }

        // SỰ KIỆN MỚI: Xử lý khi văn bản trong ô tìm kiếm thay đổi
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterProducts(); // Gọi hàm lọc mỗi khi văn bản thay đổi
        }


        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPrice.Text) || cmbCategory.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin Tên, Giá và chọn Danh mục.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Giá không hợp lệ. Vui lòng nhập một số dương.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPrice.Focus();
                return;
            }

            if (cmbCategory.SelectedValue is not int selectedCategoryId)
            {
                MessageBox.Show("Vui lòng chọn một danh mục hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var productDto = new ProductDTO
                {
                    Name = txtName.Text.Trim(),
                    Price = price,
                    Description = txtDescription.Text.Trim(),
                    CategoryId = selectedCategoryId,
                    OrderId = null // Hoặc một giá trị mặc định nếu OrderId là bắt buộc
                };

                _productService.AddProduct(productDto);
                MessageBox.Show("Sản phẩm đã được thêm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadProducts(); // Tải lại và lọc sản phẩm
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm sản phẩm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProductDTO == null)
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPrice.Text) || cmbCategory.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin Tên, Giá và chọn Danh mục.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Giá không hợp lệ. Vui lòng nhập một số dương.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPrice.Focus();
                return;
            }

            if (cmbCategory.SelectedValue is not int selectedCategoryId)
            {
                MessageBox.Show("Vui lòng chọn một danh mục hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Cập nhật ProductDTO đã chọn với dữ liệu mới
                _selectedProductDTO.Name = txtName.Text.Trim();
                _selectedProductDTO.Price = price;
                _selectedProductDTO.Description = txtDescription.Text.Trim();
                _selectedProductDTO.CategoryId = selectedCategoryId;

                _productService.UpdateProduct(_selectedProductDTO);
                MessageBox.Show("Sản phẩm đã được cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadProducts(); // Tải lại và lọc sản phẩm
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật sản phẩm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProductDTO == null)
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm để xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc muốn xóa sản phẩm '{_selectedProductDTO.Name}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _productService.DeleteProduct(_selectedProductDTO.ProductId);
                    MessageBox.Show("Sản phẩm đã được xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadProducts(); // Tải lại và lọc sản phẩm
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa sản phẩm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtPrice.Clear();
            txtDescription.Clear();
            cmbCategory.SelectedIndex = -1;
            _selectedProductDTO = null; // Reset DTO đã chọn
            ProductGrid.UnselectAll();
            txtSearch.Clear(); // Xóa cả ô tìm kiếm
        }

        private void ProductGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProductDTO = ProductGrid.SelectedItem as ProductDTO;

            if (_selectedProductDTO != null)
            {
                txtName.Text = _selectedProductDTO.Name;
                txtPrice.Text = _selectedProductDTO.Price.ToString("F2");
                txtDescription.Text = _selectedProductDTO.Description;

                

                if (_selectedProductDTO.CategoryId != 0) 
                {
                    cmbCategory.SelectedValue = _selectedProductDTO.CategoryId;
                }
                else
                {
                    cmbCategory.SelectedIndex = -1; 
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Không cần logic đặc biệt ở đây, việc chọn giá trị sẽ được dùng khi Cập nhật hoặc Thêm
        }
    }
}