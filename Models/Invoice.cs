
using System;
using System.Collections.Generic;

namespace MIS_System_App.Models
{
    public class Invoice
    {
        public int Invoice_ID { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public int Customer_ID { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public int Org_ID { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // الخصائص المشتقة
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        
        // الخصائص المحسوبة
        public decimal SubTotal => TotalAmount - TaxAmount;
        public decimal GrandTotal => TotalAmount;
        public bool IsOverdue => Status != "Paid" && DueDate < DateTime.Today;
        public int DaysOverdue => IsOverdue ? (DateTime.Today - DueDate).Days : 0;
        public string StatusColor
        {
            get
            {
                switch (Status?.ToLower())
                {
                    case "draft": return "Gray";
                    case "sent": return "Blue";
                    case "paid": return "Green";
                    case "overdue": return "Red";
                    case "cancelled": return "DarkRed";
                    default: return "Black";
                }
            }
        }
        
        // العلاقات
        public List<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }

    public class InvoiceItem
    {
        public int InvoiceItem_ID { get; set; }
        public int Invoice_ID { get; set; }
        public int Product_ID { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; }
        public decimal LineTotal { get; set; }
        
        // الخصائص المشتقة
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        
        // الخصائص المحسوبة
        public decimal LineTax => LineTotal * TaxRate / 100;
        public decimal LineTotalWithTax => LineTotal + LineTax;
    }
}
