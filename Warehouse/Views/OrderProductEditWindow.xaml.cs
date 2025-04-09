using System.Windows;
using Warehouse.Models;

namespace Warehouse.Views
{
    public partial class OrderProductEditWindow : Window
    {
        public OrderProduct? Result { get; private set; }
        private readonly Product _selectedProduct;

        public OrderProductEditWindow(Product product)
        {
            InitializeComponent();
            _selectedProduct = product;
            ProductNameText.Text = product.Name;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(QuantityTextBox.Text, out int quantity) && quantity > 0)
            {
                Result = new OrderProduct
                {
                    Product = _selectedProduct,
                    ProductId = _selectedProduct.Id,
                    Quantity = quantity,
                    PriceAtOrder = _selectedProduct.Price
                };
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Введите корректное количество.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
