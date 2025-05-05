using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Warehouse.Helper;
using Warehouse.Models;
using Warehouse.Services;

namespace Warehouse.ViewModels
{
    public class TopologyViewModel : BaseViewModel
    {
        private const int MaxPerCell = 1000;
        private readonly ICellService _cellSvc;
        private readonly IProductService _prodSvc;
        private readonly PlacementService _placementService;
        public ObservableCollection<Cell> Cells { get; }
        public ObservableCollection<Product> Products { get; }
        public ObservableCollection<Product> AssignedItems { get; }
        public ObservableCollection<Product> UnassignedItems { get; }
        public ObservableCollection<Order> Orders { get; } = new();

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
            set { _selectedZoneType = value; OnPropertyChanged(); CurrentMode = TopologyMode.Add; }
        }

        private List<Cell> _originalCells;

        public ICommand SetModeCommand { get; }
        public ICommand SetZoneTypeCommand { get; }
        public ICommand SaveTopologyCommand { get; }
        public ICommand CancelTopologyCommand { get; }
        public ICommand ArrangeAllStockCommand { get; }

        public TopologyViewModel(ICellService cellSvc, IProductService prodSvc, PlacementService placementService)
        {

            _cellSvc = cellSvc;
            _prodSvc = prodSvc;
            _placementService = placementService;

            var fromDb = _cellSvc.GetAllCells().ToList();
            Cells = new ObservableCollection<Cell>(fromDb);
            Products = new ObservableCollection<Product>(_prodSvc.GetAllProducts());
            _originalCells = fromDb.Select(Clone).ToList();

            var all = _prodSvc.GetAllProducts().ToList();
            AssignedItems = new ObservableCollection<Product>();
            UnassignedItems = new ObservableCollection<Product>();

            SetModeCommand = new RelayCommand<TopologyMode>(m => CurrentMode = m );
            SetZoneTypeCommand = new RelayCommand<ZoneType>(t => { SelectedZoneType = t; CurrentMode = TopologyMode.Add; });
            SaveTopologyCommand = new RelayCommand(SaveTopology);
            CancelTopologyCommand = new RelayCommand(CancelTopology);
            ArrangeAllStockCommand = new RelayCommand(ArrangeAllStock);
            InitializeProducts();
            _placementService = placementService;
        }

        public void InitializeProducts()
        {
            AssignedItems.Clear();
            UnassignedItems.Clear();
            var all = _prodSvc.GetAllProducts();  
            var cells = _cellSvc.GetAllCells();   

            foreach (var product in all)
            {
                var totalQuantityInCells = cells
                    .Where(c => c.ProductId == product.Id)
                    .Sum(c => c.Quantity);  


                if (totalQuantityInCells > 0)
                {
                    var productCopy = new Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Article = product.Article,
                        Quantity = product.Quantity,
                        Price = product.Price,
                        OrderProducts = product.OrderProducts
                    };
                    productCopy.Quantity = totalQuantityInCells;
                    AssignedItems.Add(productCopy);
                }

                if (product.Quantity - totalQuantityInCells > 0)
                {
                    var productCopy = new Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Article = product.Article,
                        Quantity = product.Quantity,
                        Price = product.Price,
                        OrderProducts = product.OrderProducts
                    };
                    productCopy.Quantity = productCopy.Quantity - totalQuantityInCells;
                    UnassignedItems.Add(productCopy);  
                }
                if(product.Quantity - totalQuantityInCells < 0)
                {
                    _cellSvc.UpdateCell(cells.Where(c => c.ProductId == product.Id).First());
                }
            }
            _cellSvc.AssignColorsToProductCells(Cells);
        }
        private Cell Clone(Cell c) => new Cell
        {
            Id = c.Id,
            X = c.X,
            Y = c.Y,
            ZoneType = c.ZoneType,
            ProductId = c.ProductId,
            Quantity = c.Quantity
        };

        private void SaveTopology()
        {
            var current = Cells.ToList();
            // новые
            foreach (var added in current.Where(c => c.Id == 0))
                _cellSvc.AddCell(added);
            // обновлённые
            foreach (var upd in current.Where(c => c.Id != 0))
                _cellSvc.UpdateCell(upd);
            // удалённые
            foreach (var del in _originalCells.Where(o => current.All(c => c.Id != o.Id)))
                _cellSvc.DeleteCell(del.Id);

            _originalCells = current.Select(Clone).ToList();
            MessageBox.Show("Сохранено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            CurrentMode = TopologyMode.View;
            InitializeProducts();
        }

        private void CancelTopology()
        {
            Cells.Clear();
            foreach (var c in _cellSvc.GetAllCells().ToList())
                Cells.Add(c);

            
            InitializeProducts();
            CurrentMode = TopologyMode.View;
        }

        public bool HasNeighbor(int x, int y) =>
            Cells.Any(c =>
               (c.X == x - 1 && c.Y == y) ||
               (c.X == x + 1 && c.Y == y) ||
               (c.X == x && c.Y == y - 1) ||
               (c.X == x && c.Y == y + 1));

        public void MoveProductToCell(Cell cell, Product product)
        {
            // проверка типа зоны
            if (cell.ZoneType != ZoneType.Storage)
            {
                MessageBox.Show("Можно размещать только в зонах хранения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // если ячейка пустая, размещаем товар
            if (cell.ProductId == null)
            {
                cell.Product = product;
                cell.ProductId = product.Id;
                cell.Quantity = product.Quantity < MaxPerCell ? product.Quantity : MaxPerCell; // устанавливаем количество товара в ячейке
                
                Cells.Remove(cell);
                Cells.Add(cell);
                _cellSvc.UpdateCell(cell);
                InitializeProducts();

            }
            // если товар уже в ячейке, увеличиваем количество
            else if (cell.ProductId == product.Id && cell.Quantity < MaxPerCell)
            {
                cell.Quantity = Math.Min(cell.Quantity + product.Quantity, MaxPerCell);
            }
            else
            {
                MessageBox.Show("Эта ячейка уже занята другим товаром или количество превышает лимит.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        public void СleaningCells(Cell cell)
        {
            cell.FillColor = null;
            cell.Product = null;
            cell.ProductId = null;
            cell.Quantity = 0;
            _cellSvc.UpdateCell(cell);
            Cells.Remove(cell);
            Cells.Add(cell);
            InitializeProducts();
        }

        public void ArrangeAllStock()
        {
            _placementService.PlaceAllProducts();

            var updatedCells = _cellSvc.GetAllCells().ToList();

            Cells.Clear();
            foreach (var cell in updatedCells)
                Cells.Add(cell);

            InitializeProducts();
            CurrentMode = TopologyMode.View;
        }
    }
}