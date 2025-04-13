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
    /// ViewModel для создания расходной (выходящей) накладной.
    /// При сохранении количество товара уменьшается.
    /// </summary>
    public class OutboundInvoiceViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        /// <summary>
        /// Объект заказа (накладной).
        /// </summary>
        public Order Invoice { get; set; }

        /// <summary>
        /// Команда для добавления товара в накладную.
        /// </summary>
        public ICommand AddProductCommand { get; }

        /// <summary>
        /// Команда для сохранения накладной.
        /// </summary>
        public ICommand SaveInvoiceCommand { get; }

        public OutboundInvoiceViewModel(IOrderService orderService, IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;

            // Инициализируем новый заказ
            Invoice = new Order
            {
                CustomerName = "Расход", // Можно разрешить ввод имени в EditOrderWindow
                OrderDate = DateTime.UtcNow,
                OrderProducts = new ObservableCollection<OrderProduct>()
            };

            AddProductCommand = new RelayCommand(AddProduct);
            SaveInvoiceCommand = new RelayCommand(SaveInvoice, () => Invoice.OrderProducts.Any());
        }

        /// <summary>
        /// Добавляет товар в заказ.
        /// Если позиция с данным товаром уже есть – обновляет количество (вычитает указанное значение).
        /// </summary>
        private void AddProduct()
        {
            // Открываем окно выбора продукта
            var productSelectionWindow = new ProductSelectionWindow(_productService);
            if (productSelectionWindow.ShowDialog() == true && productSelectionWindow.SelectedProduct != null)
            {
                var selectedProduct = productSelectionWindow.SelectedProduct;
                // Открываем окно для ввода количества для выбранного товара
                var editWindow = new OrderProductEditWindow(selectedProduct);
                if (editWindow.ShowDialog() == true && editWindow.Result != null)
                {
                    OrderProduct newOp = editWindow.Result;
                    // Если позиция уже существует, вычитаем количество
                    var existingOp = Invoice.OrderProducts.FirstOrDefault(op => op.ProductId == newOp.ProductId);
                    if (existingOp != null)
                    {
                        // Для расходной накладной количество уменьшается
                        existingOp.Quantity += newOp.Quantity; // здесь можно суммировать, если добавляется несколько раз
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
        /// Сохраняет расходную накладную.
        /// При сохранении для каждого товара количество уменьшается.
        /// </summary>
        private void SaveInvoice()
        {
            // Для каждого заказа-товара уменьшить количество товара в базе
            foreach (var op in Invoice.OrderProducts)
            {
                var product = _productService.GetProductById(op.ProductId);
                if (product != null)
                {
                    if (product.Quantity < op.Quantity)
                    {
                        MessageBox.Show($"Недостаточно товара {product.Name} для списания.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    product.Quantity -= op.Quantity;
                    _productService.UpdateProduct(product);
                }
            }

            _orderService.AddOrder(Invoice);
            MessageBox.Show("Расходная накладная успешно создана.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // Сброс заказа для следующего ввода
            Invoice = new Order
            {
                CustomerName = "Расход",
                OrderDate = DateTime.UtcNow,
                OrderProducts = new ObservableCollection<OrderProduct>()
            };
            OnPropertyChanged(nameof(Invoice));
        }
    }
}
