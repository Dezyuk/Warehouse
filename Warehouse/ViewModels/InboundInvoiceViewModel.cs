using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Warehouse.Helper;           // RelayCommand
using Warehouse.Models;
using Warehouse.Services;
using Warehouse.Views;

namespace Warehouse.ViewModels
{
    /// <summary>
    /// ViewModel для создания приходной накладной.
    /// Позволяет добавлять товары в накладную, при этом одна и та же позиция не может быть добавлена повторно – количество обновляется.
    /// История накладных формируется через сохранённые заказы.
    /// </summary>
    public class InboundInvoiceViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        // Объект накладной
        public Order Invoice { get; set; }

        // Команды для добавления товара в накладную и сохранения накладной
        public ICommand AddProductCommand { get; }
        public ICommand SaveInvoiceCommand { get; }

        public InboundInvoiceViewModel(IOrderService orderService, IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;

            Invoice = new Order
            {
                CustomerName = "Приход",
                OrderDate = DateTime.UtcNow,
                OrderProducts = new System.Collections.Generic.List<OrderProduct>()
            };

            AddProductCommand = new RelayCommand(AddProduct);
            SaveInvoiceCommand = new RelayCommand(SaveInvoice, CanSaveInvoice);
        }

        /// <summary>
        /// Открывает окно выбора товара, затем окно ввода количества.
        /// Если выбранный товар уже есть в накладной, его количество обновляется.
        /// </summary>
        private void AddProduct()
        {
            // Открываем окно выбора товара с использованием реальных данных через IProductService.
            var productSelectionWindow = new ProductSelectionWindow(_productService);
            if (productSelectionWindow.ShowDialog() == true && productSelectionWindow.SelectedProduct != null)
            {
                Product selectedProduct = productSelectionWindow.SelectedProduct;
                // Открываем окно для ввода количества (OrderProductEditWindow).
                var orderProductEditWindow = new OrderProductEditWindow(selectedProduct);
                if (orderProductEditWindow.ShowDialog() == true && orderProductEditWindow.Result != null)
                {
                    OrderProduct newOp = orderProductEditWindow.Result;
                    // Проверяем, существует ли уже позиция с этим товаром.
                    var existingOp = Invoice.OrderProducts.FirstOrDefault(op => op.ProductId == newOp.ProductId);
                    if (existingOp != null)
                    {
                        // Если позиция уже есть, суммируем количество.
                        existingOp.Quantity += newOp.Quantity;
                    }
                    else
                    {
                        Invoice.OrderProducts.Add(newOp);
                    }
                    OnPropertyChanged(nameof(Invoice));
                }
            }
        }

        /// <summary>
        /// Проверка, что накладная содержит хотя бы одну позицию.
        /// </summary>
        private bool CanSaveInvoice()
        {
            return Invoice.OrderProducts.Any();
        }

        /// <summary>
        /// Сохраняет приходную накладную.
        /// При сохранении для каждого OrderProduct количество товара в Product увеличивается.
        /// </summary>
        private void SaveInvoice()
        {
            // Обновляем количество товаров в базе для каждой позиции
            foreach (var op in Invoice.OrderProducts)
            {
                var product = _productService.GetProductById(op.ProductId);
                if (product != null)
                {
                    product.Quantity += op.Quantity;
                    _productService.UpdateProduct(product);
                }
            }

            // Сохраняем заказ через сервис
            _orderService.AddOrder(Invoice);
            MessageBox.Show("Приходная накладная успешно создана.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // Создаем новый пустой заказ для дальнейшего использования
            Invoice = new Order
            {
                CustomerName = "Приход",
                OrderDate = DateTime.UtcNow,
                OrderProducts = new System.Collections.Generic.List<OrderProduct>()
            };
            OnPropertyChanged(nameof(Invoice));
        }
    }
}
