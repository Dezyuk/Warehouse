using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Warehouse.Models;
using Warehouse.Services;

namespace Warehouse.Views
{
    public partial class ProductSelectionWindow : Window
    {
        private readonly IProductService _productService;
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();

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

        private void ProductsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ProductsList.SelectedItem is Product product)
            {
                SelectedProduct = product;
                DialogResult = true;
                Close();
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is Product product)
            {
                SelectedProduct = product;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Выберите товар.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
