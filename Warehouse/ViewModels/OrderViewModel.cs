using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Warehouse.Models;
using Warehouse.Services;
using Warehouse.Helper;
using Warehouse.Views;           

namespace Warehouse.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        public ObservableCollection<Order> Orders { get; set; } = new();

        private Order? _selectedOrder;
       
        
        public Order? SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateOrderCommand { get; }
        public ICommand AddProductToOrderCommand { get; }
        public ICommand SaveOrderCommand { get; }
        public ICommand DeleteOrderCommand { get; }

        public OrderViewModel(IOrderService orderService, IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;

            CreateOrderCommand = new RelayCommand(CreateOrder);
            AddProductToOrderCommand = new RelayCommand(AddProductToOrder, () => SelectedOrder != null);
            SaveOrderCommand = new RelayCommand(SaveOrder, CanSaveOrder);
            DeleteOrderCommand = new RelayCommand(DeleteOrder, () => SelectedOrder != null);

            LoadOrders();
        }

        private void LoadOrders()
        {
            Orders.Clear();
            foreach (var order in _orderService.GetAllOrders())
            {
                Orders.Add(order);
            }
        }

        private void CreateOrder()
        {
            var newOrder = new Order
            {
                OrderDate = DateTime.UtcNow,
                CustomerName = "Нове замовлення"  
            };

            SelectedOrder = newOrder;
            Orders.Add(newOrder);
        }

       
        private void AddProductToOrder()
        {
            if (SelectedOrder == null)
                return;
            var excludedIds = SelectedOrder.OrderProducts.Select(op => op.ProductId).ToList();

            var productSelectionWindow = new ProductSelectionWindow(_productService, excludedIds);
            if (productSelectionWindow.ShowDialog() == true && productSelectionWindow.SelectedProduct != null)
            {
                Product selectedProduct = productSelectionWindow.SelectedProduct;
                var editWindow = new OrderProductEditWindow(selectedProduct);
                if (editWindow.ShowDialog() == true && editWindow.Result != null)
                {
                    OrderProduct newOp = editWindow.Result;
                    var existingOp = SelectedOrder.OrderProducts.FirstOrDefault(op => op.ProductId == newOp.ProductId);
                    if (existingOp != null)
                    {
                        existingOp.Quantity = newOp.Quantity;
                    }
                    else
                    {
                        SelectedOrder.OrderProducts.Add(newOp);
                    }
                    OnPropertyChanged(nameof(SelectedOrder.OrderProducts));
                }
            }
        }

       
        private void SaveOrder()
        {
            if (SelectedOrder == null)
                return;

            if (SelectedOrder.OrderProducts.Count == 0)
            {
                MessageBox.Show("Замовлення має містити хоча б один товар.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedOrder.Id == 0)
            {
                _orderService.AddOrder(SelectedOrder);
            }
            else
            {
                _orderService.UpdateOrder(SelectedOrder);
            }
            LoadOrders();
            SelectedOrder = null;
        }

        
        private bool CanSaveOrder()
        {
            return SelectedOrder != null && SelectedOrder.OrderProducts.Count > 0;
        }

      
        private void DeleteOrder()
        {
            if (SelectedOrder == null)
                return;

            _orderService.DeleteOrder(SelectedOrder.Id);
            LoadOrders();
            SelectedOrder = null;
        }
    }
}
