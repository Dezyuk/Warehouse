using System.Windows;
using Warehouse.Services;

namespace Warehouse.ViewModels
{
    public class InboundInvoiceViewModel : InvoiceViewModel
    {
        public InboundInvoiceViewModel(
            IOrderService orderService,
            IProductService productService)
            : base(orderService, productService, initialCustomerName: "Приходная накладная")
        {
        }

        protected override void SaveInvoice()
        {
            foreach (var op in Invoice.OrderProducts)
            {
                var product = _productService.GetProductById(op.ProductId);
                if (product != null)
                {
                    product.Quantity += op.Quantity;
                    _productService.UpdateProduct(product);
                }
            }
            Invoice.OrderType = true;
            _orderService.AddOrder(Invoice);
            MessageBox.Show("Приходная накладная успешно сохранена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            ResetInvoice("Приходная накладная");
        }
    }
}
