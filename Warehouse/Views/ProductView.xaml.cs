using System.Windows.Controls;
using Warehouse.ViewModels;

namespace Warehouse.Views
{
    public partial class ProductView : UserControl
    {
        // Конструктор принимает ProductViewModel через DI.
        public ProductView(ProductViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
