using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MIS_System_App.Services
{
    public class DatabaseService
    {
        private string connectionString = "Server=DESKTOP-USPRN2R\\SQLEXPRESS;Database=MIS_System;Trusted_Connection=true;TrustServerCertificate=true;";

        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في قاعدة البيانات: {ex.Message}", "خطأ");
                // إرجاع جدول فارغ بدلاً من null
                return new DataTable();
            }

            return dataTable;
        }

        public int ExecuteNonQuery(string commandText, SqlParameter[] parameters = null)
        {
            int result = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(commandText, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        result = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تنفيذ الأمر: {ex.Message}", "خطأ في قاعدة البيانات");
                return -1; // إرجاع -1 للإشارة إلى فشل العملية
            }

            return result;
        }

        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }
                        return command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تنفيذ الاستعلام: {ex.Message}", "خطأ");
                return null;
            }
        }

        public DataSet ExecuteDataSet(string query, SqlParameter[] parameters = null)
        {
            DataSet dataSet = new DataSet();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataSet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في قاعدة البيانات: {ex.Message}", "خطأ");
                // إرجاع DataSet فارغ بدلاً من null
                return new DataSet();
            }

            return dataSet;
        }

        public bool TestConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"فشل في الاتصال بقاعدة البيانات: {ex.Message}", "خطأ اتصال");
                return false;
            }
        }

        // دالة جديدة للتحقق من وجود سجل
        public bool RecordExists(string tableName, string condition, SqlParameter[] parameters = null)
        {
            string query = $"SELECT COUNT(1) FROM {tableName} WHERE {condition}";

            try
            {
                object result = ExecuteScalar(query, parameters);
                return result != null && Convert.ToInt32(result) > 0;
            }
            catch
            {
                return false;
            }
        }

        // دالة جديدة لبدء معاملة
        public SqlTransaction BeginTransaction(SqlConnection connection)
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                return connection.BeginTransaction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في بدء المعاملة: {ex.Message}", "خطأ");
                return null;
            }
        }

        // دالة جديدة لتنفيذ استعلام مع معاملة
        public int ExecuteNonQueryWithTransaction(string commandText, SqlParameter[] parameters, SqlTransaction transaction)
        {
            try
            {
                using (SqlCommand command = new SqlCommand(commandText, transaction.Connection, transaction))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تنفيذ الأمر داخل المعاملة: {ex.Message}", "خطأ");
                throw; // إعادة رمي الاستثناء للتعامل معه في المستوى الأعلى
            }
        }

        // دالة جديدة للحصول على هيكل الجدول
        public DataTable GetTableSchema(string tableName)
        {
            string query = $"SELECT TOP 0 * FROM {tableName}";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في الحصول على هيكل الجدول: {ex.Message}", "خطأ");
                return new DataTable();
            }
        }

        // دالة جديدة لتنفيذ عدة أوامر في دفعة واحدة
        public bool ExecuteBatch(string[] commands, SqlParameter[][] parametersArray = null)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            for (int i = 0; i < commands.Length; i++)
                            {
                                using (SqlCommand command = new SqlCommand(commands[i], connection, transaction))
                                {
                                    if (parametersArray != null && parametersArray.Length > i && parametersArray[i] != null)
                                    {
                                        command.Parameters.AddRange(parametersArray[i]);
                                    }
                                    command.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطأ في تنفيذ الدفعة: {ex.Message}", "خطأ");
                    return false;
                }
            }
        }

        // دالة جديدة للنسخ الاحتياطي
        public bool BackupDatabase(string backupPath)
        {
            try
            {
                string backupQuery = $"BACKUP DATABASE MIS_System TO DISK = '{backupPath}'";
                ExecuteNonQuery(backupQuery);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في النسخ الاحتياطي: {ex.Message}", "خطأ");
                return false;
            }
        }

        // دالة جديدة لاستعادة النسخة الاحتياطية
        public bool RestoreDatabase(string backupPath)
        {
            try
            {
                // يجب أن تكون قاعدة البيانات في وضع single user للاستعادة
                string singleUserQuery = "ALTER DATABASE MIS_System SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                string restoreQuery = $"RESTORE DATABASE MIS_System FROM DISK = '{backupPath}'";
                string multiUserQuery = "ALTER DATABASE MIS_System SET MULTI_USER";

                ExecuteNonQuery(singleUserQuery);
                ExecuteNonQuery(restoreQuery);
                ExecuteNonQuery(multiUserQuery);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في استعادة النسخة الاحتياطية: {ex.Message}", "خطأ");
                return false;
            }
        }
    }
}