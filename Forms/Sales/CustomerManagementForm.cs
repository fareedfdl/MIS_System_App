using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;
using MIS_System_App.Models;

namespace MIS_System_App.Forms.Sales
{
    public partial class CustomerManagementForm : Form
    {
        private User currentUser;
        private DatabaseService databaseService;
        private DataGridView dgvCustomers;
        private TextBox txtSearch;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;

        public CustomerManagementForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            databaseService = new DatabaseService();
            LoadCustomers();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "إدارة العملاء";
            this.Size = new Size(1200, 700);
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
            topPanel.BackColor = Color.LightBlue;
            topPanel.Padding = new Padding(10);

            Label titleLabel = new Label();
            titleLabel.Text = "إدارة العملاء";
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
            dgvCustomers = new DataGridView();
            dgvCustomers.Dock = DockStyle.Fill;
            dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCustomers.ReadOnly = true;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.RowHeadersVisible = false;

            // أزرار التحكم
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);

            btnAdd = new Button();
            btnAdd.Text = "إضافة عميل";
            btnAdd.Size = new Size(120, 35);
            btnAdd.Location = new Point(10, 12);
            btnAdd.BackColor = Color.LightGreen;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button();
            btnEdit.Text = "تعديل العميل";
            btnEdit.Size = new Size(120, 35);
            btnEdit.Location = new Point(140, 12);
            btnEdit.BackColor = Color.LightYellow;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button();
            btnDelete.Text = "حذف العميل";
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
            btnClose.Location = new Point(1050, 12);
            btnClose.BackColor = Color.LightCoral;
            btnClose.Click += BtnClose_Click;

            buttonPanel.Controls.AddRange(new Control[] { 
                btnAdd, btnEdit, btnDelete, btnRefresh, btnClose 
            });

            this.Controls.AddRange(new Control[] { dgvCustomers, buttonPanel, topPanel });
        }

        private void LoadCustomers()
        {
            string query = @"
                SELECT 
                    Customer_ID as 'رقم العميل',
                    customer_name as 'اسم العميل',
                    customer_address as 'العنوان',
                    phone_number as 'الهاتف',
                    email as 'البريد الإلكتروني',
                    preferred_currency as 'العملة',
                    customer_type as 'نوع العميل',
                    created_at as 'تاريخ التسجيل'
                FROM Customer
                ORDER BY customer_name";

            DataTable customers = databaseService.ExecuteQuery(query);
            dgvCustomers.DataSource = customers;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dgvCustomers.DataSource is DataTable dataTable)
            {
                string searchText = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    dataTable.DefaultView.RowFilter = 
                        $"`اسم العميل` LIKE '%{searchText}%' OR " +
                        $"`الهاتف` LIKE '%{searchText}%' OR " +
                        $"`البريد الإلكتروني` LIKE '%{searchText}%'";
                }
                else
                {
                    dataTable.DefaultView.RowFilter = "";
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            CustomerForm customerForm = new CustomerForm();
            if (customerForm.ShowDialog() == DialogResult.OK)
            {
                LoadCustomers();
                MessageBox.Show("تم إضافة العميل بنجاح", "نجاح");
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                int customerId = Convert.ToInt32(dgvCustomers.SelectedRows[0].Cells["رقم العميل"].Value);
                CustomerForm customerForm = new CustomerForm(customerId);
                if (customerForm.ShowDialog() == DialogResult.OK)
                {
                    LoadCustomers();
                    MessageBox.Show("تم تحديث العميل بنجاح", "نجاح");
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار عميل للتعديل", "تحذير");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                int customerId = Convert.ToInt32(dgvCustomers.SelectedRows[0].Cells["رقم العميل"].Value);
                string customerName = dgvCustomers.SelectedRows[0].Cells["اسم العميل"].Value.ToString();
                
                DialogResult result = MessageBox.Show(
                    $"هل أنت متأكد من حذف العميل: {customerName}؟", 
                    "تأكيد الحذف", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    string query = "DELETE FROM Customer WHERE Customer_ID = @CustomerID";
                    SqlParameter[] parameters = {
                        new SqlParameter("@CustomerID", customerId)
                    };

                    int rowsAffected = databaseService.ExecuteNonQuery(query, parameters);
                    
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("تم حذف العميل بنجاح", "نجاح");
                        LoadCustomers();
                    }
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار عميل للحذف", "تحذير");
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadCustomers();
            txtSearch.Clear();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
