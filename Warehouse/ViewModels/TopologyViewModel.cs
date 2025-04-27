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

        private List<Cell> _originalCells;

        public ICommand SetModeCommand { get; }
        public ICommand SetZoneTypeCommand { get; }
        public ICommand SaveTopologyCommand { get; }
        public ICommand CancelTopologyCommand { get; }

        public TopologyViewModel(ICellService cellSvc, IProductService prodSvc)
        {
            _cellSvc = cellSvc;
            _prodSvc = prodSvc;

            // загрузить из БД
            var fromDb = _cellSvc.GetAllCells().ToList();
            Cells = new ObservableCollection<Cell>(fromDb);
            _originalCells = fromDb.Select(Clone).ToList();

            var all = _prodSvc.GetAllProducts().ToList();
            AssignedItems = new ObservableCollection<Product>();
            UnassignedItems = new ObservableCollection<Product>();

            SetModeCommand = new RelayCommand<TopologyMode>(m => CurrentMode = m);
            SetZoneTypeCommand = new RelayCommand<ZoneType>(t => { SelectedZoneType = t; CurrentMode = TopologyMode.ChangeType; });
            SaveTopologyCommand = new RelayCommand(SaveTopology);
            CancelTopologyCommand = new RelayCommand(CancelTopology);
            InitializeProducts();
        }

        private void InitializeProducts()
        {
            var all = _prodSvc.GetAllProducts();  // Получаем все товары
            var cells = _cellSvc.GetAllCells();   // Получаем все ячейки

            foreach (var product in all)
            {
                var totalQuantityInCells = cells
                    .Where(c => c.ProductId == product.Id)
                    .Sum(c => c.Quantity);  // Суммируем количество товара в ячейках

                if (totalQuantityInCells > 0)
                {
                    var pro = new Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Article = product.Article,
                        Quantity = totalQuantityInCells,
                        Price = product.Price,
                        OrderProducts = product.OrderProducts
                    };
                    // Если продукт есть в ячейках, добавляем его в AssignedItems с учётом количества
                    //product.Quantity = totalQuantityInCells; // Уменьшаем общее количество продукта
                    AssignedItems.Add(pro);  // Добавляем в список расставленных товаров
                }

                // Если остаток продукта больше нуля, он еще не расставлен и остается в UnassignedItems
                if (product.Quantity - totalQuantityInCells > 0)
                {
                    var pro = new Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Article = product.Article,
                        Quantity = product.Quantity - totalQuantityInCells,
                        Price = product.Price,
                        OrderProducts = product.OrderProducts
                    };
                    //var uns = product;
                    //product.Quantity -= totalQuantityInCells;
                    UnassignedItems.Add(pro);  // Добавляем в список не расставленных товаров
                }
            }
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
            InitializeProducts();
            MessageBox.Show("Сохранено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            CurrentMode = TopologyMode.View;
        }

        private void CancelTopology()
        {
            Cells.Clear();
            foreach (var c in _originalCells.Select(Clone))
                Cells.Add(c);

            // Обновляем товары
            AssignedItems.Clear();
            UnassignedItems.Clear();
            var all = _prodSvc.GetAllProducts();
            InitializeProducts();
            //foreach (var product in all)
            //{
            //    if (Cells.Any(c => c.ProductId == product.Id))
            //    {
            //        // Если продукт уже расставлен, обновляем количество в AssignedItems
            //        var assignedCell = Cells.First(c => c.ProductId == product.Id);
            //        var quantityToPlace = Math.Min(product.Quantity, 1000 - assignedCell.Quantity);

            //        assignedCell.Quantity += quantityToPlace; // Устанавливаем количество товара в ячейке
            //        product.Quantity -= quantityToPlace; // Уменьшаем количество товара

            //        // Обновляем списки Assigned и Unassigned
            //        if (!AssignedItems.Contains(product))
            //        {
            //            AssignedItems.Add(product);
            //        }
            //        UnassignedItems.Remove(product); // Убираем из списка нерасставленных
            //    }
            //    else
            //    {
            //        // Если продукт ещё не расставлен, добавляем его в UnassignedItems
            //        UnassignedItems.Add(product);
            //    }
            //}

            // Обновляем состояние интерфейса
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
            // Проверка типа зоны
            if (cell.ZoneType != ZoneType.Storage)
            {
                MessageBox.Show("Можно размещать только в зонах хранения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Если ячейка пустая, размещаем товар
            if (cell.ProductId == null)
            {
                cell.Product = product;
                cell.ProductId = product.Id;
                cell.Quantity = product.Quantity < 1000 ? product.Quantity : 1000; // Устанавливаем количество товара в ячейке
                if (AssignedItems.Contains(product))
                {
                    // Если товар уже есть в AssignedItems, увеличиваем его количество
                    foreach (var item in AssignedItems.Where(e => e.Id == product.Id))
                    {
                        item.Quantity += 1000;
                    }

                    // Уменьшаем количество товара в UnassignedItems
                    foreach (var item in UnassignedItems.Where(e => e.Id == product.Id))
                    {
                        item.Quantity -= 1000;
                    }
                }
                else
                {
                    InitializeProducts();
                    // Если товара нет в AssignedItems, добавляем его
                    //AssignedItems.Add(product); // Перемещаем товар в список "Расставленные"

                    //// Уменьшаем количество товара в UnassignedItems
                    //foreach (var item in UnassignedItems.Where(e => e.Id == product.Id))
                    //{
                    //    item.Quantity -= cell.Quantity;
                    //    if(item.Quantity == 0)
                    //    {
                    //        UnassignedItems.Remove(item);
                    //    }
                    //}
                }
                //AssignedItems.Add(product); // Перемещаем товар в список "Расставленные"
                //UnassignedItems.Remove(product); // Убираем товар из списка "Не расставленные"
            }
            // Если товар уже в ячейке, увеличиваем количество
            else if (cell.ProductId == product.Id && cell.Quantity < 1000)
            {
                cell.Quantity++; // Увеличиваем количество товара в ячейке
            }
            else
            {
                MessageBox.Show("Эта ячейка уже занята другим товаром или количество превышает лимит.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
