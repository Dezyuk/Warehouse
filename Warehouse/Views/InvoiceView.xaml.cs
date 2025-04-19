using System.Windows.Controls;
using Warehouse.ViewModels;

namespace Warehouse.Views
{
    public partial class InvoiceView : UserControl
    {
        public InvoiceView(InvoiceViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
