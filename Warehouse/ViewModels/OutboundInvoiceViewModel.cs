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
            : base(orderService, productService, initialCustomerName: "Расходная накладная")
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
                        $"Недостаточно товара для списания: {op.Product?.Name}",
                        "Ошибка",
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
                    message += $"Товар {op.Product.Name} {op.Quantity} шт.: не растовлен на складе.\n";
                    
                }
            }
            if (!message.Equals(""))
            {
                MessageBox.Show(
                        message,
                        "Ошибка",
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
           // _pdfGenerator.GenerateAndShowInvoice(Invoice);//временый вызов для тестов
            
            MessageBox.Show("Расходная накладная успешно сохранена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            ResetInvoice("Расходная накладная");
        }
    }
}
