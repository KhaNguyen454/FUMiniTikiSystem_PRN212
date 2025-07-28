using BusinessLogicLayer.Services;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GASMWPF
{
    /// <summary>
    /// Interaction logic for ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private List<Category> _categories;
        private Product? _selectedProduct;

        public ProductWindow(IProductService productService, ICategoryService categoryService)
        {
            InitializeComponent();
            _productService = productService;
            _categoryService = categoryService;

            LoadCategories();
            LoadProducts();
        }

        private void LoadCategories()
        {
            _categories = new List<Category>(_categoryService.GetAll());
            cmbCategory.ItemsSource = _categories;
            cmbCategory.DisplayMemberPath = "Name";
            cmbCategory.SelectedValuePath = "CategoryId";
        }

        private void LoadProducts()
        {
            ProductGrid.ItemsSource = null;
            ProductGrid.ItemsSource = _productService.GetAll();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(txtPrice.Text, out decimal price))
            {
                var product = new Product
                {
                    Name = txtName.Text.Trim(),
                    Price = price,
                    Description = txtDescription.Text.Trim(),
                    CategoryId = (int)cmbCategory.SelectedValue
                };

                _productService.Add(product);
                LoadProducts();
                ClearFields();
            }
            else
            {
                MessageBox.Show("Giá không hợp lệ!");
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct != null && decimal.TryParse(txtPrice.Text, out decimal price))
            {
                _selectedProduct.Name = txtName.Text.Trim();
                _selectedProduct.Price = price;
                _selectedProduct.Description = txtDescription.Text.Trim();
                _selectedProduct.CategoryId = (int)cmbCategory.SelectedValue;

                _productService.Update(_selectedProduct);
                LoadProducts();
                ClearFields();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct != null)
            {
                var result = MessageBox.Show("Bạn có chắc muốn xóa sản phẩm này?", "Xác nhận", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _productService.Delete(_selectedProduct.ProductId);
                    LoadProducts();
                    ClearFields();
                }
            }
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtPrice.Clear();
            txtDescription.Clear();
            cmbCategory.SelectedIndex = -1;
            _selectedProduct = null;
            ProductGrid.UnselectAll();
        }

        private void ProductGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = ProductGrid.SelectedItem as Product;

            if (_selectedProduct != null)
            {
                txtName.Text = _selectedProduct.Name;
                txtPrice.Text = _selectedProduct.Price.ToString("0.##");
                txtDescription.Text = _selectedProduct.Description;
                cmbCategory.SelectedValue = _selectedProduct.CategoryId;
            }
        }

        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedProduct != null && cmbCategory.SelectedValue is int selectedCategoryId)
            {
                _selectedProduct.CategoryId = selectedCategoryId;
            }
        }
    }
}
