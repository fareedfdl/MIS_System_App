using System;

namespace MIS_System_App.Models
{
    public class Supplier
    {
        public int Supplier_ID { get; set; }
        public string SupplierName { get; set; }
        public string ContactInfo { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int Org_ID { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // الخصائص المشتقة
        public string DisplayInfo => $"{SupplierName} - {PhoneNumber}";
    }
}