using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using MIS_System_App.Models;

namespace MIS_System_App.Services
{
    public class UserService
    {
        private DatabaseService databaseService;

        public UserService()
        {
            databaseService = new DatabaseService();
        }

        // دالة تسجيل الدخول المعدلة
        public User Login(string username, string password)
        {
            // جرب أولاً بالنص العادي (للكلمات المشفرة حالياً في DB)
            User user = LoginWithPlainPassword(username, password);

            if (user != null)
            {
                return user;
            }

            // إذا فشل، جرب بالتشفير (للكلمات الجديدة)
            return LoginWithEncryptedPassword(username, password);
        }

        // تسجيل الدخول بكلمة مرور نصية (للبيانات الحالية)
        private User LoginWithPlainPassword(string username, string password)
        {
            string query = @"
                SELECT u.User_ID, u.username, u.email, r.role_name, r.Role_ID, u.Org_ID, u.is_active, u.created_at
                FROM [User] u
                INNER JOIN User_Role ur ON u.User_ID = ur.User_ID
                INNER JOIN [Role] r ON ur.Role_ID = r.Role_ID
                WHERE u.username = @Username AND u.encrypted_password = @Password AND u.is_active = 1";

            SqlParameter[] parameters = {
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", password) // نص عادي
            };

            return GetUserFromQuery(query, parameters);
        }

        // تسجيل الدخول بكلمة مرور مشفرة
        private User LoginWithEncryptedPassword(string username, string password)
        {
            string encryptedPassword = EncryptPassword(password);

            string query = @"
                SELECT u.User_ID, u.username, u.email, r.role_name, r.Role_ID, u.Org_ID, u.is_active, u.created_at
                FROM [User] u
                INNER JOIN User_Role ur ON u.User_ID = ur.User_ID
                INNER JOIN [Role] r ON ur.Role_ID = r.Role_ID
                WHERE u.username = @Username AND u.encrypted_password = @Password AND u.is_active = 1";

            SqlParameter[] parameters = {
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", encryptedPassword)
            };

            return GetUserFromQuery(query, parameters);
        }

        private User GetUserFromQuery(string query, SqlParameter[] parameters)
        {
            DataTable result = databaseService.ExecuteQuery(query, parameters);

            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                return new User
                {
                    User_ID = Convert.ToInt32(row["User_ID"]),
                    Username = row["username"].ToString(),
                    Email = row["email"].ToString(),
                    RoleName = row["role_name"].ToString(),
                    Role_ID = Convert.ToInt32(row["Role_ID"]),
                    Org_ID = Convert.ToInt32(row["Org_ID"]),
                    IsActive = Convert.ToBoolean(row["is_active"]),
                    CreatedAt = Convert.ToDateTime(row["created_at"])
                };
            }

            return null;
        }

        // تشفير كلمة المرور
        public string EncryptPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password + "MIS_SYSTEM_SALT_2025");
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // تحديث كلمات المرور القديمة إلى مشفرة
        public void UpdatePasswordsToEncrypted()
        {
            string query = "SELECT User_ID, encrypted_password FROM [User]";
            DataTable users = databaseService.ExecuteQuery(query);

            foreach (DataRow row in users.Rows)
            {
                int userId = Convert.ToInt32(row["User_ID"]);
                string plainPassword = row["encrypted_password"].ToString();

                // إذا كانت كلمة المرور ليست مشفرة (أقل من 44 حرف - Base64 length)
                if (plainPassword.Length < 44)
                {
                    string encryptedPassword = EncryptPassword(plainPassword);

                    string updateQuery = "UPDATE [User] SET encrypted_password = @NewPassword WHERE User_ID = @UserID";
                    SqlParameter[] parameters = {
                        new SqlParameter("@NewPassword", encryptedPassword),
                        new SqlParameter("@UserID", userId)
                    };

                    databaseService.ExecuteNonQuery(updateQuery, parameters);
                }
            }
        }
    }
}