using System;

namespace MIS_System_App.Models
{
    public class Payroll
    {
        public int Payroll_ID { get; set; }
        public int Emp_ID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalSalary { get; set; }
        public decimal TaxesDeducted { get; set; }
        public decimal Bonuses { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetSalary { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // الخصائص المشتقة
        public string EmployeeName { get; set; }
        public string DisplayInfo => $"{EmployeeName} - {NetSalary:C0} - {PaymentDate:yyyy-MM-dd}";
    }
}