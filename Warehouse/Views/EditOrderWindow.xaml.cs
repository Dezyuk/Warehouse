using System;
using System.Windows;
using Warehouse.Models;

namespace Warehouse.Views
{
    public partial class EditOrderWindow : Window
    {
        public Order Order { get; private set; }

        public EditOrderWindow(Order order)
        {
            InitializeComponent();
            Order = order;

            // Заполняем поля при открытии
            CustomerNameTextBox.Text = Order.CustomerName;
            OrderDatePicker.SelectedDate = Order.OrderDate;

            // Заполняем список товаров
            foreach (var product in Order.Products)
            {
                ProductsListBox.Items.Add($"{product.Name} - {product.Quantity} шт.");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Проверка корректности данных
            if (string.IsNullOrWhiteSpace(CustomerNameTextBox.Text))
            {
                MessageBox.Show("Имя клиента не может быть пустым.");
                return;
            }

            if (OrderDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату заказа.");
                return;
            }

            // Сохранение изменений
            Order.CustomerName = CustomerNameTextBox.Text;
            Order.OrderDate = OrderDatePicker.SelectedDate.Value;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
