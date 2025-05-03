using System.Windows.Controls;
using Warehouse.Models;
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

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
