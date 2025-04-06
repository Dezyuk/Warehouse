using System.Windows;
using Warehouse.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Warehouse.Services;

namespace Warehouse.Views
{
    public partial class ProductSelectionWindow : Window
    {
        public ObservableCollection<Product> Products { get; set; } = new();
        private readonly IProductService _productService;

        public Product? SelectedProduct { get; private set; }

        public ProductSelectionWindow(IProductService productService)
        {
            InitializeComponent();
            _productService = productService;
            LoadProducts();
            DataContext = this;
        }

        private void LoadProducts()
        {
            Products.Clear();
            foreach (var product in _productService.GetAllProducts())
            {
                Products.Add(product);
            }
        }

        private void ProductList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ProductList.SelectedItem is Product product)
            {
                SelectedProduct = product;
                DialogResult = true;
                Close();
            }
        }
        

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
