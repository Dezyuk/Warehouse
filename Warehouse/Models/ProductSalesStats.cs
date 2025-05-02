namespace Warehouse.Models
{
    public enum Abc { A, B, C }
    public enum Xyz { X, Y, Z }

    public class ProductSalesStats
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public decimal TotalQty { get; set; }
        public decimal Mean { get; set; }
        public decimal StdDev { get; set; }
        public Abc AbcClass { get; set; }
        public Xyz XyzClass { get; set; }
        public int Priority { get; set; }
    }
}
