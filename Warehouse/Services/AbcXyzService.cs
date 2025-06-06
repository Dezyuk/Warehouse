﻿using System;
using System.Collections.Generic;
using System.Linq;
using Warehouse.Models;

namespace Warehouse.Services
{
    public class AbcXyzService
    {
        private const decimal AbcThresholdA = 0.70m;
        private const decimal AbcThresholdB = 0.90m;
        private const decimal XyzThresholdX = 0.10m;
        private const decimal XyzThresholdY = 0.25m;
        private readonly IProductService _productService;

        public AbcXyzService(IProductService productService)
        {
            _productService = productService;
        }

        public List<ProductSalesStats> ClassifyByAbcXyz(IEnumerable<Order> orders)
        {
            var filteredOrders = orders?.Where(o => o.OrderType == false).ToList() ?? new List<Order>();
            var productSales = new Dictionary<int, Dictionary<DateTime, decimal>>();

            foreach (var order in filteredOrders)
            {
                foreach (var item in order.OrderProducts)
                {
                    if (!productSales.ContainsKey(item.ProductId))
                        productSales[item.ProductId] = new Dictionary<DateTime, decimal>();

                    if (!productSales[item.ProductId].ContainsKey(order.OrderDate.Date))
                        productSales[item.ProductId][order.OrderDate.Date] = 0;

                    productSales[item.ProductId][order.OrderDate.Date] += item.Quantity;
                }
            }

            var stats = new List<ProductSalesStats>();
            foreach (var (productId, dailySales) in productSales)
            {
                var salesValues = dailySales.Values.ToList();
                var total = salesValues.Sum();
                var mean = salesValues.Average();
                var stdDev = salesValues.Count > 1
                    ? (decimal)Math.Sqrt(salesValues.Sum(v => (double)((v - mean) * (v - mean))) / (salesValues.Count - 1))
                    : 0;

                stats.Add(new ProductSalesStats
                {
                    ProductId = productId,
                    TotalQty = total,
                    Mean = mean,
                    StdDev = stdDev
                });
            }
            var allProducts = _productService.GetAllProducts();
            var productIdsWithStats = stats.Select(s => s.ProductId).ToHashSet();
            foreach (var product in allProducts)
            {
                if (!productIdsWithStats.Contains(product.Id))
                {
                    stats.Add(new ProductSalesStats
                    {
                        ProductId = product.Id,
                        TotalQty = 0,
                        Mean = 0,
                        StdDev = 0,
                        AbcClass = Abc.C,
                        XyzClass = Xyz.Z,
                        Priority = 33
                    });
                }
            }
            CalculateAbcClasses(stats);
            CalculateXyzClasses(stats);
            CalculatePriorities(stats);

            return stats.OrderBy(s => s.Priority).ToList();
        }

        private void CalculateAbcClasses(List<ProductSalesStats> stats)
        {
            var total = stats.Sum(s => s.TotalQty);
            if (total == 0) return;

            decimal cumulative = 0;
            foreach (var stat in stats.OrderByDescending(s => s.TotalQty))
            {
                cumulative += stat.TotalQty;
                stat.AbcClass = cumulative <= total * AbcThresholdA ? Abc.A :
                               cumulative <= total * AbcThresholdB ? Abc.B : Abc.C;
            }
        }

        private void CalculateXyzClasses(List<ProductSalesStats> stats)
        {
            foreach (var stat in stats)
            {
                var cv = stat.Mean != 0 ? stat.StdDev / stat.Mean : 0;
                stat.XyzClass = cv < XyzThresholdX ? Xyz.X :
                               cv < XyzThresholdY ? Xyz.Y : Xyz.Z;
            }
        }

        private void CalculatePriorities(List<ProductSalesStats> stats)
        {
            foreach (var stat in stats)
            {
                stat.Priority = (stat.AbcClass switch { Abc.A => 1, Abc.B => 2, _ => 3 }) * 10 +
                                (stat.XyzClass switch { Xyz.X => 1, Xyz.Y => 2, _ => 3 });
            }
        }
    }
}
