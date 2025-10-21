using System;

namespace MIS_System_App.Models
{
    public class Employee
    {
        public int Emp_ID { get; set; }
        public string FullName { get; set; }
        public DateTime HireDate { get; set; }
        public int Dept_ID { get; set; }
        public int? User_ID { get; set; }
        public decimal BaseSalary { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // الخصائص المشتقة
        public string DepartmentName { get; set; }
        public string Username { get; set; }
    }
}