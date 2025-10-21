using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.Inventory
{
    public partial class SelectProductForm : Form
    {
        private DatabaseService databaseService;
        private DataGridView dgvProducts;
        private TextBox txtSearch;
        private Button btnSelect, btnCancel;
        
        public int SelectedProductId { get; private set; }
        public string SelectedProductName { get; private set; }
        public decimal SelectedProductPrice { get; private set; }

        public SelectProductForm()
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            LoadProducts();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "اختر منتج";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            CreateControls();
            this.ResumeLayout(false);
        }

        private void CreateControls()
        {
            // عنوان النموذج
            Label titleLabel = new Label();
            titleLabel.Text = "اختر منتج من القائمة";
            titleLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 40;
            titleLabel.BackColor = Color.LightBlue;

            // شريط البحث
            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Height = 40;
            searchPanel.Padding = new Padding(10);

            Label lblSearch = new Label();
            lblSearch.Text = "بحث:";
            lblSearch.Location = new Point(10, 10);
            lblSearch.Size = new Size(40, 20);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(60, 8);
            txtSearch.Size = new Size(300, 25);
            txtSearch.TextChanged += TxtSearch_TextChanged;

            searchPanel.Controls.AddRange(new Control[] { lblSearch, txtSearch });

            // شبكة المنتجات
            dgvProducts = new DataGridView();
            dgvProducts.Dock = DockStyle.Fill;
            dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProducts.ReadOnly = true;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.RowHeadersVisible = false;
            dgvProducts.AllowUserToAddRows = false;

            // أزرار التحكم
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 50;
            buttonPanel.Padding = new Padding(10);

            btnSelect = new Button();
            btnSelect.Text = "اختر المنتج";
            btnSelect.Size = new Size(120, 30);
            btnSelect.Location = new Point(350, 10);
            btnSelect.BackColor = Color.LightGreen;
            btnSelect.Click += BtnSelect_Click;

            btnCancel = new Button();
            btnCancel.Text = "إلغاء";
            btnCancel.Size = new Size(120, 30);
            btnCancel.Location = new Point(220, 10);
            btnCancel.BackColor = Color.LightCoral;
            btnCancel.Click += BtnCancel_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnSelect, btnCancel });

            this.Controls.AddRange(new Control[] { dgvProducts, buttonPanel, searchPanel, titleLabel });
        }

        private void LoadProducts()
        {
            string query = @"
                SELECT 
                    Product_ID as 'رقم المنتج',
                    product_name as 'اسم المنتج',
                    available_quantity as 'الكمية المتاحة',
                    unit_of_measure as 'الوحدة',
                    selling_price as 'سعر البيع',
                    CASE WHEN is_active = 1 THEN 'نشط' ELSE 'غير نشط' END as 'الحالة'
                FROM Product
                WHERE is_active = 1 AND available_quantity > 0
                ORDER BY product_name";

            DataTable products = databaseService.ExecuteQuery(query);
            dgvProducts.DataSource = products;

            // تنسيق الأعمدة
            if (dgvProducts.Columns.Count > 0)
            {
                dgvProducts.Columns["سعر البيع"].DefaultCellStyle.Format = "C0";
                dgvProducts.Columns["رقم المنتج"].Visible = false;
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dgvProducts.DataSource is DataTable dataTable)
            {
                string searchText = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    dataTable.DefaultView.RowFilter = $"`اسم المنتج` LIKE '%{searchText}%'";
                }
                else
                {
                    dataTable.DefaultView.RowFilter = "";
                }
            }
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvProducts.SelectedRows[0];
                SelectedProductId = Convert.ToInt32(selectedRow.Cells["رقم المنتج"].Value);
                SelectedProductName = selectedRow.Cells["اسم المنتج"].Value.ToString();
                SelectedProductPrice = Convert.ToDecimal(selectedRow.Cells["سعر البيع"].Value);
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("يرجى اختيار منتج من القائمة", "تحذير");
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
