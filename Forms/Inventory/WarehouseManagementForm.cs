using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;
using MIS_System_App.Models;

namespace MIS_System_App.Forms.Inventory
{
    public partial class WarehouseManagementForm : Form
    {
        private User currentUser;
        private DatabaseService databaseService;
        private DataGridView dgvWarehouses;
        private TextBox txtSearch;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;

        public WarehouseManagementForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            databaseService = new DatabaseService();
            LoadWarehouses();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "إدارة المستودعات";
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
            topPanel.BackColor = Color.LightBlue;
            topPanel.Padding = new Padding(10);

            Label titleLabel = new Label();
            titleLabel.Text = "إدارة المستودعات";
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
            dgvWarehouses = new DataGridView();
            dgvWarehouses.Dock = DockStyle.Fill;
            dgvWarehouses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvWarehouses.ReadOnly = true;
            dgvWarehouses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvWarehouses.RowHeadersVisible = false;

            // أزرار التحكم
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);

            btnAdd = new Button();
            btnAdd.Text = "إضافة مستودع";
            btnAdd.Size = new Size(120, 35);
            btnAdd.Location = new Point(10, 12);
            btnAdd.BackColor = Color.LightGreen;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button();
            btnEdit.Text = "تعديل المستودع";
            btnEdit.Size = new Size(120, 35);
            btnEdit.Location = new Point(140, 12);
            btnEdit.BackColor = Color.LightYellow;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button();
            btnDelete.Text = "حذف المستودع";
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

            this.Controls.AddRange(new Control[] { dgvWarehouses, buttonPanel, topPanel });
        }

        private void LoadWarehouses()
        {
            string query = @"
                SELECT 
                    Warehouse_ID as 'رقم المستودع',
                    warehouse_name as 'اسم المستودع',
                    geographic_location as 'الموقع الجغرافي',
                    capacity as 'السعة',
                    created_at as 'تاريخ الإنشاء'
                FROM Warehouse
                ORDER BY warehouse_name";

            DataTable warehouses = databaseService.ExecuteQuery(query);
            dgvWarehouses.DataSource = warehouses;
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dgvWarehouses.DataSource is DataTable dataTable)
            {
                string searchText = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    dataTable.DefaultView.RowFilter = 
                        $"`اسم المستودع` LIKE '%{searchText}%' OR " +
                        $"`الموقع الجغرافي` LIKE '%{searchText}%'";
                }
                else
                {
                    dataTable.DefaultView.RowFilter = "";
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            WarehouseForm warehouseForm = new WarehouseForm();
            if (warehouseForm.ShowDialog() == DialogResult.OK)
            {
                LoadWarehouses();
                MessageBox.Show("تم إضافة المستودع بنجاح", "نجاح");
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvWarehouses.SelectedRows.Count > 0)
            {
                int warehouseId = Convert.ToInt32(dgvWarehouses.SelectedRows[0].Cells["رقم المستودع"].Value);
                WarehouseForm warehouseForm = new WarehouseForm(warehouseId);
                if (warehouseForm.ShowDialog() == DialogResult.OK)
                {
                    LoadWarehouses();
                    MessageBox.Show("تم تحديث المستودع بنجاح", "نجاح");
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار مستودع للتعديل", "تحذير");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvWarehouses.SelectedRows.Count > 0)
            {
                int warehouseId = Convert.ToInt32(dgvWarehouses.SelectedRows[0].Cells["رقم المستودع"].Value);
                string warehouseName = dgvWarehouses.SelectedRows[0].Cells["اسم المستودع"].Value.ToString();
                
                DialogResult result = MessageBox.Show(
                    $"هل أنت متأكد من حذف المستودع: {warehouseName}؟", 
                    "تأكيد الحذف", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    string query = "DELETE FROM Warehouse WHERE Warehouse_ID = @WarehouseID";
                    SqlParameter[] parameters = {
                        new SqlParameter("@WarehouseID", warehouseId)
                    };

                    int rowsAffected = databaseService.ExecuteNonQuery(query, parameters);
                    
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("تم حذف المستودع بنجاح", "نجاح");
                        LoadWarehouses();
                    }
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار مستودع للحذف", "تحذير");
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadWarehouses();
            txtSearch.Clear();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}