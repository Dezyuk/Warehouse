using System.Windows;
using Warehouse.Models;

namespace Warehouse.Views
{
    public partial class ProductEditWindow : Window
    {
        public ProductEditWindow(Product product)
        {
            InitializeComponent();
            DataContext = product;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
