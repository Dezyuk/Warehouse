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
    /// <summary>
    /// ViewModel для управления заказами.
    /// Позволяет создавать новый заказ, добавлять в него товары через OrderProduct,
    /// редактировать и сохранять заказ, при этом заказ должен содержать хотя бы один товар.
    /// </summary>
    public class OrderViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        /// <summary>
        /// Коллекция заказов для отображения.
        /// </summary>
        public ObservableCollection<Order> Orders { get; set; } = new();

        private Order? _selectedOrder;
        /// <summary>
        /// Выбранный заказ.
        /// </summary>
        public Order? SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();
            }
        }

        // Команды для управления заказами:
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

        /// <summary>
        /// Загружает все заказы из сервиса в коллекцию Orders.
        /// </summary>
        private void LoadOrders()
        {
            Orders.Clear();
            foreach (var order in _orderService.GetAllOrders())
            {
                Orders.Add(order);
            }
        }

        /// <summary>
        /// Создает новый заказ с пустым списком OrderProducts.
        /// </summary>
        private void CreateOrder()
        {
            var newOrder = new Order
            {
                OrderDate = DateTime.UtcNow,
                CustomerName = "Новый заказ"  // Можно изменить, добавить ввод через окно
            };

            // При создании нового заказа делаем его выбранным.
            SelectedOrder = newOrder;
            Orders.Add(newOrder);
        }

        /// <summary>
        /// Открывает окно выбора продукта, затем окно ввода количества для выбранного продукта.
        /// Если все подтверждено, созданный OrderProduct добавляется в SelectedOrder.
        /// </summary>
        private void AddProductToOrder()
        {
            if (SelectedOrder == null)
                return;
            // Составляем список Id товаров, которые уже добавлены в накладную
            var excludedIds = SelectedOrder.OrderProducts.Select(op => op.ProductId).ToList();

            var productSelectionWindow = new ProductSelectionWindow(_productService, excludedIds);
            if (productSelectionWindow.ShowDialog() == true && productSelectionWindow.SelectedProduct != null)
            {
                Product selectedProduct = productSelectionWindow.SelectedProduct;
                var editWindow = new OrderProductEditWindow(selectedProduct);
                if (editWindow.ShowDialog() == true && editWindow.Result != null)
                {
                    OrderProduct newOp = editWindow.Result;
                    // Если позиция уже существует, обновляем количество, иначе добавляем новую
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

        /// <summary>
        /// Сохраняет заказ через сервис.
        /// Проверяет, что заказ содержит хотя бы один OrderProduct.
        /// </summary>
        private void SaveOrder()
        {
            if (SelectedOrder == null)
                return;

            if (SelectedOrder.OrderProducts.Count == 0)
            {
                MessageBox.Show("Заказ должен содержать хотя бы один товар.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Если заказ новый (например, Id == 0), вызываем AddOrder, иначе UpdateOrder.
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

        /// <summary>
        /// Проверка, что заказ можно сохранить: он не null и содержит хотя бы один товар.
        /// </summary>
        private bool CanSaveOrder()
        {
            return SelectedOrder != null && SelectedOrder.OrderProducts.Count > 0;
        }

        /// <summary>
        /// Удаляет выбранный заказ.
        /// </summary>
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
