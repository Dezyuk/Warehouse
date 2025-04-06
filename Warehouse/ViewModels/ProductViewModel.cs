using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Warehouse.Helper;
using Warehouse.Models;
using Warehouse.Services;
using Warehouse.Views;

namespace Warehouse.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;

        public ObservableCollection<Product> Products { get; set; } = new();
        private Product? _selectedProduct;
        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        public ProductViewModel(IProductService productService)
        {
            _productService = productService;

            LoadProducts();

            AddCommand = new RelayCommand(AddProduct);
            UpdateCommand = new RelayCommand(UpdateProduct, () => SelectedProduct != null);
            DeleteCommand = new RelayCommand(DeleteProduct, () => SelectedProduct != null);
        }

        private void LoadProducts()
        {
            Products.Clear();
            foreach (var product in _productService.GetAllProducts())
            {
                Products.Add(product);
            }
        }

        private void AddProduct()
        {
            var newProduct = new Product();
            var window = new EditProductWindow(newProduct);

            if (window.ShowDialog() == true)
            {
                _productService.AddProduct(newProduct);
                LoadProducts();
            }
        }

        private void UpdateProduct()
        {
            if (SelectedProduct == null) return;

            // Копируем данные, чтобы не менять напрямую
            var editedProduct = new Product
            {
                Id = SelectedProduct.Id,
                Name = SelectedProduct.Name,
                Article = SelectedProduct.Article,
                Quantity = SelectedProduct.Quantity,
                Price = SelectedProduct.Price
            };

            var window = new EditProductWindow(editedProduct);

            if (window.ShowDialog() == true)
            {
                _productService.UpdateProduct(editedProduct);
                LoadProducts();
            }
        }

        private void DeleteProduct()
        {
            if (SelectedProduct == null) return;

            _productService.DeleteProduct(SelectedProduct.Id);
            LoadProducts();
        }

        
    }
}
