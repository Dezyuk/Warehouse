using System.Windows;
using Warehouse.ViewModels;
using Warehouse.Models;

namespace Warehouse.Views
{
    public partial class OrdersWindow : Window
    {
        private readonly OrderViewModel _viewModel;

        public OrdersWindow(OrderViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            OrdersGrid.SelectionChanged += OrdersGrid_SelectionChanged;
        }

        private void OrdersGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is Order selectedOrder)
            {
                _viewModel.SelectedOrder = selectedOrder;
            }
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            var newOrder = new Order
            {
                Id = _viewModel.Orders.Count + 1, // Генерация ID (лучше получать из БД)
                CustomerName = "Новый заказ",
                OrderDate = System.DateTime.Now
            };

            _viewModel.AddOrder(newOrder);
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is Order selectedOrder)
            {
                var editWindow = new EditOrderWindow(selectedOrder);
                if (editWindow.ShowDialog() == true)
                {
                    _viewModel.EditOrder(selectedOrder);
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для редактирования.");
            }
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedOrder != null)
            {
                _viewModel.DeleteOrder();
            }
            else
            {
                MessageBox.Show("Выберите заказ для удаления.");
            }
        }
    }
}
