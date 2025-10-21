
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;
using MIS_System_App.Models;

namespace MIS_System_App.Forms.Inventory
{
    public partial class ProductManagementForm : Form
    {
        private User currentUser;
        private DatabaseService databaseService;
        private DataGridView dgvProducts;
        private TextBox txtSearch;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;

        public ProductManagementForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            databaseService = new DatabaseService();
            LoadProducts();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "إدارة المنتجات";
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
            topPanel.BackColor = Color.LightGreen;
            topPanel.Padding = new Padding(10);

            Label titleLabel = new Label();
            titleLabel.Text = "إدارة المنتجات والمخزون";
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
            dgvProducts = new DataGridView();
            dgvProducts.Dock = DockStyle.Fill;
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProducts.ReadOnly = true;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.RowHeadersVisible = false;

            // أزرار التحكم
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);

            btnAdd = new Button();
            btnAdd.Text = "إضافة منتج";
            btnAdd.Size = new Size(120, 35);
            btnAdd.Location = new Point(10, 12);
            btnAdd.BackColor = Color.LightGreen;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button();
            btnEdit.Text = "تعديل المنتج";
            btnEdit.Size = new Size(120, 35);
            btnEdit.Location = new Point(140, 12);
            btnEdit.BackColor = Color.LightYellow;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button();
            btnDelete.Text = "حذف المنتج";
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

            this.Controls.AddRange(new Control[] { dgvProducts, buttonPanel, topPanel });
        }

        private void LoadProducts()
        {
            string query = @"
                SELECT 
                    Product_ID as 'رقم المنتج',
                    product_name as 'اسم المنتج',
                    product_description as 'الوصف',
                    available_quantity as 'الكمية المتاحة',
                    unit_of_measure as 'الوحدة',
                    cost_price as 'سعر التكلفة',
                    selling_price as 'سعر البيع',
                    reorder_level as 'حد إعادة الطلب',
                    CASE WHEN is_active = 1 THEN 'نشط' ELSE 'غير نشط' END as 'الحالة'
                FROM Product
                ORDER BY product_name";

            DataTable products = databaseService.ExecuteQuery(query);
            dgvProducts.DataSource = products;

            // تنسيق الأعمدة
            if (dgvProducts.Columns.Count > 0)
            {
                dgvProducts.Columns["سعر التكلفة"].DefaultCellStyle.Format = "C0";
                dgvProducts.Columns["سعر البيع"].DefaultCellStyle.Format = "C0";
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dgvProducts.DataSource is DataTable dataTable)
            {
                string searchText = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    dataTable.DefaultView.RowFilter = 
                        $"`اسم المنتج` LIKE '%{searchText}%' OR " +
                        $"`الوصف` LIKE '%{searchText}%'";
                }
                else
                {
                    dataTable.DefaultView.RowFilter = "";
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ProductForm productForm = new ProductForm();
            if (productForm.ShowDialog() == DialogResult.OK)
            {
                LoadProducts();
                MessageBox.Show("تم إضافة المنتج بنجاح", "نجاح");
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                int productId = Convert.ToInt32(dgvProducts.SelectedRows[0].Cells["رقم المنتج"].Value);
                ProductForm productForm = new ProductForm(productId);
                if (productForm.ShowDialog() == DialogResult.OK)
                {
                    LoadProducts();
                    MessageBox.Show("تم تحديث المنتج بنجاح", "نجاح");
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار منتج للتعديل", "تحذير");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                int productId = Convert.ToInt32(dgvProducts.SelectedRows[0].Cells["رقم المنتج"].Value);
                string productName = dgvProducts.SelectedRows[0].Cells["اسم المنتج"].Value.ToString();
                
                DialogResult result = MessageBox.Show(
                    $"هل أنت متأكد من حذف المنتج: {productName}؟", 
                    "تأكيد الحذف", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    string query = "DELETE FROM Product WHERE Product_ID = @ProductID";
                    SqlParameter[] parameters = {
                        new SqlParameter("@ProductID", productId)
                    };

                    int rowsAffected = databaseService.ExecuteNonQuery(query, parameters);
                    
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("تم حذف المنتج بنجاح", "نجاح");
                        LoadProducts();
                    }
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار منتج للحذف", "تحذير");
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadProducts();
            txtSearch.Clear();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
