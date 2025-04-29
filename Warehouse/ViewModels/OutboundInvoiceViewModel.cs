using System.Linq;
using System.Windows;
using Warehouse.Models;
using Warehouse.Services;

namespace Warehouse.ViewModels
{
    /// <summary>
    /// Расходная накладная — списывает остатки.
    /// </summary>
    public class OutboundInvoiceViewModel : InvoiceViewModel
    {
        private readonly ICellService _cellService;
        public OutboundInvoiceViewModel(
            IOrderService orderService,
            IProductService productService,
            ICellService cellService)
            : base(orderService, productService, initialCustomerName: "Расход")
        {
            _cellService = cellService;
        }

        protected override void SaveInvoice()
        {
            // Проверка остатков
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
            // Списание из ячеек
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
            

            // Обновление общего остатка товара
            foreach (var op in Invoice.OrderProducts)
            {
                var product = _productService.GetProductById(op.ProductId)!;
                product.Quantity -= op.Quantity;
                _productService.UpdateProduct(product);
            }

            _orderService.AddOrder(Invoice);
            MessageBox.Show("Расходная накладная успешно сохранена", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            ResetInvoice("Расход");
        }
    }
}
