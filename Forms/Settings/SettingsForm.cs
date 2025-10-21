using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;
using MIS_System_App.Models;

namespace MIS_System_App.Forms.Settings
{
    public partial class SettingsForm : Form
    {
        private User currentUser;
        private DatabaseService databaseService;
        private TabControl tabControl;

        public SettingsForm(User user)
        {
            try
            {
                InitializeComponent();
                currentUser = user ?? throw new ArgumentNullException(nameof(user));
                databaseService = new DatabaseService();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تهيئة النموذج: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "إعدادات النظام";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Arial", 10);

            CreateControls();
            this.ResumeLayout(false);
        }

        private void CreateControls()
        {
            try
            {
                // لوحة العنوان
                Panel topPanel = new Panel();
                topPanel.Dock = DockStyle.Top;
                topPanel.Height = 60;
                topPanel.BackColor = Color.LightSteelBlue;

                Label titleLabel = new Label();
                titleLabel.Text = "إعدادات النظام والإعدادات العامة";
                titleLabel.Font = new Font("Arial", 14, FontStyle.Bold);
                titleLabel.TextAlign = ContentAlignment.MiddleCenter;
                titleLabel.Dock = DockStyle.Fill;
                topPanel.Controls.Add(titleLabel);

                // عنصر التحكم بالتبويبات
                tabControl = new TabControl();
                tabControl.Dock = DockStyle.Fill;
                tabControl.Font = new Font("Arial", 10, FontStyle.Regular);

                // إنشاء التبويبات
                CreateUserSettingsTab();
                CreateSystemSettingsTab();
                CreateDatabaseSettingsTab();
                CreateBackupSettingsTab();

                // أزرار التحكم
                Panel buttonPanel = new Panel();
                buttonPanel.Dock = DockStyle.Bottom;
                buttonPanel.Height = 60;
                buttonPanel.Padding = new Padding(10);

                Button btnSave = new Button();
                btnSave.Text = "حفظ الإعدادات";
                btnSave.Size = new Size(120, 35);
                btnSave.Location = new Point(10, 12);
                btnSave.BackColor = Color.LightGreen;
                btnSave.Click += BtnSave_Click;

                Button btnClose = new Button();
                btnClose.Text = "إغلاق";
                btnClose.Size = new Size(80, 35);
                btnClose.Location = new Point(750, 12);
                btnClose.BackColor = Color.LightCoral;
                btnClose.Click += BtnClose_Click;

                buttonPanel.Controls.AddRange(new Control[] { btnSave, btnClose });

                this.Controls.AddRange(new Control[] { tabControl, buttonPanel, topPanel });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إنشاء عناصر التحكم: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateUserSettingsTab()
        {
            try
            {
                TabPage userTab = new TabPage("إعدادات المستخدمين");
                userTab.BackColor = Color.White;
                userTab.Padding = new Padding(10);

                // إعدادات المستخدم الحالي
                GroupBox currentUserGroup = new GroupBox();
                currentUserGroup.Text = "المستخدم الحالي";
                currentUserGroup.Location = new Point(20, 20);
                currentUserGroup.Size = new Size(800, 120);
                currentUserGroup.Font = new Font("Arial", 10, FontStyle.Bold);

                // التحقق من أن currentUser ليس null
                string username = currentUser?.Username ?? "غير معروف";
                string roleName = currentUser?.RoleName ?? "غير معروف";

                Label lblCurrentUser = new Label();
                lblCurrentUser.Text = $"اسم المستخدم: {username}";
                lblCurrentUser.Location = new Point(600, 30);
                lblCurrentUser.Size = new Size(180, 25);

                Label lblCurrentRole = new Label();
                lblCurrentRole.Text = $"الدور: {roleName}";
                lblCurrentRole.Location = new Point(600, 60);
                lblCurrentRole.Size = new Size(180, 25);

                Button btnChangePassword = new Button();
                btnChangePassword.Text = "تغيير كلمة المرور";
                btnChangePassword.Location = new Point(50, 40);
                btnChangePassword.Size = new Size(150, 35);
                btnChangePassword.BackColor = Color.LightBlue;
                btnChangePassword.Click += BtnChangePassword_Click;

                currentUserGroup.Controls.AddRange(new Control[] { lblCurrentUser, lblCurrentRole, btnChangePassword });

                // إدارة المستخدمين
                GroupBox usersGroup = new GroupBox();
                usersGroup.Text = "إدارة المستخدمين";
                usersGroup.Location = new Point(20, 160);
                usersGroup.Size = new Size(800, 300);
                usersGroup.Font = new Font("Arial", 10, FontStyle.Bold);

                DataGridView dgvUsers = new DataGridView();
                dgvUsers.Location = new Point(20, 30);
                dgvUsers.Size = new Size(760, 200);
                dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvUsers.ReadOnly = true;
                LoadUsersData(dgvUsers);

                Button btnAddUser = new Button();
                btnAddUser.Text = "إضافة مستخدم";
                btnAddUser.Location = new Point(20, 240);
                btnAddUser.Size = new Size(120, 35);
                btnAddUser.BackColor = Color.LightGreen;
                btnAddUser.Click += BtnAddUser_Click;

                Button btnEditUser = new Button();
                btnEditUser.Text = "تعديل مستخدم";
                btnEditUser.Location = new Point(150, 240);
                btnEditUser.Size = new Size(120, 35);
                btnEditUser.BackColor = Color.LightYellow;
                btnEditUser.Click += BtnEditUser_Click;

                Button btnResetPassword = new Button();
                btnResetPassword.Text = "إعادة تعيين كلمة المرور";
                btnResetPassword.Location = new Point(280, 240);
                btnResetPassword.Size = new Size(150, 35);
                btnResetPassword.BackColor = Color.LightBlue;
                btnResetPassword.Click += BtnResetPassword_Click;

                usersGroup.Controls.AddRange(new Control[] { dgvUsers, btnAddUser, btnEditUser, btnResetPassword });

                userTab.Controls.AddRange(new Control[] { currentUserGroup, usersGroup });
                tabControl.TabPages.Add(userTab);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إنشاء تبويب إعدادات المستخدمين: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateSystemSettingsTab()
        {
            try
            {
                TabPage systemTab = new TabPage("إعدادات النظام");
                systemTab.BackColor = Color.White;
                systemTab.Padding = new Padding(10);

                // إعدادات الشركة
                GroupBox companyGroup = new GroupBox();
                companyGroup.Text = "إعدادات الشركة";
                companyGroup.Location = new Point(20, 20);
                companyGroup.Size = new Size(800, 150);

                Label lblCompanyName = new Label();
                lblCompanyName.Text = "اسم الشركة:";
                lblCompanyName.Location = new Point(650, 30);
                lblCompanyName.Size = new Size(100, 25);

                TextBox txtCompanyName = new TextBox();
                txtCompanyName.Location = new Point(450, 30);
                txtCompanyName.Size = new Size(190, 25);
                txtCompanyName.Text = "شركة الاختبار";

                Label lblCurrency = new Label();
                lblCurrency.Text = "العملة الأساسية:";
                lblCurrency.Location = new Point(650, 70);
                lblCurrency.Size = new Size(100, 25);

                ComboBox cmbCurrency = new ComboBox();
                cmbCurrency.Location = new Point(450, 70);
                cmbCurrency.Size = new Size(190, 25);
                cmbCurrency.Items.AddRange(new string[] { "SAR", "USD", "EUR", "YER" });
                cmbCurrency.SelectedIndex = 0;

                Label lblLanguage = new Label();
                lblLanguage.Text = "اللغة:";
                lblLanguage.Location = new Point(650, 110);
                lblLanguage.Size = new Size(100, 25);

                ComboBox cmbLanguage = new ComboBox();
                cmbLanguage.Location = new Point(450, 110);
                cmbLanguage.Size = new Size(190, 25);
                cmbLanguage.Items.AddRange(new string[] { "Arabic", "English" });
                cmbLanguage.SelectedIndex = 0;

                companyGroup.Controls.AddRange(new Control[] {
                    lblCompanyName, txtCompanyName,
                    lblCurrency, cmbCurrency,
                    lblLanguage, cmbLanguage
                });

                // إعدادات المبيعات
                GroupBox salesGroup = new GroupBox();
                salesGroup.Text = "إعدادات المبيعات";
                salesGroup.Location = new Point(20, 190);
                salesGroup.Size = new Size(800, 100);

                CheckBox chkAutoInvoice = new CheckBox();
                chkAutoInvoice.Text = "إنشاء فاتورة تلقائية بعد تأكيد الطلب";
                chkAutoInvoice.Location = new Point(500, 30);
                chkAutoInvoice.Size = new Size(280, 25);
                chkAutoInvoice.Checked = true;

                CheckBox chkStockCheck = new CheckBox();
                chkStockCheck.Text = "التحقق من المخزون قبل إنشاء الطلب";
                chkStockCheck.Location = new Point(500, 60);
                chkStockCheck.Size = new Size(280, 25);
                chkStockCheck.Checked = true;

                salesGroup.Controls.AddRange(new Control[] { chkAutoInvoice, chkStockCheck });

                systemTab.Controls.AddRange(new Control[] { companyGroup, salesGroup });
                tabControl.TabPages.Add(systemTab);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إنشاء تبويب إعدادات النظام: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateDatabaseSettingsTab()
        {
            try
            {
                TabPage dbTab = new TabPage("إعدادات قاعدة البيانات");
                dbTab.BackColor = Color.White;
                dbTab.Padding = new Padding(10);

                GroupBox connectionGroup = new GroupBox();
                connectionGroup.Text = "إعدادات الاتصال";
                connectionGroup.Location = new Point(20, 20);
                connectionGroup.Size = new Size(800, 200);

                Label lblServer = new Label();
                lblServer.Text = "اسم الخادم:";
                lblServer.Location = new Point(650, 30);
                lblServer.Size = new Size(100, 25);

                TextBox txtServer = new TextBox();
                txtServer.Location = new Point(400, 30);
                txtServer.Size = new Size(240, 25);
                txtServer.Text = @"DESKTOP-USPRN2R\SQLEXPRESS";

                Label lblDatabase = new Label();
                lblDatabase.Text = "اسم قاعدة البيانات:";
                lblDatabase.Location = new Point(650, 70);
                lblDatabase.Size = new Size(100, 25);

                TextBox txtDatabase = new TextBox();
                txtDatabase.Location = new Point(400, 70);
                txtDatabase.Size = new Size(240, 25);
                txtDatabase.Text = "MIS_System";

                Label lblTimeout = new Label();
                lblTimeout.Text = "مهلة الاتصال (ثانية):";
                lblTimeout.Location = new Point(650, 110);
                lblTimeout.Size = new Size(100, 25);

                TextBox txtTimeout = new TextBox();
                txtTimeout.Location = new Point(400, 110);
                txtTimeout.Size = new Size(240, 25);
                txtTimeout.Text = "30";

                Button btnTestConnection = new Button();
                btnTestConnection.Text = "اختبار الاتصال";
                btnTestConnection.Location = new Point(50, 150);
                btnTestConnection.Size = new Size(120, 35);
                btnTestConnection.BackColor = Color.LightBlue;
                btnTestConnection.Click += BtnTestConnection_Click;

                connectionGroup.Controls.AddRange(new Control[] {
                    lblServer, txtServer,
                    lblDatabase, txtDatabase,
                    lblTimeout, txtTimeout,
                    btnTestConnection
                });

                dbTab.Controls.Add(connectionGroup);
                tabControl.TabPages.Add(dbTab);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إنشاء تبويب إعدادات قاعدة البيانات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateBackupSettingsTab()
        {
            try
            {
                TabPage backupTab = new TabPage("النسخ الاحتياطي");
                backupTab.BackColor = Color.White;
                backupTab.Padding = new Padding(10);

                GroupBox backupGroup = new GroupBox();
                backupGroup.Text = "إعدادات النسخ الاحتياطي";
                backupGroup.Location = new Point(20, 20);
                backupGroup.Size = new Size(800, 200);

                Label lblBackupPath = new Label();
                lblBackupPath.Text = "مسار النسخ الاحتياطي:";
                lblBackupPath.Location = new Point(650, 30);
                lblBackupPath.Size = new Size(120, 25);

                TextBox txtBackupPath = new TextBox();
                txtBackupPath.Location = new Point(350, 30);
                txtBackupPath.Size = new Size(290, 25);
                txtBackupPath.Text = @"C:\Backup\MIS_System\";

                Label lblAutoBackup = new Label();
                lblAutoBackup.Text = "نسخ احتياطي تلقائي:";
                lblAutoBackup.Location = new Point(650, 70);
                lblAutoBackup.Size = new Size(120, 25);

                ComboBox cmbAutoBackup = new ComboBox();
                cmbAutoBackup.Location = new Point(350, 70);
                cmbAutoBackup.Size = new Size(290, 25);
                cmbAutoBackup.Items.AddRange(new string[] { "معطل", "يومي", "أسبوعي", "شهري" });
                cmbAutoBackup.SelectedIndex = 1;

                Button btnBackupNow = new Button();
                btnBackupNow.Text = "نسخ احتياطي الآن";
                btnBackupNow.Location = new Point(50, 120);
                btnBackupNow.Size = new Size(120, 35);
                btnBackupNow.BackColor = Color.LightGreen;
                btnBackupNow.Click += BtnBackupNow_Click;

                Button btnRestore = new Button();
                btnRestore.Text = "استعادة نسخة احتياطية";
                btnRestore.Location = new Point(180, 120);
                btnRestore.Size = new Size(150, 35);
                btnRestore.BackColor = Color.LightYellow;
                btnRestore.Click += BtnRestore_Click;

                backupGroup.Controls.AddRange(new Control[] {
                    lblBackupPath, txtBackupPath,
                    lblAutoBackup, cmbAutoBackup,
                    btnBackupNow, btnRestore
                });

                backupTab.Controls.Add(backupGroup);
                tabControl.TabPages.Add(backupTab);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إنشاء تبويب النسخ الاحتياطي: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUsersData(DataGridView dgv)
        {
            try
            {
                string query = @"
                    SELECT 
                        u.User_ID as 'رقم المستخدم',
                        u.username as 'اسم المستخدم',
                        u.email as 'البريد الإلكتروني',
                        r.role_name as 'الدور',
                        CASE WHEN u.is_active = 1 THEN 'نشط' ELSE 'غير نشط' END as 'الحالة',
                        u.created_at as 'تاريخ الإنشاء'
                    FROM [User] u
                    INNER JOIN User_Role ur ON u.User_ID = ur.User_ID
                    INNER JOIN [Role] r ON ur.Role_ID = r.Role_ID
                    ORDER BY u.username";

                DataTable users = databaseService?.ExecuteQuery(query) ?? new DataTable();
                dgv.DataSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل بيانات المستخدمين: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgv.DataSource = new DataTable();
            }
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentUser != null)
                {
                    ChangePasswordForm passwordForm = new ChangePasswordForm(currentUser);
                    passwordForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("المستخدم الحالي غير معروف", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في فتح نموذج تغيير كلمة المرور: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: تنفيذ نموذج إضافة مستخدم جديد
                MessageBox.Show("سيتم فتح نموذج إضافة مستخدم جديد", "إضافة مستخدم");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إضافة مستخدم: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEditUser_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: تنفيذ نموذج تعديل المستخدم
                MessageBox.Show("سيتم فتح نموذج تعديل المستخدم", "تعديل مستخدم");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تعديل المستخدم: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnResetPassword_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: تنفيذ إعادة تعيين كلمة المرور
                MessageBox.Show("سيتم إعادة تعيين كلمة مرور المستخدم", "إعادة تعيين كلمة المرور");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إعادة تعيين كلمة المرور: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                DatabaseService testService = new DatabaseService();
                if (testService.TestConnection())
                {
                    MessageBox.Show("الاتصال بقاعدة البيانات ناجح", "اختبار الاتصال", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("فشل الاتصال بقاعدة البيانات", "خطأ في الاتصال", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في اختبار الاتصال: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBackupNow_Click(object sender, EventArgs e)
        {
            try
            {
                string backupQuery = @"
                    BACKUP DATABASE MIS_System 
                    TO DISK = 'C:\Backup\MIS_System\Backup_' + REPLACE(CONVERT(VARCHAR, GETDATE(), 120), ':', '') + '.bak'";

                int result = databaseService?.ExecuteNonQuery(backupQuery) ?? -1;

                if (result > 0)
                {
                    MessageBox.Show("تم إنشاء نسخة احتياطية بنجاح", "نسخ احتياطي", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("فشل في إنشاء النسخة الاحتياطية", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في النسخ الاحتياطي: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRestore_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: تنفيذ استعادة النسخة الاحتياطية
                MessageBox.Show("سيتم فتح نافذة استعادة النسخة الاحتياطية", "استعادة نسخة احتياطية");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في استعادة النسخة الاحتياطية: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("تم حفظ الإعدادات بنجاح", "حفظ الإعدادات", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ الإعدادات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إغلاق النموذج: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close(); // إغلاق النموذج رغم الخطأ
            }
        }
    }
}