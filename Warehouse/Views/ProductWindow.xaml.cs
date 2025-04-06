using System.Windows;
using Warehouse.ViewModels;
using Warehouse.Models;

namespace Warehouse.Views
{
    public partial class ProductWindow : Window
    {
        private readonly ProductViewModel _viewModel;

        public ProductWindow(ProductViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var newProduct = new Product
            {
                
                Name = "Новый товар",
                Article = "temp",
                CategoryId = 0,
                Quantity = 0,
                MinimumStock = 0
            };

            _viewModel.AddProduct(newProduct);
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem is Product selectedProduct)
            {
                var editWindow = new EditProductWindow(selectedProduct);
                if (editWindow.ShowDialog() == true)
                {
                    _viewModel.EditProduct(selectedProduct);
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования.");
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem is Product selectedProduct)
            {
                _viewModel.DeleteProduct(selectedProduct);
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления.");
            }
        }
    }
}
