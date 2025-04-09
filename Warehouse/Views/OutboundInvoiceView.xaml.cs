using System.Windows.Controls;
using Warehouse.ViewModels;

namespace Warehouse.Views
{
    public partial class OutboundInvoiceView : UserControl
    {
        public OutboundInvoiceView(OutboundInvoiceViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
