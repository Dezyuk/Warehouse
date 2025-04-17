using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Warehouse.Helper;
using Warehouse.Models;
using Warehouse.Services;
using Warehouse.Views;

namespace Warehouse.ViewModels
{
    
    /// Базовый ViewModel для накладных: общий код выбора товара и общая команда сохранения.
    /// Конкретная логика сохранения (приход/расход) реализуется в наследниках.
    
    public abstract class InvoiceViewModel : BaseViewModel
    {
        protected readonly IOrderService _orderService;
        protected readonly IProductService _productService;

        /// <summary>
        /// Текущая накладная.
        /// </summary>
        public Order Invoice { get; protected set; }

        public ICommand AddProductCommand { get; }
        public ICommand SaveInvoiceCommand { get; }

        protected InvoiceViewModel(
            IOrderService orderService,
            IProductService productService,
            string initialCustomerName)
        {
            _orderService = orderService;
            _productService = productService;

            Invoice = new Order
            {
                CustomerName = initialCustomerName,
                OrderDate = DateTime.UtcNow,
                OrderProducts = new ObservableCollection<OrderProduct>()
            };

            AddProductCommand = new RelayCommand(AddProduct);
            SaveInvoiceCommand = new RelayCommand(SaveInvoice, CanSaveInvoice);
        }

        private void AddProduct()
        {
            // Окно выбора товара (исключаем уже добавленные позиции)
            var excludedIds = Invoice.OrderProducts.Select(op => op.ProductId).ToList();
            var selWin = new ProductSelectionWindow(_productService, excludedIds);
            if (selWin.ShowDialog() != true || selWin.SelectedProduct == null) return;

            var editWin = new OrderProductEditWindow(selWin.SelectedProduct);
            if (editWin.ShowDialog() != true || editWin.Result == null) return;

            var newOp = editWin.Result;
            var exists = Invoice.OrderProducts.FirstOrDefault(op => op.ProductId == newOp.ProductId);
            if (exists != null)
            {
                // по умолчанию замена — если нужно суммировать, сделайте exists.Quantity += newOp.Quantity
                exists.Quantity = newOp.Quantity;
            }
            else
            {
                Invoice.OrderProducts.Add(newOp);
            }

            // обновляем UI и проверяем, можно ли теперь сохранить
            OnPropertyChanged(nameof(Invoice.OrderProducts));
            ((RelayCommand)SaveInvoiceCommand).RaiseCanExecuteChanged();
        }

        private bool CanSaveInvoice()
            => Invoice.OrderProducts.Any();

        /// <summary>
        /// Здесь происходит главная работа: сохранение накладной + изменение остатков в репозитории.
        /// Конкретная реализация (прибавить или отнять) в наследниках.
        /// </summary>
        protected abstract void SaveInvoice();

        /// <summary>
        /// Сбрасывает текущее состояние накладной (создаёт новую пустую).
        /// </summary>
        protected void ResetInvoice(string customerName)
        {
            Invoice = new Order
            {
                CustomerName = customerName,
                OrderDate = DateTime.UtcNow,
                OrderProducts = new ObservableCollection<OrderProduct>()
            };
            OnPropertyChanged(nameof(Invoice));
            ((RelayCommand)SaveInvoiceCommand).RaiseCanExecuteChanged();
        }
    }
}
