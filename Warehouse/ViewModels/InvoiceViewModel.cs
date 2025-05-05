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
    public abstract class InvoiceViewModel : BaseViewModel
    {
        protected readonly IOrderService _orderService;
        protected readonly IProductService _productService;

        public Order Invoice { get; protected set; }

        public string SaveButtonText { get; }
        private OrderProduct? _selectedOrderProduct;
        public OrderProduct? SelectedOrderProduct
        {
            get => _selectedOrderProduct;
            set
            {
                _selectedOrderProduct = value;
                OnPropertyChanged();
                ((RelayCommand)EditProductCommand).RaiseCanExecuteChanged();
                ((RelayCommand)RemoveProductCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddProductCommand { get; }
        public ICommand SaveInvoiceCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand RemoveProductCommand { get; }

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

            // Текст кнопки зависит от вида накладной
            SaveButtonText = initialCustomerName switch
            {
                "Приходная накладная" => "Провести приходную накладную",
                "Расходная накладная" => "Провести расходную накладную",
                _ => "Сохранить"
            };

            AddProductCommand = new RelayCommand(AddProduct);
            SaveInvoiceCommand = new RelayCommand(SaveInvoice, CanSaveInvoice);
            EditProductCommand = new RelayCommand(EditProduct, () => SelectedOrderProduct != null);
            RemoveProductCommand = new RelayCommand(RemoveProduct, () => SelectedOrderProduct != null);
        }
        
        private void AddProduct()
        {
            var excluded = Invoice.OrderProducts.Select(op => op.ProductId).ToList();
            var selWin = new ProductSelectionWindow(_productService, excluded);
            if (selWin.ShowDialog() != true || selWin.SelectedProduct == null)
                return;

            var editWin = new OrderProductEditWindow(selWin.SelectedProduct);
            if (editWin.ShowDialog() != true || editWin.Result == null)
                return;

            var newOp = editWin.Result;
            var existing = Invoice.OrderProducts.FirstOrDefault(op => op.ProductId == newOp.ProductId);
            if (existing != null)
                existing.Quantity += newOp.Quantity;
            else
                Invoice.OrderProducts.Add(newOp);

            OnPropertyChanged(nameof(Invoice.OrderProducts));
            ((RelayCommand)SaveInvoiceCommand).RaiseCanExecuteChanged();
        }

        private bool CanSaveInvoice() => Invoice.OrderProducts.Any();

        
        // Сбрасывает накладную на новую пустую с тем же именем.
        
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

        protected abstract void SaveInvoice();

        private void EditProduct()
        {
            if (SelectedOrderProduct == null) return;

            var product = _productService.GetProductById(SelectedOrderProduct.ProductId);
            if (product == null) return;

            var editWindow = new OrderProductEditWindow(product, SelectedOrderProduct);
            if (editWindow.ShowDialog() != true || editWindow.Result == null) return;

            SelectedOrderProduct.Quantity = editWindow.Result.Quantity;
            OnPropertyChanged(nameof(Invoice.OrderProducts));
        }

        private void RemoveProduct()
        {
            if (SelectedOrderProduct == null) return;

            Invoice.OrderProducts.Remove(SelectedOrderProduct);
            SelectedOrderProduct = null;
            OnPropertyChanged(nameof(Invoice.OrderProducts));
            ((RelayCommand)SaveInvoiceCommand).RaiseCanExecuteChanged();
        }
    }
}
