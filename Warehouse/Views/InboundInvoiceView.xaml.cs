using System.Windows.Controls;
using Warehouse.ViewModels;

namespace Warehouse.Views
{
    public partial class InboundInvoiceView : UserControl
    {
        // Ожидается, что InboundInvoiceViewModel задаётся через DI, поэтому используем конструктор с параметром.
        public InboundInvoiceView(InboundInvoiceViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
