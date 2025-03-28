using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Warehouse.ViewModels;

namespace Warehouse.Views
{
    /// <summary>
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private readonly ProductViewModel _viewModel;

        public ProductWindow(ProductViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Логика для добавления товара
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            // Логика для редактирования товара
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            // Логика для удаления товара
        }
    }
}
