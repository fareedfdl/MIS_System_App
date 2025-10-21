using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace MIS_System_App.Services
{
    public class ReportService
    {
        private DatabaseService databaseService;

        public ReportService()
        {
            databaseService = new DatabaseService();
        }

        // تقرير المبيعات اليومية
        public DataTable GetDailySalesReport(DateTime date)
        {
            string query = @"
                SELECT 
                    so.Order_ID as 'رقم الطلب',
                    c.customer_name as 'اسم العميل',
                    so.order_date as 'تاريخ الطلب',
                    so.total_amount as 'المبلغ الإجمالي',
                    so.status as 'الحالة'
                FROM Sales_Order so
                INNER JOIN Customer c ON so.Customer_ID = c.Customer_ID
                WHERE CAST(so.order_date AS DATE) = @Date
                ORDER BY so.order_date DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@Date", date.Date)
            };

            return databaseService.ExecuteQuery(query, parameters);
        }

        // تقرير المنتجات الأكثر مبيعاً
        public DataTable GetTopSellingProducts(DateTime fromDate, DateTime toDate, int topCount = 10)
        {
            string query = @"
                SELECT TOP (@TopCount)
                    p.product_name as 'اسم المنتج',
                    SUM(oi.quantity) as 'الكمية المباعة',
                    SUM(oi.quantity * oi.unit_price) as 'إجمالي المبيعات',
                    COUNT(DISTINCT oi.Order_ID) as 'عدد الطلبات'
                FROM Order_Item oi
                INNER JOIN Product p ON oi.Product_ID = p.Product_ID
                INNER JOIN Sales_Order so ON oi.Order_ID = so.Order_ID
                WHERE so.order_date BETWEEN @FromDate AND @ToDate
                GROUP BY p.product_name
                ORDER BY SUM(oi.quantity) DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate),
                new SqlParameter("@TopCount", topCount)
            };

            return databaseService.ExecuteQuery(query, parameters);
        }

        // تقرير العملاء الأكثر شراءً
        public DataTable GetTopCustomers(DateTime fromDate, DateTime toDate, int topCount = 10)
        {
            string query = @"
                SELECT TOP (@TopCount)
                    c.customer_name as 'اسم العميل',
                    COUNT(so.Order_ID) as 'عدد الطلبات',
                    SUM(so.total_amount) as 'إجمالي المشتريات',
                    MAX(so.order_date) as 'آخر طلب'
                FROM Sales_Order so
                INNER JOIN Customer c ON so.Customer_ID = c.Customer_ID
                WHERE so.order_date BETWEEN @FromDate AND @ToDate
                GROUP BY c.customer_name
                ORDER BY SUM(so.total_amount) DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate),
                new SqlParameter("@TopCount", topCount)
            };

            return databaseService.ExecuteQuery(query, parameters);
        }

        // تقرير المخزون المنخفض
        public DataTable GetLowStockReport()
        {
            string query = @"
                SELECT 
                    product_name as 'اسم المنتج',
                    available_quantity as 'الكمية المتاحة',
                    reorder_level as 'حد إعادة الطلب',
                    unit_of_measure as 'الوحدة',
                    (available_quantity - reorder_level) as 'الفرق'
                FROM Product
                WHERE is_active = 1 AND available_quantity <= reorder_level
                ORDER BY available_quantity ASC";

            return databaseService.ExecuteQuery(query);
        }

        // تقرير المبيعات الشهرية
        public DataTable GetMonthlySalesReport(int year)
        {
            string query = @"
                SELECT 
                    DATENAME(MONTH, DATEFROMPARTS(@Year, MONTH(order_date), 1)) as 'الشهر',
                    COUNT(*) as 'عدد الطلبات',
                    SUM(total_amount) as 'إجمالي المبيعات',
                    AVG(total_amount) as 'متوسط قيمة الطلب',
                    MAX(total_amount) as 'أعلى طلب',
                    MIN(total_amount) as 'أقل طلب'
                FROM Sales_Order
                WHERE YEAR(order_date) = @Year
                GROUP BY MONTH(order_date), DATENAME(MONTH, DATEFROMPARTS(@Year, MONTH(order_date), 1))
                ORDER BY MONTH(order_date)";

            SqlParameter[] parameters = {
                new SqlParameter("@Year", year)
            };

            return databaseService.ExecuteQuery(query, parameters);
        }

        // تقرير أداء الموظفين
        public DataTable GetEmployeePerformanceReport(DateTime fromDate, DateTime toDate)
        {
            string query = @"
                SELECT 
                    e.full_name as 'اسم الموظف',
                    d.department_name as 'القسم',
                    COUNT(so.Order_ID) as 'عدد الطلبات المعالجة',
                    SUM(so.total_amount) as 'إجمالي المبيعات',
                    AVG(so.total_amount) as 'متوسط قيمة الطلب'
                FROM Sales_Order so
                INNER JOIN Employee e ON so.Org_ID = e.Org_ID  -- علاقة مبسطة
                INNER JOIN Department d ON e.Dept_ID = d.Dept_ID
                WHERE so.order_date BETWEEN @FromDate AND @ToDate
                GROUP BY e.full_name, d.department_name
                ORDER BY SUM(so.total_amount) DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            return databaseService.ExecuteQuery(query, parameters);
        }

        // إحصائيات سريعة للوحة التحكم
        public DataTable GetDashboardStats()
        {
            string query = @"
                SELECT 
                    (SELECT COUNT(*) FROM Customer) as 'عدد العملاء',
                    (SELECT COUNT(*) FROM Product WHERE is_active = 1) as 'عدد المنتجات',
                    (SELECT COUNT(*) FROM Employee) as 'عدد الموظفين',
                    (SELECT COUNT(*) FROM Sales_Order WHERE CAST(order_date AS DATE) = CAST(GETDATE() AS DATE)) as 'طلبات اليوم',
                    (SELECT ISNULL(SUM(total_amount), 0) FROM Sales_Order WHERE CAST(order_date AS DATE) = CAST(GETDATE() AS DATE)) as 'مبيعات اليوم',
                    (SELECT COUNT(*) FROM Product WHERE available_quantity <= reorder_level AND is_active = 1) as 'منتجات تحتاج إعادة طلب'";

            return databaseService.ExecuteQuery(query);
        }

        // تصدير التقرير لملف Excel
        public void ExportToExcel(DataTable dataTable, string fileName)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.csv";
                saveDialog.FileName = fileName;

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(saveDialog.FileName, false, System.Text.Encoding.UTF8))
                    {
                        // كتابة العناوين
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            sw.Write(dataTable.Columns[i].ColumnName);
                            if (i < dataTable.Columns.Count - 1)
                                sw.Write(",");
                        }
                        sw.WriteLine();

                        // كتابة البيانات
                        foreach (DataRow row in dataTable.Rows)
                        {
                            for (int i = 0; i < dataTable.Columns.Count; i++)
                            {
                                string value = row[i].ToString().Replace(",", ";");
                                sw.Write(value);
                                if (i < dataTable.Columns.Count - 1)
                                    sw.Write(",");
                            }
                            sw.WriteLine();
                        }
                    }

                    MessageBox.Show($"تم تصدير التقرير بنجاح إلى: {saveDialog.FileName}", "تصدير ناجح");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في التصدير: {ex.Message}", "خطأ في التصدير");
            }
        }

        // إنشاء تقرير مخصص
        public DataTable GetCustomReport(string customQuery, SqlParameter[] parameters = null)
        {
            try
            {
                return databaseService.ExecuteQuery(customQuery, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تنفيذ التقرير المخصص: {ex.Message}", "خطأ");
                return new DataTable();
            }
        }
    }
}
