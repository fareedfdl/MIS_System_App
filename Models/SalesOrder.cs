using System;
using System.Collections.Generic;

namespace MIS_System_App.Models
{
    public class SalesOrder
    {
        public int Order_ID { get; set; }
        public int Customer_ID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        public int Org_ID { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // الخصائص المشتقة
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        
        // قائمة عناصر الطلب
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        // الخصائص المحسوبة
        public int ItemsCount => OrderItems.Count;
        public bool CanEdit => Status == "Pending" || Status == "Confirmed";
        public bool CanCancel => Status == "Pending" || Status == "Confirmed";
        public string StatusColor
        {
            get
            {
                switch (Status?.ToLower())
                {
                    case "pending": return "Orange";
                    case "confirmed": return "Blue";
                    case "shipped": return "Purple";
                    case "delivered": return "Green";
                    case "cancelled": return "Red";
                    default: return "Gray";
                }
            }
        }
    }

    public class OrderItem
    {
        public int Item_ID { get; set; }
        public int Order_ID { get; set; }
        public int Product_ID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // الخصائص المشتقة
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal AvailableQuantity { get; set; }
        
        // الخصائص المحسوبة
        public bool IsAvailable => AvailableQuantity >= Quantity;
        public decimal Profit => (UnitPrice * Quantity) - (GetProductCost() * Quantity);
        
        private decimal GetProductCost()
        {
            // في التطبيق الحقيقي، سيتم جلب سعر التكلفة من قاعدة البيانات
            return UnitPrice * 0.7m; // افتراضي 70% من سعر البيع
        }
    }
}
