using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Warehouse.Helper;
using Warehouse.Models;
using Warehouse.Services;

namespace Warehouse.ViewModels
{
    /// <summary>
    /// ViewModel для редактора топологии склада.
    /// </summary>
    public class TopologyViewModel : BaseViewModel
    {
        private readonly ICellService _cellSvc;
        private readonly IProductService _prodSvc;

        public ObservableCollection<Cell> Cells { get; }
        public ObservableCollection<Product> AssignedItems { get; }
        public ObservableCollection<Product> UnassignedItems { get; }

        private TopologyMode _currentMode = TopologyMode.View;
        public TopologyMode CurrentMode
        {
            get => _currentMode;
            set { _currentMode = value; OnPropertyChanged(); }
        }

        private ZoneType _selectedZoneType = ZoneType.Storage;
        public ZoneType SelectedZoneType
        {
            get => _selectedZoneType;
            set { _selectedZoneType = value; OnPropertyChanged(); }
        }

        public ICommand SetModeCommand { get; }
        public ICommand SetZoneTypeCommand { get; }
        public ICommand SaveTopologyCommand { get; }
        public ICommand CancelTopologyCommand { get; }

        public TopologyViewModel(ICellService cellService, IProductService productService)
        {
            _cellSvc = cellService;
            _prodSvc = productService;

            // Загрузка существующих клеток и распределение товаров
            Cells = new ObservableCollection<Cell>(_cellSvc.GetAllCells());
            var allProducts = _prodSvc.GetAllProducts().ToList();
            AssignedItems = new ObservableCollection<Product>(allProducts.Where(p => Cells.Any(c => c.ProductId == p.Id)));
            UnassignedItems = new ObservableCollection<Product>(allProducts.Except(AssignedItems));

            // Команды
            SetModeCommand = new RelayCommand<TopologyMode>(m => CurrentMode = m);
            SetZoneTypeCommand = new RelayCommand<ZoneType>(t => { SelectedZoneType = t; CurrentMode = TopologyMode.ChangeType; });
            SaveTopologyCommand = new RelayCommand<object>(_ => SaveTopology(), _ => CurrentMode != TopologyMode.View);
            CancelTopologyCommand = new RelayCommand<object>(_ => CancelTopology(), _ => CurrentMode != TopologyMode.View);
        }

        private void SaveTopology()
        {
            foreach (var c in Cells)
                _cellSvc.UpdateCell(c);

            CurrentMode = TopologyMode.View;
        }

        private void CancelTopology()
        {
            Cells.Clear();
            foreach (var c in _cellSvc.GetAllCells())
                Cells.Add(c);

            RebuildProductLists();
            CurrentMode = TopologyMode.View;
        }

        private void RebuildProductLists()
        {
            AssignedItems.Clear();
            UnassignedItems.Clear();
            var all = _prodSvc.GetAllProducts();
            foreach (var p in all)
            {
                if (Cells.Any(c => c.ProductId == p.Id))
                    AssignedItems.Add(p);
                else
                    UnassignedItems.Add(p);
            }
        }

        public bool HasNeighbor(int x, int y) =>
            Cells.Any(c =>
                (c.X == x - 1 && c.Y == y) ||
                (c.X == x + 1 && c.Y == y) ||
                (c.X == x && c.Y == y - 1) ||
                (c.X == x && c.Y == y + 1));
    }
}
