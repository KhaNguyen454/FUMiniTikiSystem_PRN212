using BusinessLogicLayer.Services;
using DataAccessLayer;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GASMWPF.Admin
{
    /// <summary>
    /// Interaction logic for CategoryWindow.xaml
    /// </summary>
    public partial class CategoryWindow : Window
    {
        private readonly FUMiniTikiSystemDBContext _context;
        private readonly ICategoryService _categoryService;
        private Category? _selectedCategory;

        public CategoryWindow()
        {
            InitializeComponent();

            // Create DB context
            var optionsBuilder = new DbContextOptionsBuilder<FUMiniTikiSystemDBContext>();
            optionsBuilder.UseSqlServer("Server=(local);Database=FUMiniTikiSystem;Trusted_Connection=True;TrustServerCertificate=True;");
            _context = new FUMiniTikiSystemDBContext(optionsBuilder.Options);

            // Initialize services
            ICategoryRepository categoryRepository = new CategoryRepository(_context);
            _categoryService = new CategoryService(categoryRepository);

            // Set selection changed event
            CategoryGrid.SelectionChanged += CategoryGrid_SelectionChanged;

            // Load data
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                CategoryGrid.ItemsSource = _categoryService.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CategoryGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryGrid.SelectedItem is Category category)
            {
                _selectedCategory = category;
                txtName.Text = category.Name;
                txtDescription.Text = category.Description;
                txtPicture.Text = category.Picture;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Category name cannot be empty", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newCategory = new Category
                {
                    Name = txtName.Text,
                    Description = txtDescription.Text,
                    Picture = txtPicture.Text
                };

                _categoryService.Add(newCategory);
                
                MessageBox.Show("Category added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearFields();
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategory == null)
            {
                MessageBox.Show("Please select a category to update", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Category name cannot be empty", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _selectedCategory.Name = txtName.Text;
                _selectedCategory.Description = txtDescription.Text;
                _selectedCategory.Picture = txtPicture.Text;

                _categoryService.Update(_selectedCategory);
                
                MessageBox.Show("Category updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearFields();
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategory == null)
            {
                MessageBox.Show("Please select a category to delete", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete category '{_selectedCategory.Name}'?", 
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _categoryService.Delete(_selectedCategory.CategoryId);
                    MessageBox.Show("Category deleted successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearFields();
                    LoadCategories();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting category: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        private void ClearFields()
        {
            txtName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtPicture.Text = string.Empty;
            _selectedCategory = null;
            CategoryGrid.SelectedItem = null;
        }
    }
}
