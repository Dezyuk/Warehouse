//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Warehouse.Models;

//namespace Warehouse.Services
//{
//    public enum Abc { A, B, C }
//    public enum Xyz { X, Y, Z }

//    public class ProductSalesStats
//    {
//        public int ProductId { get; set; }
//        public string ProductName { get; set; }
//        public decimal TotalQty { get; set; }
//        public decimal Mean { get; set; }
//        public decimal StdDev { get; set; }
//        public Abc AbcClass { get; set; }
//        public Xyz XyzClass { get; set; }
//        public int Priority { get; set; }
//    }

//    public class AbcXyzService
//    {
//        private const decimal AbcThresholdA = 0.70m;
//        private const decimal AbcThresholdB = 0.90m;
//        private const decimal XyzThresholdX = 0.10m;
//        private const decimal XyzThresholdY = 0.25m;

//        public List<ProductSalesStats> ClassifyByAbcXyz(IEnumerable<Order> orders)
//        {
//            var filteredOrders = orders?.Where(o => o.OrderType == false).ToList() ?? new List<Order>();
//            var productSales = new Dictionary<int, Dictionary<DateTime, decimal>>();

//            foreach (var order in filteredOrders)
//            {
//                foreach (var item in order.OrderProducts)
//                {
//                    if (!productSales.ContainsKey(item.ProductId))
//                        productSales[item.ProductId] = new Dictionary<DateTime, decimal>();

//                    if (!productSales[item.ProductId].ContainsKey(order.OrderDate.Date))
//                        productSales[item.ProductId][order.OrderDate.Date] = 0;

//                    productSales[item.ProductId][order.OrderDate.Date] += item.Quantity;
//                }
//            }

//            var stats = new List<ProductSalesStats>();
//            foreach (var (productId, dailySales) in productSales)
//            {
//                var salesValues = dailySales.Values.ToList();
//                var total = salesValues.Sum();
//                var mean = salesValues.Average();
//                var stdDev = salesValues.Count > 1
//                    ? (decimal)Math.Sqrt(salesValues.Sum(v => (double)((v - mean) * (v - mean))) / (salesValues.Count - 1))
//                    : 0;

//                stats.Add(new ProductSalesStats
//                {
//                    ProductId = productId,
//                    TotalQty = total,
//                    Mean = mean,
//                    StdDev = stdDev
//                });
//            }

//            CalculateAbcClasses(stats);
//            CalculateXyzClasses(stats);
//            CalculatePriorities(stats);

//            return stats.OrderBy(s => s.Priority).ToList();
//        }

//        private void CalculateAbcClasses(List<ProductSalesStats> stats)
//        {
//            var total = stats.Sum(s => s.TotalQty);
//            if (total == 0) return;

//            decimal cumulative = 0;
//            foreach (var stat in stats.OrderByDescending(s => s.TotalQty))
//            {
//                cumulative += stat.TotalQty;
//                stat.AbcClass = cumulative <= total * AbcThresholdA ? Abc.A :
//                               cumulative <= total * AbcThresholdB ? Abc.B : Abc.C;
//            }
//        }

//        private void CalculateXyzClasses(List<ProductSalesStats> stats)
//        {
//            foreach (var stat in stats)
//            {
//                var cv = stat.Mean != 0 ? stat.StdDev / stat.Mean : 0;
//                stat.XyzClass = cv < XyzThresholdX ? Xyz.X :
//                               cv < XyzThresholdY ? Xyz.Y : Xyz.Z;
//            }
//        }

//        private void CalculatePriorities(List<ProductSalesStats> stats)
//        {
//            foreach (var stat in stats)
//            {
//                stat.Priority = (stat.AbcClass switch { Abc.A => 1, Abc.B => 2, _ => 3 }) * 10 +
//                                (stat.XyzClass switch { Xyz.X => 1, Xyz.Y => 2, _ => 3 });
//            }
//        }
//    }
//}
