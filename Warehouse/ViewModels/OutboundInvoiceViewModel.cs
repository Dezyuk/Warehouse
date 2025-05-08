using System.Linq;
using System.Windows;
using Warehouse.Helper;
using Warehouse.Models;
using Warehouse.Services;

namespace Warehouse.ViewModels
{
    public class OutboundInvoiceViewModel : InvoiceViewModel
    {
        private readonly ICellService _cellService;
        private readonly PdfGenerator _pdfGenerator;
        private readonly PackProductService _packProductService;
        public OutboundInvoiceViewModel( IOrderService orderService, IProductService productService, ICellService cellService, PdfGenerator pdfGenerator, PackProductService packProductService)
            : base(orderService, productService, initialCustomerName: "Витратна накладна")
        {
            _cellService = cellService;
            _pdfGenerator = pdfGenerator;
            _packProductService = packProductService;
        }

        protected override void SaveInvoice()
        {
            foreach (var op in Invoice.OrderProducts)
            {
                var product = _productService.GetProductById(op.ProductId);
                if (product == null || product.Quantity < op.Quantity)
                {
                    MessageBox.Show(
                        $"Недостатньо товару для списання: {op.Product?.Name}",
                        "Помилка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
            }
            string message ="";
            _packProductService.PackOrder(Invoice);
            foreach (var op in Invoice.OrderProducts)
            {
                try
                {
                    _cellService.DeductFromCells(op.ProductId, op.Quantity);
                }
                catch(InvalidOperationException e)
                {
                    message += $"Товар {op.Product.Name} {op.Quantity} шт.: не розставлений на складі.\n";
                    
                }
            }
            if (!message.Equals(""))
            {
                MessageBox.Show(
                        message,
                        "Помилка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
            }

            
            foreach (var op in Invoice.OrderProducts)
            {
                var product = _productService.GetProductById(op.ProductId)!;
                product.Quantity -= op.Quantity;
                _productService.UpdateProduct(product);
            }
            Invoice.OrderType = false;
            _orderService.AddOrder(Invoice);
           _pdfGenerator.GenerateAndShowInvoice(Invoice);
            
            MessageBox.Show("Витратна накладна успішно збережена", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

            ResetInvoice("Витратна накладна");
        }
    }
}
