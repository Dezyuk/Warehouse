using System.Collections.ObjectModel;
using System.Windows.Controls;
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
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();

        private Product? _selectedProduct;
        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(); 
                (UpdateCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        
        public ProductViewModel(IProductService productService)
        {
            _productService = productService;
            AddCommand = new RelayCommand(AddProduct);
            UpdateCommand = new RelayCommand(UpdateProduct, () => SelectedProduct != null);
            DeleteCommand = new RelayCommand(DeleteProduct, () => SelectedProduct != null);
            LoadProducts();
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
            var window = new ProductEditWindow(newProduct);
            if (window.ShowDialog() == true)
            {
                _productService.AddProduct(newProduct);
                LoadProducts();
            }
        }

       
        private void UpdateProduct()
        {
            if (SelectedProduct == null)
                return;

            var editedProduct = new Product
            {
                Id = SelectedProduct.Id,
                Name = SelectedProduct.Name,
                Article = SelectedProduct.Article,
                Quantity = SelectedProduct.Quantity,
                Price = SelectedProduct.Price
            };

            var window = new ProductEditWindow(editedProduct);
            if (window.ShowDialog() == true)
            {
                _productService.UpdateProduct(editedProduct);
                LoadProducts();
            }
        }

        private void DeleteProduct()
        {
            if (SelectedProduct == null)
                return;

            var result = System.Windows.MessageBox.Show(
                $"Удалить продукт \"{SelectedProduct.Name}\"?",
                "Подтверждение удаления",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                _productService.DeleteProduct(SelectedProduct.Id);
                LoadProducts();
            }
        }
    }
}
