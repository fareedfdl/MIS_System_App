using System;

namespace MIS_System_App.Models
{
    public class Product
    {
        public int Product_ID { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int AvailableQuantity { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int ReorderLevel { get; set; }
        public int Org_ID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // الخصائص المشتقة
        public decimal ProfitMargin => SellingPrice - CostPrice;
        public bool NeedsReorder => AvailableQuantity <= ReorderLevel;
    }
}