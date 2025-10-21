using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.Accounting
{
    public partial class AccountManagementForm : Form
    {
        private DatabaseService databaseService;
        private DataGridView dgvAccounts;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;

        public AccountManagementForm()
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            LoadAccounts();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "إدارة الحسابات";
            this.Size = new Size(800, 500);
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
            topPanel.Height = 60;
            topPanel.BackColor = Color.LightGreen;

            Label titleLabel = new Label();
            titleLabel.Text = "إدارة الحسابات المحاسبية";
            titleLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Fill;
            topPanel.Controls.Add(titleLabel);

            // شبكة البيانات
            dgvAccounts = new DataGridView();
            dgvAccounts.Dock = DockStyle.Fill;
            dgvAccounts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAccounts.ReadOnly = true;
            dgvAccounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAccounts.RowHeadersVisible = false;

            // أزرار التحكم
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);

            btnAdd = new Button();
            btnAdd.Text = "إضافة حساب";
            btnAdd.Size = new Size(120, 35);
            btnAdd.Location = new Point(10, 12);
            btnAdd.BackColor = Color.LightGreen;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button();
            btnEdit.Text = "تعديل الحساب";
            btnEdit.Size = new Size(120, 35);
            btnEdit.Location = new Point(140, 12);
            btnEdit.BackColor = Color.LightYellow;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button();
            btnDelete.Text = "حذف الحساب";
            btnDelete.Size = new Size(120, 35);
            btnDelete.Location = new Point(270, 12);
            btnDelete.BackColor = Color.LightCoral;
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new Button();
            btnRefresh.Text = "تحديث";
            btnRefresh.Size = new Size(120, 35);
            btnRefresh.Location = new Point(400, 12);
            btnRefresh.BackColor = Color.LightBlue;
            btnRefresh.Click += BtnRefresh_Click;

            Button btnClose = new Button();
            btnClose.Text = "إغلاق";
            btnClose.Size = new Size(80, 35);
            btnClose.Location = new Point(650, 12);
            btnClose.BackColor = Color.LightCoral;
            btnClose.Click += BtnClose_Click;

            buttonPanel.Controls.AddRange(new Control[] { 
                btnAdd, btnEdit, btnDelete, btnRefresh, btnClose 
            });

            this.Controls.AddRange(new Control[] { dgvAccounts, buttonPanel, topPanel });
        }

        private void LoadAccounts()
        {
            string query = @"
                SELECT 
                    Account_ID as 'رقم الحساب',
                    account_name as 'اسم الحساب',
                    account_type as 'نوع الحساب',
                    account_code as 'كود الحساب',
                    CASE WHEN is_active = 1 THEN 'نشط' ELSE 'غير نشط' END as 'الحالة'
                FROM Account
                ORDER BY account_type, account_code";

            DataTable accounts = databaseService.ExecuteQuery(query);
            dgvAccounts.DataSource = accounts;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            AccountForm accountForm = new AccountForm();
            if (accountForm.ShowDialog() == DialogResult.OK)
            {
                LoadAccounts();
                MessageBox.Show("تم إضافة الحساب بنجاح", "نجاح");
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvAccounts.SelectedRows.Count > 0)
            {
                int accountId = Convert.ToInt32(dgvAccounts.SelectedRows[0].Cells["رقم الحساب"].Value);
                AccountForm accountForm = new AccountForm(accountId);
                if (accountForm.ShowDialog() == DialogResult.OK)
                {
                    LoadAccounts();
                    MessageBox.Show("تم تحديث الحساب بنجاح", "نجاح");
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار حساب للتعديل", "تحذير");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAccounts.SelectedRows.Count > 0)
            {
                int accountId = Convert.ToInt32(dgvAccounts.SelectedRows[0].Cells["رقم الحساب"].Value);
                string accountName = dgvAccounts.SelectedRows[0].Cells["اسم الحساب"].Value.ToString();
                
                DialogResult result = MessageBox.Show(
                    $"هل أنت متأكد من حذف الحساب: {accountName}؟", 
                    "تأكيد الحذف", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    string query = "DELETE FROM Account WHERE Account_ID = @AccountID";
                    SqlParameter[] parameters = {
                        new SqlParameter("@AccountID", accountId)
                    };

                    int rowsAffected = databaseService.ExecuteNonQuery(query, parameters);
                    
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("تم حذف الحساب بنجاح", "نجاح");
                        LoadAccounts();
                    }
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار حساب للحذف", "تحذير");
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadAccounts();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}