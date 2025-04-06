using System.Windows;
using Warehouse.Models;

namespace Warehouse.Views
{
    public partial class OrderProductEditWindow : Window
    {
        public OrderProduct OrderProduct { get; private set; }

        public OrderProductEditWindow(Product selectedProduct)
        {
            InitializeComponent();
            OrderProduct = new OrderProduct
            {
                Product = selectedProduct,
                ProductId = selectedProduct.Id
            };

            ProductNameTextBlock.Text = selectedProduct.Name;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(QuantityTextBox.Text, out int quantity) && quantity > 0)
            {
                OrderProduct.Quantity = quantity;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Введите корректное количество.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
