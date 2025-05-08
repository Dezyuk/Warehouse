using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Warehouse.Models;
using Warehouse.Services;

namespace Warehouse.Views
{
    
    public partial class ProductSelectionWindow : Window
    {
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();

        private readonly IProductService _productService;
        private readonly System.Collections.Generic.List<int> _excludedProductIds;
        public Product? SelectedProduct { get; private set; }
        
        
        public ProductSelectionWindow(IProductService productService, System.Collections.Generic.List<int> excludedProductIds = null)
        {
            InitializeComponent();
            _productService = productService;
            _excludedProductIds = excludedProductIds ?? new System.Collections.Generic.List<int>();
            LoadProducts();
            DataContext = this;
        }

       
        private void LoadProducts()
        {
            Products.Clear();
            var allProducts = _productService.GetAllProducts();
            var filtered = allProducts.Where(p => !_excludedProductIds.Contains(p.Id));
            foreach (var product in filtered)
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
                MessageBox.Show("Виберіть товар.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
