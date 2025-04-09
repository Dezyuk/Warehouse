using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Warehouse.Models;

namespace Warehouse.Views
{
    public partial class WarehouseTopologyView : UserControl
    {
        // Здесь можно загрузить данные (например, через WarehouseTopologyViewModel)
        // Для простоты создадим список ячеек в памяти
        private readonly System.Collections.Generic.List<Cell> _cells = new System.Collections.Generic.List<Cell>();

        public WarehouseTopologyView()
        {
            InitializeComponent();
            // Пример: загрузка существующих ячеек (здесь можно вызвать сервис или ViewModel)
            // Для демонстрации создадим одну ячейку
            _cells.Add(new Cell { Id = 1, X = 1, Y = 1, WarehouseZoneType = ZoneType.Storage });
            RenderCells();
        }

        private void RenderCells()
        {
            WarehouseCanvas.Children.Clear();
            foreach (var cell in _cells)
            {
                var rect = new Rectangle
                {
                    Width = 50,
                    Height = 50,
                    Fill = Brushes.LightBlue,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Tag = cell
                };
                Canvas.SetLeft(rect, cell.X * 55);
                Canvas.SetTop(rect, cell.Y * 55);
                WarehouseCanvas.Children.Add(rect);
            }
        }

        private void AddCell_Click(object sender, RoutedEventArgs e)
        {
            // Пример: добавление новой ячейки рядом с последней (простое правило)
            int newX = _cells.Last().X + 1;
            int newY = _cells.Last().Y;
            // Здесь можно открыть окно выбора типа ячейки
            var newCell = new Cell { Id = _cells.Count + 1, X = newX, Y = newY, WarehouseZoneType = ZoneType.Storage };
            _cells.Add(newCell);
            RenderCells();
        }

        private void DeleteCell_Click(object sender, RoutedEventArgs e)
        {
            // Пример: удаляем последнюю ячейку
            if (_cells.Any())
            {
                _cells.RemoveAt(_cells.Count - 1);
                RenderCells();
            }
        }

        private void WarehouseCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Здесь можно реализовать выбор ячейки для редактирования
        }

        // Пример обработки перетаскивания товара
        private void WarehouseCanvas_Drop(object sender, DragEventArgs e)
        {
            // Проверяем, перетащен ли товар
            if (e.Data.GetDataPresent(typeof(Product)))
            {
                var product = e.Data.GetData(typeof(Product)) as Product;
                if (product != null)
                {
                    Point dropPosition = e.GetPosition(WarehouseCanvas);
                    // Найти ячейку, в которую попадает перетаскивание
                    foreach (var child in WarehouseCanvas.Children)
                    {
                        if (child is Rectangle rect)
                        {
                            double left = Canvas.GetLeft(rect);
                            double top = Canvas.GetTop(rect);
                            if (dropPosition.X >= left && dropPosition.X <= left + rect.Width &&
                                dropPosition.Y >= top && dropPosition.Y <= top + rect.Height)
                            {
                                // Здесь можно установить, что эта ячейка теперь содержит товар
                                if (rect.Tag is Cell cell)
                                {
                                    cell.Product = product;
                                    // Дополнительно можно обновить состояние ячейки, например, изменить цвет
                                    rect.Fill = Brushes.LightGreen;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
