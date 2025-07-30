using BusinessLogicLayer.Services;
using DataAccessLayer;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

            var optionsBuilder = new DbContextOptionsBuilder<FUMiniTikiSystemDBContext>();
            optionsBuilder.UseSqlServer(AppConfiguration.GetConnectionString("FUMiniTikiDB"));
            _context = new FUMiniTikiSystemDBContext(optionsBuilder.Options);

            var repo = new CategoryRepository(_context);
            _categoryService = new CategoryService(repo);

            LoadCategories();
        }

        private void LoadCategories()
        {
            CategoryGrid.ItemsSource = _categoryService.GetAll();
        }

        private void CategoryGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedCategory = CategoryGrid.SelectedItem as Category;
            if (_selectedCategory != null)
            {
                txtName.Text = _selectedCategory.Name;
                txtDescription.Text = _selectedCategory.Description;
                txtPicture.Text = _selectedCategory.Picture;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var category = new Category
            {
                Name = txtName.Text,
                Description = txtDescription.Text,
                Picture = txtPicture.Text
            };
            _categoryService.Add(category);
            LoadCategories();
            ClearFields();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategory != null)
            {
                _selectedCategory.Name = txtName.Text;
                _selectedCategory.Description = txtDescription.Text;
                _selectedCategory.Picture = txtPicture.Text;

                _categoryService.Update(_selectedCategory);
                LoadCategories();
                ClearFields();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategory != null)
            {
                _categoryService.Delete(_selectedCategory.CategoryId);
                LoadCategories();
                ClearFields();
            }
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtDescription.Clear();
            txtPicture.Clear();
            _selectedCategory = null;
            CategoryGrid.UnselectAll();
        }
    }
}
