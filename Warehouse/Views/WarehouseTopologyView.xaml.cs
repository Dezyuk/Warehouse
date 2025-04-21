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

        // Общая логика клика/рисования на Canvas
        private void ProcessClick(Point pos)
        {
            var vm = (TopologyViewModel)DataContext;
            int x = (int)(pos.X / 55);
            int y = (int)(pos.Y / 55);

            switch (vm.CurrentMode)
            {
                case TopologyMode.Add:
                    // Разрешаем добавить, если:
                    // 1) нет такой клетки, и
                    // 2) это первая клетка (vm.Cells.Count==0) ИЛИ рядом есть существующая клетка
                    if (!vm.Cells.Any(c => c.X == x && c.Y == y)
                        && (vm.Cells.Count == 0 || vm.HasNeighbor(x, y)))
                    {
                        vm.Cells.Add(new Cell
                        {
                            X = x,
                            Y = y,
                            ZoneType = vm.SelectedZoneType
                        });
                    }
                    break;

                case TopologyMode.Delete:
                    var toDelete = vm.Cells.FirstOrDefault(c => c.X == x && c.Y == y);
                    if (toDelete != null)
                    {
                        if (toDelete.Product != null)
                            vm.UnassignedItems.Add(toDelete.Product);
                        vm.Cells.Remove(toDelete);
                    }
                    break;

                case TopologyMode.ChangeType:
                    var toChange = vm.Cells.FirstOrDefault(c => c.X == x && c.Y == y);
                    if (toChange != null)
                        toChange.ZoneType = vm.SelectedZoneType;
                    break;

                    // Для режима View обрабатывается drop, здесь клики не нужны
            }
        }

        // Обработчик нажатия мыши
        private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(TopologyCanvas);
            ProcessClick(pos);
        }

        // Обработчик движения мыши (для режима Add — рисование зажатием)
        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            var vm = (TopologyViewModel)DataContext;
            if (vm.CurrentMode == TopologyMode.Add
             && e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(TopologyCanvas);
                ProcessClick(pos);
            }
        }

        // Обработка дропа товара на клетку
        private void OnCanvasDrop(object sender, DragEventArgs e)
        {
            var vm = (TopologyViewModel)DataContext;
            if (vm.CurrentMode != TopologyMode.View) return;
            if (!e.Data.GetDataPresent(typeof(Product))) return;

            var p = (Product)e.Data.GetData(typeof(Product));
            var pos = e.GetPosition(TopologyCanvas);
            int x = (int)(pos.X / 55);
            int y = (int)(pos.Y / 55);

            var cell = vm.Cells.FirstOrDefault(c => c.X == x && c.Y == y);
            if (cell == null) return;

            if (cell.ProductId == null)
            {
                cell.Product = p;
                cell.ProductId = p.Id;
                cell.Quantity = 1;
                vm.AssignedItems.Add(p);
                vm.UnassignedItems.Remove(p);
            }
            else if (cell.ProductId == p.Id && cell.Quantity < 1000)
            {
                cell.Quantity++;
            }
            else
            {
                MessageBox.Show(
                  "Нельзя положить сюда этот товар.",
                  "Ошибка",
                  MessageBoxButton.OK,
                  MessageBoxImage.Warning);
            }
        }
        private void OnProductDragStart(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element?.DataContext is Product product)
            {
                DragDrop.DoDragDrop(element, product, DragDropEffects.Move);
            }
        }
    }
}
