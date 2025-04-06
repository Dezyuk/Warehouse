using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Warehouse.Models;
using Warehouse.Services;
using Warehouse.Helper;// Для RelayCommand
using Warehouse.Views;           // Для окон ProductSelectionWindow и OrderProductEditWindow

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

        public OrderViewModel(IOrderService orderService)
        {
            _orderService = orderService;

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

            // Открываем окно выбора продукта.
            var productSelectionWindow = new ProductSelectionWindow();
            if (productSelectionWindow.ShowDialog() == true)
            {
                Product selectedProduct = productSelectionWindow.SelectedProduct;
                if (selectedProduct != null)
                {
                    // Открываем окно ввода данных для OrderProduct (например, количество, скидку).
                    var orderProductEditWindow = new OrderProductEditWindow(selectedProduct);
                    if (orderProductEditWindow.ShowDialog() == true)
                    {
                        // Получаем созданный объект OrderProduct.
                        OrderProduct op = orderProductEditWindow.OrderProduct;

                        // Если заказ уже содержит продукт с таким ProductId, можно обновить количество.
                        // В данном примере просто добавляем новый объект.
                        SelectedOrder.OrderProducts.Add(op);

                        // Обновляем состояние, чтобы, например, команда SaveOrder стала активной.
                        OnPropertyChanged(nameof(SelectedOrder));
                    }
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
