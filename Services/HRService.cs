using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MIS_System_App.Services
{
    public class HRService
    {
        private DatabaseService databaseService;

        public HRService()
        {
            databaseService = new DatabaseService();
        }

        // الحصول على عدد الموظفين
        public int GetEmployeesCount()
        {
            string query = "SELECT COUNT(*) FROM Employee";
            object result = databaseService.ExecuteScalar(query);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // الحصول على إحصائيات الموظفين
        public DataTable GetEmployeeStatistics()
        {
            string query = @"
                SELECT 
                    d.department_name as 'القسم',
                    COUNT(e.Emp_ID) as 'عدد الموظفين',
                    AVG(e.base_salary) as 'متوسط الراتب',
                    SUM(e.base_salary) as 'إجمالي الرواتب'
                FROM Employee e
                INNER JOIN Department d ON e.Dept_ID = d.Dept_ID
                GROUP BY d.department_name
                ORDER BY COUNT(e.Emp_ID) DESC";

            return databaseService.ExecuteQuery(query);
        }

        // تسجيل راتب موظف
        public bool RecordPayroll(int empId, DateTime paymentDate, decimal totalSalary, 
                                decimal taxes, decimal bonuses, decimal deductions)
        {
            decimal netSalary = totalSalary + bonuses - taxes - deductions;

            string query = @"
                INSERT INTO Payroll (Emp_ID, payment_date, total_salary, taxes_deducted, 
                                   bonuses, deductions, net_salary, created_at)
                VALUES (@EmpID, @PaymentDate, @TotalSalary, @Taxes, @Bonuses, @Deductions, @NetSalary, GETDATE())";

            SqlParameter[] parameters = {
                new SqlParameter("@EmpID", empId),
                new SqlParameter("@PaymentDate", paymentDate),
                new SqlParameter("@TotalSalary", totalSalary),
                new SqlParameter("@Taxes", taxes),
                new SqlParameter("@Bonuses", bonuses),
                new SqlParameter("@Deductions", deductions),
                new SqlParameter("@NetSalary", netSalary)
            };

            int result = databaseService.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // الحصول على سجل الرواتب
        public DataTable GetPayrollHistory(int? empId = null)
        {
            string query = @"
                SELECT 
                    p.Payroll_ID as 'رقم المسير',
                    e.full_name as 'اسم الموظف',
                    p.payment_date as 'تاريخ الدفع',
                    p.total_salary as 'الراتب الإجمالي',
                    p.taxes_deducted as 'الضرائب',
                    p.bonuses as 'المكافآت',
                    p.deductions as 'الخصومات',
                    p.net_salary as 'صافي الراتب',
                    p.created_at as 'تاريخ الإنشاء'
                FROM Payroll p
                INNER JOIN Employee e ON p.Emp_ID = e.Emp_ID
                WHERE (@EmpID IS NULL OR p.Emp_ID = @EmpID)
                ORDER BY p.payment_date DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@EmpID", empId ?? (object)DBNull.Value)
            };

            return databaseService.ExecuteQuery(query, parameters);
        }
    }
}