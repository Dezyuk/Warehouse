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
        private readonly IProductService _productService;

        public ObservableCollection<Order> Orders { get; }

        private Order? _selectedOrder;
        public Order? SelectedOrder
        {
            get => _selectedOrder;
            set { _selectedOrder = value; OnPropertyChanged(); }
        }

        public ICommand EditCommand { get; }
        
        public InvoiceHistoryViewModel(IOrderService orderService, IProductService productService)
        {
            
            _orderService = orderService;
            _productService = productService;
            Orders = orderService.Orders;

            EditCommand = new RelayCommand(EditOrder, () => SelectedOrder != null);
        }

        private void EditOrder()
        {
            if (SelectedOrder == null) return;


            var win = new OrderDetailsWindow(SelectedOrder);
            win.ShowDialog();

        }
    }
}
