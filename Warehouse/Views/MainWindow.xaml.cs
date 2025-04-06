using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Warehouse.Models;
using Warehouse.Services;
using Warehouse.ViewModels;

namespace Warehouse.Views
{
    public partial class MainWindow : Window
    {
        private readonly WarehouseViewModel _viewModel;
        private readonly ProductViewModel _productViewModel;
        private readonly OrderViewModel _orderViewModel;
        private bool _isDeleteMode = false;
        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(WarehouseViewModel viewModel, ProductViewModel productViewModel, OrderViewModel orderViewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _productViewModel = productViewModel;
            _orderViewModel = orderViewModel;

            DataContext = _viewModel;

            RenderWarehouse();
        }

        private void ToggleModeButton_Click(object sender, RoutedEventArgs e)
        {
            _isDeleteMode = !_isDeleteMode;
            ToggleModeButton.Content = _isDeleteMode ? "Режим: Удаление" : "Режим: Добавление";
        }

        private void RefreshWarehouse_Click(object sender, RoutedEventArgs e)
        {
            RenderWarehouse();
        }

        private void OpenProductsWindow_Click(object sender, RoutedEventArgs e)
        {
            var productWindow = new ProductWindow(_productViewModel);
            productWindow.Show();
        }

        private void OpenOrdersWindow_Click(object sender, RoutedEventArgs e)
        {
            var ordersWindow = new OrdersWindow(_orderViewModel);
            ordersWindow.Show();
        }

        private void WarehouseCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(WarehouseCanvas);
            int x = (int)(point.X / 30);
            int y = (int)(point.Y / 30);

            if (_isDeleteMode)
            {
                DeleteCell(x, y);
            }
            else
            {
                AddCell(x, y);
            }
        }

        private void AddCell(int x, int y)
        {
            if (!_viewModel.HasNeighbor(x, y)) return;

            var cell = new Rectangle
            {
                Width = 30,
                Height = 30,
                Fill = Brushes.LightBlue,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Canvas.SetLeft(cell, x * 30);
            Canvas.SetTop(cell, y * 30);
            WarehouseCanvas.Children.Add(cell);

            _viewModel.AddCell(x, y);
        }

        private void DeleteCell(int x, int y)
        {
            var cell = WarehouseCanvas.Children
                .OfType<Rectangle>()
                .FirstOrDefault(c => Canvas.GetLeft(c) == x * 30 && Canvas.GetTop(c) == y * 30);

            if (cell != null)
            {
                WarehouseCanvas.Children.Remove(cell);
                _viewModel.RemoveCell(x, y);
            }
        }

        private void RenderWarehouse()
        {
            WarehouseCanvas.Children.Clear();

            foreach (var cell in _viewModel.Cells)
            {
                var rectangle = new Rectangle
                {
                    Width = 30,
                    Height = 30,
                    Fill = Brushes.LightBlue,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                Canvas.SetLeft(rectangle, cell.X * 30);
                Canvas.SetTop(rectangle, cell.Y * 30);
                WarehouseCanvas.Children.Add(rectangle);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
