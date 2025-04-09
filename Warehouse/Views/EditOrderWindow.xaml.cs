using System.Windows;
using Warehouse.Models;
using Warehouse.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Warehouse.Views
{
    public partial class EditOrderWindow : Window
    {
        // Можно установить DataContext через конструктор, ожидая, что он будет типа Order
        public EditOrderWindow(Order order)
        {
            InitializeComponent();
            DataContext = order;
        }

        // Обработчик для кнопки "Добавить позицию"
        private void AddPosition_Click(object sender, RoutedEventArgs e)
        {
            // Создаем и открываем окно для выбора продукта
            // Здесь предполагаем, что OrderViewModel или родительский контекст организует добавление позиции.
            // Для простоты, открываем ProductSelectionWindow.
            var productSelectionWindow = new ProductSelectionWindow(App.ServiceProvider.GetService<IProductService>()); // Вариант получения сервиса через DI статически или иным способом.
            if (productSelectionWindow.ShowDialog() == true && productSelectionWindow.SelectedProduct != null)
            {
                var selectedProduct = productSelectionWindow.SelectedProduct;
                // Открываем окно редактирования для OrderProduct
                var editPositionWindow = new OrderProductEditWindow(selectedProduct);
                if (editPositionWindow.ShowDialog() == true && editPositionWindow.Result != null)
                {
                    // Добавляем позицию в заказ (DataContext)
                    if (DataContext is Order order)
                    {
                        // Проверяем, существует ли позиция
                        var existing = order.OrderProducts.Find(op => op.ProductId == selectedProduct.Id);
                        if (existing != null)
                        {
                            existing.Quantity += editPositionWindow.Result.Quantity;
                        }
                        else
                        {
                            order.OrderProducts.Add(editPositionWindow.Result);
                        }
                    }
                }
            }
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
