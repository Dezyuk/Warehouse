using System.Collections.ObjectModel;
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

        private Order? _selectedOrder;
        public Order? SelectedOrder
        {
            get => _selectedOrder;
            set { _selectedOrder = value; OnPropertyChanged(); }
        }

        public ICommand EditCommand { get; }

        public InvoiceHistoryViewModel(IOrderService orderService)
        {
            _orderService = orderService;
            Orders = orderService.Orders;

            EditCommand = new RelayCommand(EditOrder, () => SelectedOrder != null);
        }

        private void EditOrder()
        {
            if (SelectedOrder == null) return;

            // Создаём копию для редактирования
            var edited = new Order
            {
                Id = SelectedOrder.Id,
                CustomerName = SelectedOrder.CustomerName,
                OrderDate = SelectedOrder.OrderDate,
                OrderProducts = SelectedOrder.OrderProducts
            };

            var win = new EditOrderWindow(edited);
            if (win.ShowDialog() == true)
            {
                _orderService.UpdateOrder(edited);
                // Orders обновится автоматически
            }
        }
    }
}
