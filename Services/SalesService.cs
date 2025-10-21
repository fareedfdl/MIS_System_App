using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MIS_System_App.Services
{
    public class SalesService
    {
        private DatabaseService databaseService;

        public SalesService()
        {
            databaseService = new DatabaseService();
        }

        // إنشاء فاتورة من طلب بيع
        public bool CreateInvoiceFromOrder(int orderId)
        {
            try
            {
                string query = @"
                    INSERT INTO Invoice (issue_date, due_date, Customer_ID, status, total_amount, tax_amount, Org_ID, created_at)
                    SELECT 
                        GETDATE() as issue_date,
                        DATEADD(DAY, 30, GETDATE()) as due_date,
                        so.Customer_ID,
                        'Sent' as status,
                        so.total_amount,
                        so.total_amount * 0.15 as tax_amount,
                        so.Org_ID,
                        GETDATE()
                    FROM Sales_Order so
                    WHERE so.Order_ID = @OrderID;

                    SELECT SCOPE_IDENTITY();";

                SqlParameter[] parameters = {
                    new SqlParameter("@OrderID", orderId)
                };

                object result = databaseService.ExecuteScalar(query, parameters);
                return result != null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إنشاء الفاتورة: {ex.Message}", "خطأ");
                return false;
            }
        }

        // الحصول على إحصائيات المبيعات
        public DataTable GetSalesStatistics(DateTime fromDate, DateTime toDate)
        {
            string query = @"
                SELECT 
                    COUNT(*) as 'عدد الطلبات',
                    SUM(total_amount) as 'إجمالي المبيعات',
                    AVG(total_amount) as 'متوسط قيمة الطلب',
                    MAX(total_amount) as 'أعلى طلب',
                    MIN(total_amount) as 'أقل طلب'
                FROM Sales_Order
                WHERE order_date BETWEEN @FromDate AND @ToDate";

            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            return databaseService.ExecuteQuery(query, parameters);
        }

        // الحصول على أفضل العملاء
        public DataTable GetTopCustomers(int topCount = 10)
        {
            string query = @"
                SELECT TOP (@TopCount)
                    c.customer_name as 'اسم العميل',
                    COUNT(so.Order_ID) as 'عدد الطلبات',
                    SUM(so.total_amount) as 'إجمالي المشتريات',
                    MAX(so.order_date) as 'آخر طلب'
                FROM Sales_Order so
                INNER JOIN Customer c ON so.Customer_ID = c.Customer_ID
                GROUP BY c.customer_name
                ORDER BY SUM(so.total_amount) DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@TopCount", topCount)
            };

            return databaseService.ExecuteQuery(query, parameters);
        }

        // تحديث حالة الطلب
        public bool UpdateOrderStatus(int orderId, string status)
        {
            string query = "UPDATE Sales_Order SET status = @Status WHERE Order_ID = @OrderID";

            SqlParameter[] parameters = {
                new SqlParameter("@Status", status),
                new SqlParameter("@OrderID", orderId)
            };

            int result = databaseService.ExecuteNonQuery(query, parameters);
            return result > 0;
        }
        // إضافة هذه الدوال في نهاية class SalesService

        // الحصول على إحصائيات سريعة للمبيعات
        public DataTable GetQuickSalesStats()
        {
            string query = @"
        SELECT 
            (SELECT COUNT(*) FROM Customer) as 'عدد العملاء',
            (SELECT COUNT(*) FROM Sales_Order WHERE MONTH(order_date) = MONTH(GETDATE())) as 'طلبات الشهر',
            (SELECT ISNULL(SUM(total_amount), 0) FROM Sales_Order WHERE MONTH(order_date) = MONTH(GETDATE())) as 'مبيعات الشهر',
            (SELECT COUNT(*) FROM Sales_Order WHERE status = 'Pending') as 'طلبات معلقة'";

            return databaseService.ExecuteQuery(query);
        }

        // تحديث حالة الفاتورة
        public bool UpdateInvoiceStatus(int invoiceId, string status)
        {
            string query = "UPDATE Invoice SET status = @Status WHERE Invoice_ID = @InvoiceID";

            SqlParameter[] parameters = {
        new SqlParameter("@Status", status),
        new SqlParameter("@InvoiceID", invoiceId)
    };

            int result = databaseService.ExecuteNonQuery(query, parameters);
            return result > 0;
        }
    }
}