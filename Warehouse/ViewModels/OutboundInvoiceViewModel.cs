using System.Linq;
using System.Windows;
using Warehouse.Services;

namespace Warehouse.ViewModels
{
    /// <summary>
    /// Расходная накладная — списывает остатки.
    /// </summary>
    public class OutboundInvoiceViewModel : InvoiceViewModel
    {
        public OutboundInvoiceViewModel(
            IOrderService orderService,
            IProductService productService)
            : base(orderService, productService, initialCustomerName: "Расход")
        {
        }

        protected override void SaveInvoice()
        {
            // Сначала проверяем, хватит ли остатков
            foreach (var op in Invoice.OrderProducts)
            {
                var product = _productService.GetProductById(op.ProductId);
                if (product == null || product.Quantity < op.Quantity)
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
                var product = _productService.GetProductById(op.ProductId);
                product.Quantity -= op.Quantity;
                _productService.UpdateProduct(product);
            }

            _orderService.AddOrder(Invoice);
            MessageBox.Show("Расходная накладная успешно сохранена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // Сбрасываем на новую пустую накладную
            ResetInvoice("Расход");
        }
    }
}
