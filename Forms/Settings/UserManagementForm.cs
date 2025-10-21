using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;
using MIS_System_App.Models;
using System.Data.SqlClient;
using MIS_System_App.Forms.Settings;


namespace MIS_System_App.Forms.Settings
{
    public partial class UserManagementForm : Form
    {
        private User currentUser;
        private DatabaseService databaseService;
        private DataGridView dgvUsers;
        private TextBox txtSearch;
        private Button btnAdd, btnEdit, btnDelete, btnResetPassword, btnRefresh;

        public UserManagementForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            databaseService = new DatabaseService();
            LoadUsers();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "إدارة المستخدمين";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Arial", 10);

            CreateControls();
            this.ResumeLayout(false);
        }

        private void CreateControls()
        {
            // لوحة العنوان
            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 80;
            topPanel.BackColor = Color.LightSteelBlue;
            topPanel.Padding = new Padding(10);

            Label titleLabel = new Label();
            titleLabel.Text = "إدارة المستخدمين والصلاحيات";
            titleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 40;

            // شريط البحث
            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Height = 40;

            Label lblSearch = new Label();
            lblSearch.Text = "بحث:";
            lblSearch.Location = new Point(10, 10);
            lblSearch.Size = new Size(40, 20);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(60, 8);
            txtSearch.Size = new Size(200, 25);
            txtSearch.TextChanged += TxtSearch_TextChanged;

            searchPanel.Controls.AddRange(new Control[] { lblSearch, txtSearch });
            topPanel.Controls.AddRange(new Control[] { titleLabel, searchPanel });

            // شبكة البيانات
            dgvUsers = new DataGridView();
            dgvUsers.Dock = DockStyle.Fill;
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsers.ReadOnly = true;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.RowHeadersVisible = false;

            // أزرار التحكم
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);

            btnAdd = new Button();
            btnAdd.Text = "إضافة مستخدم";
            btnAdd.Size = new Size(120, 35);
            btnAdd.Location = new Point(10, 12);
            btnAdd.BackColor = Color.LightGreen;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button();
            btnEdit.Text = "تعديل مستخدم";
            btnEdit.Size = new Size(120, 35);
            btnEdit.Location = new Point(140, 12);
            btnEdit.BackColor = Color.LightYellow;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button();
            btnDelete.Text = "حذف مستخدم";
            btnDelete.Size = new Size(120, 35);
            btnDelete.Location = new Point(270, 12);
            btnDelete.BackColor = Color.LightCoral;
            btnDelete.Click += BtnDelete_Click;

            btnResetPassword = new Button();
            btnResetPassword.Text = "إعادة تعيين كلمة المرور";
            btnResetPassword.Size = new Size(150, 35);
            btnResetPassword.Location = new Point(400, 12);
            btnResetPassword.BackColor = Color.LightBlue;
            btnResetPassword.Click += BtnResetPassword_Click;

            btnRefresh = new Button();
            btnRefresh.Text = "تحديث";
            btnRefresh.Size = new Size(120, 35);
            btnRefresh.Location = new Point(560, 12);
            btnRefresh.BackColor = Color.LightGray;
            btnRefresh.Click += BtnRefresh_Click;

            Button btnClose = new Button();
            btnClose.Text = "إغلاق";
            btnClose.Size = new Size(80, 35);
            btnClose.Location = new Point(850, 12);
            btnClose.BackColor = Color.LightCoral;
            btnClose.Click += BtnClose_Click;

            buttonPanel.Controls.AddRange(new Control[] { 
                btnAdd, btnEdit, btnDelete, btnResetPassword, btnRefresh, btnClose 
            });

            this.Controls.AddRange(new Control[] { dgvUsers, buttonPanel, topPanel });
        }

        private void LoadUsers()
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

            DataTable users = databaseService.ExecuteQuery(query);
            dgvUsers.DataSource = users;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dgvUsers.DataSource is DataTable dataTable)
            {
                string searchText = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    dataTable.DefaultView.RowFilter = 
                        $"`اسم المستخدم` LIKE '%{searchText}%' OR " +
                        $"`البريد الإلكتروني` LIKE '%{searchText}%' OR " +
                        $"`الدور` LIKE '%{searchText}%'";
                }
                else
                {
                    dataTable.DefaultView.RowFilter = "";
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            UserForm userForm = new UserForm();
            if (userForm.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
                MessageBox.Show("تم إضافة المستخدم بنجاح", "نجاح");
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["رقم المستخدم"].Value);
                UserForm userForm = new UserForm(userId);
                if (userForm.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                    MessageBox.Show("تم تحديث المستخدم بنجاح", "نجاح");
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار مستخدم للتعديل", "تحذير");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["رقم المستخدم"].Value);
                string username = dgvUsers.SelectedRows[0].Cells["اسم المستخدم"].Value.ToString();
                
                // منع حذف المستخدم الحالي
                if (userId == currentUser.User_ID)
                {
                    MessageBox.Show("لا يمكن حذف المستخدم الحالي", "تحذير");
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"هل أنت متأكد من حذف المستخدم: {username}؟", 
                    "تأكيد الحذف", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    string query = "DELETE FROM [User] WHERE User_ID = @UserID";
                    SqlParameter[] parameters = {
                        new SqlParameter("@UserID", userId)
                    };

                    int rowsAffected = databaseService.ExecuteNonQuery(query, parameters);
                    
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("تم حذف المستخدم بنجاح", "نجاح");
                        LoadUsers();
                    }
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار مستخدم للحذف", "تحذير");
            }
        }

        private void BtnResetPassword_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["رقم المستخدم"].Value);
                string username = dgvUsers.SelectedRows[0].Cells["اسم المستخدم"].Value.ToString();
                
                string newPassword = "123456"; // كلمة مرور افتراضية
                UserService userService = new UserService();
                string encryptedPassword = userService.EncryptPassword(newPassword);

                string query = "UPDATE [User] SET encrypted_password = @Password WHERE User_ID = @UserID";
                SqlParameter[] parameters = {
                    new SqlParameter("@Password", encryptedPassword),
                    new SqlParameter("@UserID", userId)
                };

                int rowsAffected = databaseService.ExecuteNonQuery(query, parameters);
                
                if (rowsAffected > 0)
                {
                    MessageBox.Show($"تم إعادة تعيين كلمة المرور للمستخدم: {username}\nكلمة المرور الجديدة: 123456", "نجاح");
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار مستخدم لإعادة تعيين كلمة المرور", "تحذير");
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadUsers();
            txtSearch.Clear();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}