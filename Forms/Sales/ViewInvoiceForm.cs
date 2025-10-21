using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.Sales
{
    public partial class ViewInvoiceForm : Form
    {
        private DatabaseService databaseService;
        private int invoiceId;
        private Label lblInvoiceDetails;
        private DataGridView dgvInvoiceItems;
        private Button btnPrint, btnClose;

        public ViewInvoiceForm(int invId)
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            invoiceId = invId;
            LoadInvoiceData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "عرض الفاتورة";
            this.Size = new Size(800, 600);
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
            topPanel.Height = 100;
            topPanel.BackColor = Color.LightYellow;
            topPanel.Padding = new Padding(10);

            lblInvoiceDetails = new Label();
            lblInvoiceDetails.Text = "جاري تحميل بيانات الفاتورة...";
            lblInvoiceDetails.Font = new Font("Arial", 12, FontStyle.Bold);
            lblInvoiceDetails.TextAlign = ContentAlignment.MiddleCenter;
            lblInvoiceDetails.Dock = DockStyle.Fill;
            topPanel.Controls.Add(lblInvoiceDetails);

            // شبكة عناصر الفاتورة
            dgvInvoiceItems = new DataGridView();
            dgvInvoiceItems.Dock = DockStyle.Fill;
            dgvInvoiceItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInvoiceItems.ReadOnly = true;
            dgvInvoiceItems.RowHeadersVisible = false;

            // أزرار التحكم
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);

            btnPrint = new Button();
            btnPrint.Text = "طباعة الفاتورة";
            btnPrint.Size = new Size(120, 35);
            btnPrint.Location = new Point(10, 12);
            btnPrint.BackColor = Color.LightYellow;
            btnPrint.Click += BtnPrint_Click;

            btnClose = new Button();
            btnClose.Text = "إغلاق";
            btnClose.Size = new Size(80, 35);
            btnClose.Location = new Point(650, 12);
            btnClose.BackColor = Color.LightCoral;
            btnClose.Click += BtnClose_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnPrint, btnClose });

            this.Controls.AddRange(new Control[] { dgvInvoiceItems, buttonPanel, topPanel });
        }

        private void LoadInvoiceData()
        {
            try
            {
                // تحميل بيانات الفاتورة
                string invoiceQuery = @"
                    SELECT 
                        i.Invoice_ID,
                        c.customer_name,
                        i.issue_date,
                        i.due_date,
                        i.total_amount,
                        i.tax_amount,
                        i.status,
                        (i.total_amount - i.tax_amount) as sub_total
                    FROM Invoice i
                    INNER JOIN Customer c ON i.Customer_ID = c.Customer_ID
                    WHERE i.Invoice_ID = @InvoiceID";

                SqlParameter[] parameters = {
                    new SqlParameter("@InvoiceID", invoiceId)
                };

                DataTable invoiceData = databaseService.ExecuteQuery(invoiceQuery, parameters);
                
                if (invoiceData.Rows.Count > 0)
                {
                    DataRow row = invoiceData.Rows[0];
                    string invoiceText = $@"
                    الفاتورة رقم: {row["Invoice_ID"]} | العميل: {row["customer_name"]}
                    تاريخ الإصدار: {Convert.ToDateTime(row["issue_date"]):yyyy-MM-dd} | تاريخ الاستحقاق: {Convert.ToDateTime(row["due_date"]):yyyy-MM-dd}
                    الحالة: {row["status"]} | الإجمالي: {Convert.ToDecimal(row["total_amount"]):C0}";

                    lblInvoiceDetails.Text = invoiceText;
                }

                // تحميل عناصر الفاتورة (من طلب البيع)
                string itemsQuery = @"
                    SELECT 
                        p.product_name as 'المنتج',
                        oi.quantity as 'الكمية',
                        oi.unit_price as 'سعر الوحدة',
                        oi.line_total as 'المجموع'
                    FROM Order_Item oi
                    INNER JOIN Product p ON oi.Product_ID = p.Product_ID
                    WHERE oi.Order_ID = (
                        SELECT TOP 1 Order_ID FROM Sales_Order 
                        WHERE Customer_ID = (SELECT Customer_ID FROM Invoice WHERE Invoice_ID = @InvoiceID)
                        ORDER BY Order_ID DESC
                    )";

                DataTable itemsData = databaseService.ExecuteQuery(itemsQuery, parameters);
                dgvInvoiceItems.DataSource = itemsData;

                // تنسيق الأعمدة
                if (dgvInvoiceItems.Columns.Count > 0)
                {
                    dgvInvoiceItems.Columns["سعر الوحدة"].DefaultCellStyle.Format = "C0";
                    dgvInvoiceItems.Columns["المجموع"].DefaultCellStyle.Format = "C0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل بيانات الفاتورة: {ex.Message}", "خطأ");
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"سيتم طباعة الفاتورة رقم: {invoiceId}", "طباعة الفاتورة");
            // كود الطباعة الفعلي
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}