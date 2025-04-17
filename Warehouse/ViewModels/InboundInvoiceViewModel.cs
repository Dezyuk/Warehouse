using System;
using System.Windows;
using Warehouse.Models;
using Warehouse.Services;

namespace Warehouse.ViewModels
{
    /// <summary>
    /// Логика приходной накладной: пополнение остатков и сохранение заказа.
    /// </summary>
    public class InboundInvoiceViewModel : InvoiceViewModel
    {
        public InboundInvoiceViewModel(
            IOrderService orderService,
            IProductService productService)
            : base(orderService, productService, initialCustomerName: "Приход")
        { }

        protected override void SaveInvoice()
        {
            // Для каждой позиции увеличиваем остаток
            foreach (var op in Invoice.OrderProducts)
            {
                var prod = _productService.GetProductById(op.ProductId);
                if (prod != null)
                {
                    prod.Quantity += op.Quantity;
                    _productService.UpdateProduct(prod);
                }
            }

            _orderService.AddOrder(Invoice);
            MessageBox.Show("Приходная накладная успешно сохранена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // сбрасываем на новую накладную
            ResetInvoice(customerName: "Приход");
        }
    }
}
