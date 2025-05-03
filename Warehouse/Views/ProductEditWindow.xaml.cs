using System.Windows;
using Warehouse.Models;

namespace Warehouse.Views
{
  
    // Логика взаимодействия для ProductEditWindow.xaml.
    // Это окно используется для редактирования или создания товара.
    // При открытии DataContext устанавливается равным объекту Product, который редактируется.
   
    public partial class ProductEditWindow : Window
    {
        public ProductEditWindow(Product product)
        {
            InitializeComponent();
            // Устанавливаем DataContext окна равным редактируемому товару
            DataContext = product;
        }

        // Обработчик для кнопки OK — подтверждает ввод данных
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            // При успешном вводе возвращаем DialogResult = true и закрываем окно
            DialogResult = true;
            Close();
        }

        // Обработчик для кнопки Отмена — отменяет изменения
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
