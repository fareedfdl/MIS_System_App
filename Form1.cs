using System;
using System.Windows.Forms;
using MIS_System_App.Services;
using MIS_System_App.Models;

namespace MIS_System_App
{
    public partial class Form1 : Form
    {
        private UserService userService;

        public Form1()
        {
            InitializeComponent();
            userService = new UserService();

            // اختبار اتصال قاعدة البيانات
            TestDatabaseConnection();

            // إضافة زر لتحديث كلمات المرور (مؤقت للإصلاح)
            AddUpdatePasswordButton();
        }

        private void AddUpdatePasswordButton()
        {
            Button btnUpdatePasswords = new Button();
            btnUpdatePasswords.Text = "تحديث كلمات المرور";
            btnUpdatePasswords.Size = new System.Drawing.Size(120, 30);
            btnUpdatePasswords.Location = new System.Drawing.Point(250, 230);
            btnUpdatePasswords.BackColor = System.Drawing.Color.Orange;
            btnUpdatePasswords.Click += BtnUpdatePasswords_Click;
            this.Controls.Add(btnUpdatePasswords);
        }

        private void BtnUpdatePasswords_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "هل تريد تحديث كلمات المرور إلى النسخة المشفرة؟\nهذا الإجراء لمرة واحدة فقط.",
                "تحديث كلمات المرور",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                userService.UpdatePasswordsToEncrypted();
                MessageBox.Show("تم تحديث كلمات المرور بنجاح!", "تم",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void TestDatabaseConnection()
        {
            DatabaseService dbService = new DatabaseService();
            if (!dbService.TestConnection())
            {
                MessageBox.Show("تعذر الاتصال بقاعدة البيانات. يرجى التحقق من إعدادات الخادم.", "خطأ في الاتصال",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("يرجى إدخال اسم المستخدم وكلمة المرور", "تحذير",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                User user = userService.Login(username, password);

                if (user != null)
                {
                    MessageBox.Show($"تم تسجيل الدخول بنجاح!\nمرحباً {username}\nالدور: {user.RoleName}",
                                  "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // افتح النافذة الرئيسية
                    MainForm mainForm = new MainForm(user);
                    mainForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("اسم المستخدم أو كلمة المرور غير صحيحة", "خطأ في التسجيل",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // في دالة btnLogin_Click، تحديث جزء معالجة الأخطاء
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في النظام: {ex.Message}\n\nتفاصيل التقنية: {ex.InnerException?.Message}", "خطأ",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnTestData_Click(object sender, EventArgs e)
        {
            string testData = @"بيانات الاختبار:
            
مدير النظام:
اسم المستخدم: admin
كلمة المرور: admin123

مدير مبيعات:
اسم المستخدم: sales  
كلمة المرور: sales123

موظف مخزون:
اسم المستخدم: inventory
كلمة المرور: inventory123

ملاحظة: إذا لم تعمل كلمات المرور، اضغط على زر 'تحديث كلمات المرور'";

            MessageBox.Show(testData, "بيانات تسجيل الدخول للاختبار",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}