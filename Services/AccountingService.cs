using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MIS_System_App.Services
{
    public class AccountingService
    {
        private DatabaseService databaseService;

        public AccountingService()
        {
            databaseService = new DatabaseService();
        }

        // تسجيل معاملة محاسبية
        public bool RecordTransaction(int debitAccountId, int creditAccountId, decimal amount, string description, string referenceNumber = "")
        {
            string query = @"
                INSERT INTO [Transaction] (debit_account_id, credit_account_id, amount, transaction_date, description, reference_number, Org_ID, created_at)
                VALUES (@DebitAccount, @CreditAccount, @Amount, GETDATE(), @Description, @ReferenceNumber, 1, GETDATE())";

            SqlParameter[] parameters = {
                new SqlParameter("@DebitAccount", debitAccountId),
                new SqlParameter("@CreditAccount", creditAccountId),
                new SqlParameter("@Amount", amount),
                new SqlParameter("@Description", description),
                new SqlParameter("@ReferenceNumber", referenceNumber)
            };

            int result = databaseService.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // الحصول على كشف حساب
        public DataTable GetAccountStatement(int accountId, DateTime fromDate, DateTime toDate)
        {
            string query = @"
                SELECT 
                    transaction_date as 'التاريخ',
                    description as 'الوصف',
                    CASE 
                        WHEN debit_account_id = @AccountID THEN amount
                        ELSE 0 
                    END as 'مدين',
                    CASE 
                        WHEN credit_account_id = @AccountID THEN amount
                        ELSE 0 
                    END as 'دائن',
                    reference_number as 'رقم المرجع'
                FROM [Transaction]
                WHERE (debit_account_id = @AccountID OR credit_account_id = @AccountID)
                AND transaction_date BETWEEN @FromDate AND @ToDate
                ORDER BY transaction_date DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@AccountID", accountId),
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            return databaseService.ExecuteQuery(query, parameters);
        }

        // الميزانية العمومية
        public DataTable GetBalanceSheet(DateTime asOfDate)
        {
            string query = @"
                SELECT 
                    a.account_name as 'اسم الحساب',
                    a.account_type as 'نوع الحساب',
                    (SELECT ISNULL(SUM(amount), 0) FROM [Transaction] t WHERE t.debit_account_id = a.Account_ID AND t.transaction_date <= @AsOfDate) as 'إجمالي المدين',
                    (SELECT ISNULL(SUM(amount), 0) FROM [Transaction] t WHERE t.credit_account_id = a.Account_ID AND t.transaction_date <= @AsOfDate) as 'إجمالي الدائن',
                    ((SELECT ISNULL(SUM(amount), 0) FROM [Transaction] t WHERE t.debit_account_id = a.Account_ID AND t.transaction_date <= @AsOfDate) - 
                     (SELECT ISNULL(SUM(amount), 0) FROM [Transaction] t WHERE t.credit_account_id = a.Account_ID AND t.transaction_date <= @AsOfDate)) as 'الرصيد'
                FROM Account a
                WHERE a.is_active = 1
                ORDER BY a.account_type, a.account_name";

            SqlParameter[] parameters = {
                new SqlParameter("@AsOfDate", asOfDate)
            };

            return databaseService.ExecuteQuery(query, parameters);
        }

        // قائمة الدخل
        public DataTable GetIncomeStatement(DateTime fromDate, DateTime toDate)
        {
            string query = @"
                -- الإيرادات
                SELECT 'إيرادات' as 'النوع', a.account_name as 'البند', 
                       SUM(t.amount) as 'المبلغ'
                FROM [Transaction] t
                INNER JOIN Account a ON t.credit_account_id = a.Account_ID
                WHERE a.account_type = 'Revenue' 
                AND t.transaction_date BETWEEN @FromDate AND @ToDate
                GROUP BY a.account_name
                
                UNION ALL
                
                -- المصروفات
                SELECT 'مصروفات' as 'النوع', a.account_name as 'البند', 
                       SUM(t.amount) as 'المبلغ'
                FROM [Transaction] t
                INNER JOIN Account a ON t.debit_account_id = a.Account_ID
                WHERE a.account_type = 'Expense' 
                AND t.transaction_date BETWEEN @FromDate AND @ToDate
                GROUP BY a.account_name";

            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            return databaseService.ExecuteQuery(query, parameters);
        }
        // إضافة هذه الدوال في نهاية class AccountingService

        // الحصول على حركة الحسابات
        public DataTable GetAccountMovement(DateTime fromDate, DateTime toDate)
        {
            string query = @"
        SELECT 
            a.account_name as 'اسم الحساب',
            (SELECT ISNULL(SUM(amount), 0) FROM [Transaction] t WHERE t.debit_account_id = a.Account_ID AND t.transaction_date BETWEEN @FromDate AND @ToDate) as 'إجمالي المدين',
            (SELECT ISNULL(SUM(amount), 0) FROM [Transaction] t WHERE t.credit_account_id = a.Account_ID AND t.transaction_date BETWEEN @FromDate AND @ToDate) as 'إجمالي الدائن'
        FROM Account a
        WHERE a.is_active = 1
        ORDER BY a.account_name";

            SqlParameter[] parameters = {
        new SqlParameter("@FromDate", fromDate),
        new SqlParameter("@ToDate", toDate)
    };

            return databaseService.ExecuteQuery(query, parameters);
        }

        // الحصول على أرصدة الحسابات
        public DataTable GetAccountBalances(DateTime asOfDate)
        {
            string query = @"
        SELECT 
            a.account_name as 'اسم الحساب',
            a.account_type as 'نوع الحساب',
            (SELECT ISNULL(SUM(amount), 0) FROM [Transaction] t WHERE t.debit_account_id = a.Account_ID AND t.transaction_date <= @AsOfDate) as 'إجمالي المدين',
            (SELECT ISNULL(SUM(amount), 0) FROM [Transaction] t WHERE t.credit_account_id = a.Account_ID AND t.transaction_date <= @AsOfDate) as 'إجمالي الدائن',
            ((SELECT ISNULL(SUM(amount), 0) FROM [Transaction] t WHERE t.debit_account_id = a.Account_ID AND t.transaction_date <= @AsOfDate) - 
             (SELECT ISNULL(SUM(amount), 0) FROM [Transaction] t WHERE t.credit_account_id = a.Account_ID AND t.transaction_date <= @AsOfDate)) as 'الرصيد'
        FROM Account a
        WHERE a.is_active = 1
        ORDER BY a.account_type, a.account_name";

            SqlParameter[] parameters = {
        new SqlParameter("@AsOfDate", asOfDate)
    };

            return databaseService.ExecuteQuery(query, parameters);
        }
    }
}