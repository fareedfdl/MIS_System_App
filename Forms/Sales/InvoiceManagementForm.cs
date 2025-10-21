using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;
using MIS_System_App.Models;

namespace MIS_System_App.Forms.Sales
{
    public partial class InvoiceManagementForm : Form
    {
        private User currentUser;
        private DatabaseService databaseService;
        private SalesService salesService;
        private DataGridView dgvInvoices;
        private TextBox txtSearch;
        private Button btnCreate, btnView, btnPrint, btnRefresh;

        public InvoiceManagementForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            databaseService = new DatabaseService();
            salesService = new SalesService();
            LoadInvoices();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "إدارة الفواتير";
            this.Size = new Size(1200, 600);
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
            titleLabel.Text = "إدارة الفواتير";
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
            dgvInvoices = new DataGridView();
            dgvInvoices.Dock = DockStyle.Fill;
            dgvInvoices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInvoices.ReadOnly = true;
            dgvInvoices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInvoices.RowHeadersVisible = false;

            // أزرار التحكم
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);

            btnCreate = new Button();
            btnCreate.Text = "إنشاء فاتورة";
            btnCreate.Size = new Size(120, 35);
            btnCreate.Location = new Point(10, 12);
            btnCreate.BackColor = Color.LightGreen;
            btnCreate.Click += BtnCreate_Click;

            btnView = new Button();
            btnView.Text = "عرض الفاتورة";
            btnView.Size = new Size(120, 35);
            btnView.Location = new Point(140, 12);
            btnView.BackColor = Color.LightBlue;
            btnView.Click += BtnView_Click;

            btnPrint = new Button();
            btnPrint.Text = "طباعة الفاتورة";
            btnPrint.Size = new Size(120, 35);
            btnPrint.Location = new Point(270, 12);
            btnPrint.BackColor = Color.LightYellow;
            btnPrint.Click += BtnPrint_Click;

            btnRefresh = new Button();
            btnRefresh.Text = "تحديث";
            btnRefresh.Size = new Size(120, 35);
            btnRefresh.Location = new Point(400, 12);
            btnRefresh.BackColor = Color.LightGray;
            btnRefresh.Click += BtnRefresh_Click;

            Button btnClose = new Button();
            btnClose.Text = "إغلاق";
            btnClose.Size = new Size(80, 35);
            btnClose.Location = new Point(1050, 12);
            btnClose.BackColor = Color.LightCoral;
            btnClose.Click += BtnClose_Click;

            buttonPanel.Controls.AddRange(new Control[] { 
                btnCreate, btnView, btnPrint, btnRefresh, btnClose 
            });

            this.Controls.AddRange(new Control[] { dgvInvoices, buttonPanel, topPanel });
        }

        private void LoadInvoices()
        {
            string query = @"
                SELECT 
                    i.Invoice_ID as 'رقم الفاتورة',
                    c.customer_name as 'اسم العميل',
                    i.issue_date as 'تاريخ الإصدار',
                    i.due_date as 'تاريخ الاستحقاق',
                    i.total_amount as 'المبلغ الإجمالي',
                    i.tax_amount as 'ضريبة القيمة المضافة',
                    i.status as 'الحالة',
                    i.created_at as 'تاريخ الإنشاء'
                FROM Invoice i
                INNER JOIN Customer c ON i.Customer_ID = c.Customer_ID
                ORDER BY i.issue_date DESC";

            DataTable invoices = databaseService.ExecuteQuery(query);
            dgvInvoices.DataSource = invoices;

            // تنسيق الأعمدة
            if (dgvInvoices.Columns.Count > 0)
            {
                dgvInvoices.Columns["المبلغ الإجمالي"].DefaultCellStyle.Format = "C0";
                dgvInvoices.Columns["ضريبة القيمة المضافة"].DefaultCellStyle.Format = "C0";
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dgvInvoices.DataSource is DataTable dataTable)
            {
                string searchText = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    dataTable.DefaultView.RowFilter = 
                        $"`اسم العميل` LIKE '%{searchText}%' OR " +
                        $"`الحالة` LIKE '%{searchText}%' OR " +
                        $"`رقم الفاتورة` LIKE '%{searchText}%'";
                }
                else
                {
                    dataTable.DefaultView.RowFilter = "";
                }
            }
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            CreateInvoiceForm createForm = new CreateInvoiceForm();
            if (createForm.ShowDialog() == DialogResult.OK)
            {
                LoadInvoices();
                MessageBox.Show("تم إنشاء الفاتورة بنجاح", "نجاح");
            }
        }

        private void BtnView_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.SelectedRows.Count > 0)
            {
                int invoiceId = Convert.ToInt32(dgvInvoices.SelectedRows[0].Cells["رقم الفاتورة"].Value);
                ViewInvoiceForm viewForm = new ViewInvoiceForm(invoiceId);
                viewForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("يرجى اختيار فاتورة للعرض", "تحذير");
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.SelectedRows.Count > 0)
            {
                int invoiceId = Convert.ToInt32(dgvInvoices.SelectedRows[0].Cells["رقم الفاتورة"].Value);
                MessageBox.Show($"سيتم طباعة الفاتورة رقم: {invoiceId}", "طباعة الفاتورة");
                // كود الطباعة الفعلي
            }
            else
            {
                MessageBox.Show("يرجى اختيار فاتورة للطباعة", "تحذير");
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadInvoices();
            txtSearch.Clear();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}