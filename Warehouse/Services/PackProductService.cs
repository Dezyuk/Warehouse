using System;
using System.Collections.Generic;
using System.Linq;
using Warehouse.Models;

namespace Warehouse.Services
{
    public class PackProductService
    {
        private const int MaxPerPallet = 1000;
        private readonly ICellService _cellService;

        public PackProductService(ICellService cellService)
        {
            _cellService = cellService;
        }

        public List<Pallet> PackOrder(Order order)
        {
            var pallets = new List<Pallet>();
            var productCells = GetProductCells(_cellService.GetAllCells().ToList());

            var remainingProducts = order.OrderProducts
                .Select(op => new OrderProduct
                {
                    ProductId = op.ProductId,
                    Product = op.Product,
                    Quantity = op.Quantity
                }).ToList();

            TryFormPerfectPallets(remainingProducts, pallets);

            PackMixedPallets(remainingProducts.Where(op => op.Quantity > 0).ToList(), productCells, pallets);

            return pallets;
        }


        private void PackMixedPallets(List<OrderProduct> remainingProducts, Dictionary<int, List<Cell>> productCells, List<Pallet> pallets)
        {
            if (remainingProducts.Count == 0)
                return;

            int totalQty = remainingProducts.Sum(p => p.Quantity);
            int minPalletCount = (int)Math.Ceiling(totalQty / (decimal)MaxPerPallet);

            var startPoints = remainingProducts
                .Select(p =>
                {
                    var cells = productCells.ContainsKey(p.ProductId) ? productCells[p.ProductId] : new List<Cell>();
                    var minDelta = cells
                        .Where(c => c.Quantity > 0)
                        .Select(c => new { Cell = c, Delta = Math.Abs(c.Quantity - p.Quantity) })
                        .OrderBy(x => x.Delta)
                        .FirstOrDefault();
                    if (minDelta != null)
                        return (p, minDelta.Cell, minDelta.Delta);
                    var placeholderCell = new Cell { Id = -1, ProductId = p.ProductId, Quantity = 0 };
                    return (p, placeholderCell, int.MaxValue);
                })
                .OrderBy(x => x.Item3)
                .Take(minPalletCount)
                .ToList();

            foreach (var start in startPoints)
            {
                var pallet = new Pallet(pallets.Count() + 1);
                pallet.Items.Add(new PalletItem(start.Item1.ProductId, start.Item1.Product.Name, start.Item1.Quantity, start.Item2));
                pallets.Add(pallet);
                remainingProducts.FirstOrDefault(c => c.ProductId == start.Item1.ProductId).Quantity = 0;
            }

            while (remainingProducts.Any(p => p.Quantity > 0))
            {

                var targetPallet = pallets
                    .OrderBy(p => p.Items.Sum(i => i.Quantity))
                    .First();

                var bestMatch = remainingProducts
                    .Where(p => p.Quantity > 0)
                    .Select(p =>
                    {
                        var cells = productCells.ContainsKey(p.ProductId) ? productCells[p.ProductId] : new List<Cell>();
                        var closest = cells
                            .Where(c => c.Quantity > 0)
                            .Select(c =>
                            {
                                int distance = targetPallet.Items.Last().ProductId == -1 ? int.MaxValue : Math.Abs(c.X - targetPallet.Items.Last().FromCell.X) + Math.Abs(c.Y - targetPallet.Items.Last().FromCell.Y);
                                return new { Cell = c, Distance = distance };
                            })
                            .OrderBy(x => x.Distance)
                            .FirstOrDefault();

                        if (closest != null)
                            return (p, closest.Cell, closest.Distance);

                        var fakeCell = new Cell { Id = -1, ProductId = p.ProductId, Quantity = 0 };
                        return (p, fakeCell, int.MaxValue);
                    })
                    .OrderBy(x => x.Item3)
                    .FirstOrDefault();

                if (bestMatch.p == null)
                    break;
                if (targetPallet.TotalQuantity + bestMatch.p.Quantity > MaxPerPallet)
                {
                    var pallet = new Pallet(pallets.Count() + 1);
                    pallet.Items.Add(new PalletItem(bestMatch.p.ProductId, bestMatch.p.Product.Name, bestMatch.p.Quantity, bestMatch.Item2));
                    pallets.Add(pallet);

                }
                else
                {
                    targetPallet.Items.Add(new PalletItem(bestMatch.p.ProductId, bestMatch.p.Product.Name, bestMatch.p.Quantity, bestMatch.Item2));
                }


                remainingProducts.First(c => c.ProductId == bestMatch.p.ProductId).Quantity = 0;
            }

        }


        private Dictionary<int, List<Cell>> GetProductCells(List<Cell> warehouseCells)
        {
            return warehouseCells
                .Where(c => c.ProductId.HasValue && c.Quantity > 0)
                .GroupBy(c => c.ProductId!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        private void TryFormPerfectPallets(List<OrderProduct> orderProducts, List<Pallet> pallets) // простая но рабочая версия
        {
            foreach (var orderProduct in orderProducts.ToList())
            {
                if (orderProduct.Quantity < MaxPerPallet)
                    continue;

                while (orderProduct.Quantity >= MaxPerPallet)
                {
                    var pallet = new Pallet(pallets.Count() + 1);
                    var fakeCell = new Cell { Id = -1, ProductId = orderProduct.ProductId, Quantity = 0 };
                    pallet.Items.Add(new PalletItem(orderProduct.ProductId, orderProduct.Product.Name, MaxPerPallet, fakeCell));
                    pallets.Add(pallet);
                    orderProduct.Quantity -= MaxPerPallet;
                }
            }
        }

    }
}