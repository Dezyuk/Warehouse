using System.Windows;
using Warehouse.Models;

namespace Warehouse.Views
{
    public partial class EditProductWindow : Window
    {
        public Product Product { get; private set; }

        public EditProductWindow(Product product)
        {
            InitializeComponent();
            Product = product;
            DataContext = Product;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
