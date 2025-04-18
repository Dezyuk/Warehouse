using System.Windows.Controls;
using System.Windows.Input;
using Warehouse.ViewModels;

namespace Warehouse.Views
{
    public partial class InvoiceHistoryView : UserControl
    {
        public InvoiceHistoryView(InvoiceHistoryViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is InvoiceHistoryViewModel vm
                && vm.EditCommand.CanExecute(null))
            {
                vm.EditCommand.Execute(null);
            }
        }
    }
}
