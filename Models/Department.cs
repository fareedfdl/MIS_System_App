using System;

namespace MIS_System_App.Models
{
    public class Department
    {
        public int Dept_ID { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentLocation { get; set; }
        public int Org_ID { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // الخصائص المشتقة
        public string DisplayInfo => $"{DepartmentName} - {DepartmentLocation}";
    }
}