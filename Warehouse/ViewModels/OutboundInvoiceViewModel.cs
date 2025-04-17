using System;
using System.Linq;
using System.Windows;
using Warehouse.Services;

namespace Warehouse.ViewModels
{
    /// <summary>
    /// Логика расходной накладной: списание остатков и сохранение заказа.
    /// </summary>
    public class OutboundInvoiceViewModel : InvoiceViewModel
    {
        public OutboundInvoiceViewModel(
            IOrderService orderService,
            IProductService productService)
            : base(orderService, productService, initialCustomerName: "Расход")
        { }

        protected override void SaveInvoice()
        {
            // Сначала проверка наличия
            foreach (var op in Invoice.OrderProducts)
            {
                var prod = _productService.GetProductById(op.ProductId);
                if (prod == null || prod.Quantity < op.Quantity)
                {
                    MessageBox.Show(
                        $"Недостаточно товара для списания: {op.Product?.Name}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
            }

            // Списываем остатки
            foreach (var op in Invoice.OrderProducts)
            {
                var prod = _productService.GetProductById(op.ProductId);
                prod.Quantity -= op.Quantity;
                _productService.UpdateProduct(prod);
            }

            _orderService.AddOrder(Invoice);
            MessageBox.Show("Расходная накладная успешно сохранена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // сбрасываем на новую накладную
            ResetInvoice(customerName: "Расход");
        }
    }
}
