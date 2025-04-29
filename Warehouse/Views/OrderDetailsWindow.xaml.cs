using System.Linq;
using System.Windows;
using Warehouse.Models;
using Warehouse.Services;
using Warehouse.Views;

namespace Warehouse.Views
{
    public partial class OrderDetailsWindow : Window
    {
        public OrderDetailsWindow(Order order)
        {
            InitializeComponent();
            DataContext = order;
        }
    }
}
