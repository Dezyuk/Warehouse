using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Warehouse.Models;
using Warehouse.ViewModels;

namespace Warehouse.Views
{
    public partial class WarehouseTopologyView : UserControl
    {
        public WarehouseTopologyView(TopologyViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Обновляем ячейки
            var vm = (TopologyViewModel)DataContext;
            vm.InitializeProducts();
        }
        private void ProcessClick(Point pos)
        {
            var vm = (TopologyViewModel)DataContext;
            int x = (int)(pos.X / 55), y = (int)(pos.Y / 55);

            switch (vm.CurrentMode)
            {
                case TopologyMode.Add:
                    if (!vm.Cells.Any(c => c.X == x && c.Y == y)
                        && (vm.Cells.Count == 0 || vm.HasNeighbor(x, y)))
                    {
                        vm.Cells.Add(new Cell { X = x, Y = y, ZoneType = vm.SelectedZoneType });
                        vm.InitializeProducts();
                    }
                        //vm.Cells.Add(new Cell { X = x, Y = y, ZoneType = vm.SelectedZoneType });
                    break;

                case TopologyMode.Delete:
                    var d = vm.Cells.FirstOrDefault(c => c.X == x && c.Y == y);
                    if (d != null)
                    {
                        if (d.Product != null) vm.InitializeProducts(); ;
                        vm.Cells.Remove(d);
                    }
                    break;

                case TopologyMode.ChangeType:
                    var ch = vm.Cells.FirstOrDefault(c => c.X == x && c.Y == y);
                    if (ch != null) ch.ZoneType = vm.SelectedZoneType;
                    break;

                case TopologyMode.Сleaning:
                    var cl = vm.Cells.FirstOrDefault(c => c.X == x && c.Y == y);
                    if (cl != null)
                    {
                        if (cl.Product != null)
                        {
                            vm.СleaningCells(cl);
                        }
                        
                    }
                    break;
            }
        }

        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
            => ProcessClick(e.GetPosition(TopologyCanvas));

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            var vm = (TopologyViewModel)DataContext;
            if (e.LeftButton == MouseButtonState.Pressed
                && (vm.CurrentMode == TopologyMode.Add || vm.CurrentMode == TopologyMode.Delete))
            {
                ProcessClick(e.GetPosition(TopologyCanvas));
            }
        }
        private void OnCanvasDragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("Product")) return; // Проверяем, что перетаскиваемый объект — это товар.

            var targetCell = (sender as FrameworkElement)?.Tag as Cell;
            if (targetCell != null && targetCell.ZoneType == ZoneType.Storage)
            {
                e.Effects = DragDropEffects.Move;  // Разрешаем перемещение только в зоны хранения
            }
            else
            {
                e.Effects = DragDropEffects.None;  // Запрещаем перемещение в другие зоны
            }
        }

        private void OnCanvasDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Product"))
            {
                var product = e.Data.GetData("Product") as Product;
                var targetCell = (sender as FrameworkElement)?.Tag as Cell;

                if (targetCell != null && product != null)
                {
                    // Перемещаем товар в ячейку
                    var viewModel = (TopologyViewModel)DataContext;
                    viewModel.MoveProductToCell(targetCell, product);  // Метод из ViewModel для обновления состояния
                }
            }
        }


        private void OnProductDragStart(object sender, MouseEventArgs e)
        {
            var listBoxItem = sender as FrameworkElement;
            if (listBoxItem == null) return;

            // Извлекаем DataContext из элемента
            var product = listBoxItem.DataContext as Product;

            if (product != null)
            {
                // Начать перетаскивание
                DataObject data = new DataObject();
                data.SetData("Product", product);  // Продукт передается как DataObject
                DragDrop.DoDragDrop(listBoxItem, data, DragDropEffects.Move);
            }
        }


    }
}
