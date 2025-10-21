using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;
using MIS_System_App.Models;

namespace MIS_System_App.Forms.Inventory
{
    public partial class SupplierManagementForm : Form
    {
        private User currentUser;
        private DatabaseService databaseService;
        private DataGridView dgvSuppliers;
        private TextBox txtSearch;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;

        public SupplierManagementForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            databaseService = new DatabaseService();
            LoadSuppliers();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "إدارة الموردين";
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
            topPanel.BackColor = Color.LightGreen;
            topPanel.Padding = new Padding(10);

            Label titleLabel = new Label();
            titleLabel.Text = "إدارة الموردين";
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
            dgvSuppliers = new DataGridView();
            dgvSuppliers.Dock = DockStyle.Fill;
            dgvSuppliers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSuppliers.ReadOnly = true;
            dgvSuppliers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSuppliers.RowHeadersVisible = false;

            // أزرار التحكم
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);

            btnAdd = new Button();
            btnAdd.Text = "إضافة مورد";
            btnAdd.Size = new Size(120, 35);
            btnAdd.Location = new Point(10, 12);
            btnAdd.BackColor = Color.LightGreen;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button();
            btnEdit.Text = "تعديل المورد";
            btnEdit.Size = new Size(120, 35);
            btnEdit.Location = new Point(140, 12);
            btnEdit.BackColor = Color.LightYellow;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button();
            btnDelete.Text = "حذف المورد";
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
            btnClose.Location = new Point(850, 12);
            btnClose.BackColor = Color.LightCoral;
            btnClose.Click += BtnClose_Click;

            buttonPanel.Controls.AddRange(new Control[] { 
                btnAdd, btnEdit, btnDelete, btnRefresh, btnClose 
            });

            this.Controls.AddRange(new Control[] { dgvSuppliers, buttonPanel, topPanel });
        }

        private void LoadSuppliers()
        {
            string query = @"
                SELECT 
                    Supplier_ID as 'رقم المورد',
                    supplier_name as 'اسم المورد',
                    contact_info as 'معلومات الاتصال',
                    phone_number as 'الهاتف',
                    email as 'البريد الإلكتروني',
                    address as 'العنوان',
                    created_at as 'تاريخ التسجيل'
                FROM Supplier
                ORDER BY supplier_name";

            DataTable suppliers = databaseService.ExecuteQuery(query);
            dgvSuppliers.DataSource = suppliers;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dgvSuppliers.DataSource is DataTable dataTable)
            {
                string searchText = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    dataTable.DefaultView.RowFilter = 
                        $"`اسم المورد` LIKE '%{searchText}%' OR " +
                        $"`معلومات الاتصال` LIKE '%{searchText}%' OR " +
                        $"`الهاتف` LIKE '%{searchText}%'";
                }
                else
                {
                    dataTable.DefaultView.RowFilter = "";
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            SupplierForm supplierForm = new SupplierForm();
            if (supplierForm.ShowDialog() == DialogResult.OK)
            {
                LoadSuppliers();
                MessageBox.Show("تم إضافة المورد بنجاح", "نجاح");
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvSuppliers.SelectedRows.Count > 0)
            {
                int supplierId = Convert.ToInt32(dgvSuppliers.SelectedRows[0].Cells["رقم المورد"].Value);
                SupplierForm supplierForm = new SupplierForm(supplierId);
                if (supplierForm.ShowDialog() == DialogResult.OK)
                {
                    LoadSuppliers();
                    MessageBox.Show("تم تحديث المورد بنجاح", "نجاح");
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار مورد للتعديل", "تحذير");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvSuppliers.SelectedRows.Count > 0)
            {
                int supplierId = Convert.ToInt32(dgvSuppliers.SelectedRows[0].Cells["رقم المورد"].Value);
                string supplierName = dgvSuppliers.SelectedRows[0].Cells["اسم المورد"].Value.ToString();
                
                DialogResult result = MessageBox.Show(
                    $"هل أنت متأكد من حذف المورد: {supplierName}؟", 
                    "تأكيد الحذف", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    string query = "DELETE FROM Supplier WHERE Supplier_ID = @SupplierID";
                    SqlParameter[] parameters = {
                        new SqlParameter("@SupplierID", supplierId)
                    };

                    int rowsAffected = databaseService.ExecuteNonQuery(query, parameters);
                    
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("تم حذف المورد بنجاح", "نجاح");
                        LoadSuppliers();
                    }
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار مورد للحذف", "تحذير");
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadSuppliers();
            txtSearch.Clear();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}