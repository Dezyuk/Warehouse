using System.Windows;
using Warehouse.ViewModels;
using Warehouse.Services;
using Warehouse.Services;
using Warehouse.ViewModels;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using Warehouse.Models;
using Warehouse.Data.Repositories;

namespace Warehouse.Views
{
    public partial class MainWindow : Window
    {
        private WarehouseZone _warehouseZone = new WarehouseZone();
        public MainWindow()
        {
            InitializeComponent();

            //var productRepository = new ProductRepository();
            //var productService = new ProductService(productRepository);
            //var productViewModel = new ProductViewModel(productService);

            //DataContext = productViewModel;


            var cell = new Rectangle
            {
                Width = 30,
                Height = 30,
                Fill = Brushes.LightBlue,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Canvas.SetLeft(cell, 0);
            Canvas.SetTop(cell, 0);

            WarehouseCanvas.Children.Add(cell);
            _warehouseZone.Cells.Add(new Cell
            {
                X = 1,
                Y = 1,
                IsOccupied = false
            });

        }
        private void WarehouseCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && _isDeleteMode) // Режим удаления
            {

                //var cell = new Rectangle
                //{
                //    Width = 30,
                //    Height = 30,
                //    Fill = Brushes.Red,
                //    Stroke = Brushes.Black,
                //    StrokeThickness = 1
                //};

                //Canvas.SetLeft(cell, 300);
                //Canvas.SetTop(cell,  300);


                var point = e.GetPosition(WarehouseCanvas);
                int x = (int)(point.X / 30);
                int y = (int)(point.Y / 30);
                var element = e.Source as Rectangle;

                if (element != null)
                {
                    //_warehouseZone.Cells.Remove(_warehouseZone.Cells.First(c => c.X == x && c.Y == y));// Не работает весь метод(
                    WarehouseCanvas.Children.Remove(element);
                }
            }
            else if (e.ChangedButton == MouseButton.Left) // Режим добавления
            {
                var point = e.GetPosition(WarehouseCanvas);
                int x = (int)(point.X / 30);
                int y = (int)(point.Y / 30);

                if (HasNeighbor(x, y))
                {
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

                    _warehouseZone.Cells.Add(new Cell
                    {
                        X = x,
                        Y = y,
                        IsOccupied = false
                    });
                }
            }
        }
        private bool HasNeighbor(int x, int y)
        {
            return _warehouseZone.Cells.Any(c =>
                (c.X == x - 1 && c.Y == y) || 
                (c.X == x + 1 && c.Y == y) || 
                (c.X == x && c.Y == y - 1) || 
                (c.X == x && c.Y == y + 1));  
        }

        private bool _isDeleteMode = false; 

        private void ToggleModeButton_Click(object sender, RoutedEventArgs e)
        {
            _isDeleteMode = !_isDeleteMode; // Переключаем режим
            ToggleModeButton.Content = _isDeleteMode ? "Режим: Удаление" : "Режим: Добавление";
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void WarehouseCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !_isDeleteMode) 
            {
                var point = e.GetPosition(WarehouseCanvas);
                int x = (int)(point.X / 30);
                int y = (int)(point.Y / 30);

                if (HasNeighbor(x, y))
                {
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

                    _warehouseZone.Cells.Add(new Cell
                    {
                        X = x,
                        Y = y,
                        IsOccupied = false
                    });
                }
            }
        }
    }
}