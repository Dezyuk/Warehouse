using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Warehouse.Helper;
using Warehouse.Models;
using Warehouse.Services;
using Warehouse.Views;

namespace Warehouse.ViewModels
{
    public class InvoiceHistoryViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;

        public ObservableCollection<Order> Orders { get; }
            = new ObservableCollection<Order>();

        private Order? _selectedOrder;
        public Order? SelectedOrder
        {
            get => _selectedOrder;
            set { _selectedOrder = value; OnPropertyChanged(); }
        }

        public ICommand RefreshCommand { get; }
        public ICommand EditCommand { get; }

        public InvoiceHistoryViewModel(IOrderService orderService)
        {
            _orderService = orderService;
            RefreshCommand = new RelayCommand(LoadOrders);
            EditCommand = new RelayCommand(EditOrder, () => SelectedOrder != null);
            LoadOrders();
        }

        private void LoadOrders()
        {
            Orders.Clear();
            foreach (var o in _orderService.GetAllOrders())
                Orders.Add(o);
        }

        private void EditOrder()
        {
            if (SelectedOrder == null) return;

            // Копируем для редактирования
            var edited = new Order
            {
                Id = SelectedOrder.Id,
                CustomerName = SelectedOrder.CustomerName,
                OrderDate = SelectedOrder.OrderDate,
                OrderProducts = new ObservableCollection<OrderProduct>(SelectedOrder.OrderProducts)
            };

            var win = new EditOrderWindow(edited);
            if (win.ShowDialog() == true)
            {
                _orderService.UpdateOrder(edited);
                LoadOrders();
            }
        }
    }
}
