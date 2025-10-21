using System;

namespace MIS_System_App.Models
{
    public class Warehouse
    {
        public int Warehouse_ID { get; set; }
        public string WarehouseName { get; set; }
        public string GeographicLocation { get; set; }
        public decimal Capacity { get; set; }
        public int Org_ID { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // الخصائص المشتقة
        public string FullLocation => $"{WarehouseName} - {GeographicLocation}";
    }
}