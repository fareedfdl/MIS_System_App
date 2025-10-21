using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;
using MIS_System_App.Models;

namespace MIS_System_App.Forms.Reports
{
    public partial class ReportsForm : Form
    {
        private User currentUser;
        private DatabaseService databaseService;
        private DataGridView dgvReports;
        private ComboBox cmbReportType;
        private DateTimePicker dtpFrom, dtpTo;
        private Button btnGenerate, btnExport, btnPrint;

        public ReportsForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            databaseService = new DatabaseService();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "التقارير والإحصائيات";
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
            topPanel.Height = 120;
            topPanel.BackColor = Color.LightGoldenrodYellow;
            topPanel.Padding = new Padding(10);

            Label titleLabel = new Label();
            titleLabel.Text = "نظام التقارير والإحصائيات";
            titleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 40;

            // لوحة الفلاتر
            Panel filterPanel = new Panel();
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Height = 80;
            filterPanel.Padding = new Padding(10);

            Label lblReportType = new Label();
            lblReportType.Text = "نوع التقرير:";
            lblReportType.Location = new Point(900, 10);
            lblReportType.Size = new Size(80, 25);

            cmbReportType = new ComboBox();
            cmbReportType.Location = new Point(750, 10);
            cmbReportType.Size = new Size(140, 25);
            cmbReportType.Items.AddRange(new string[] {
                "تقرير المبيعات",
                "تقرير المنتجات",
                "تقرير العملاء", 
                "تقرير الموظفين",
                "تقرير المخزون",
                "تقرير المبيعات الشهري"
            });
            cmbReportType.SelectedIndex = 0;

            Label lblFrom = new Label();
            lblFrom.Text = "من تاريخ:";
            lblFrom.Location = new Point(650, 10);
            lblFrom.Size = new Size(70, 25);

            dtpFrom = new DateTimePicker();
            dtpFrom.Location = new Point(520, 10);
            dtpFrom.Size = new Size(120, 25);
            dtpFrom.Value = DateTime.Today.AddMonths(-1);

            Label lblTo = new Label();
            lblTo.Text = "إلى تاريخ:";
            lblTo.Location = new Point(380, 10);
            lblTo.Size = new Size(70, 25);

            dtpTo = new DateTimePicker();
            dtpTo.Location = new Point(250, 10);
            dtpTo.Size = new Size(120, 25);
            dtpTo.Value = DateTime.Today;

            btnGenerate = new Button();
            btnGenerate.Text = "توليد التقرير";
            btnGenerate.Location = new Point(120, 10);
            btnGenerate.Size = new Size(100, 25);
            btnGenerate.BackColor = Color.LightBlue;
            btnGenerate.Click += BtnGenerate_Click;

            btnExport = new Button();
            btnExport.Text = "تصدير Excel";
            btnExport.Location = new Point(10, 10);
            btnExport.Size = new Size(100, 25);
            btnExport.BackColor = Color.LightGreen;
            btnExport.Click += BtnExport_Click;

            filterPanel.Controls.AddRange(new Control[] {
                lblReportType, cmbReportType,
                lblFrom, dtpFrom,
                lblTo, dtpTo,
                btnGenerate, btnExport
            });

            topPanel.Controls.AddRange(new Control[] { titleLabel, filterPanel });

            // شبكة البيانات
            dgvReports = new DataGridView();
            dgvReports.Dock = DockStyle.Fill;
            dgvReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReports.ReadOnly = true;
            dgvReports.RowHeadersVisible = false;

            // لوحة الإحصائيات
            Panel statsPanel = new Panel();
            statsPanel.Dock = DockStyle.Bottom;
            statsPanel.Height = 80;
            statsPanel.BackColor = Color.LightGray;
            statsPanel.Padding = new Padding(10);

            CreateStatistics(statsPanel);

            // أزرار التحكم
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);

            btnPrint = new Button();
            btnPrint.Text = "طباعة التقرير";
            btnPrint.Size = new Size(120, 35);
            btnPrint.Location = new Point(10, 12);
            btnPrint.BackColor = Color.LightYellow;
            btnPrint.Click += BtnPrint_Click;

            Button btnClose = new Button();
            btnClose.Text = "إغلاق";
            btnClose.Size = new Size(80, 35);
            btnClose.Location = new Point(1050, 12);
            btnClose.BackColor = Color.LightCoral;
            btnClose.Click += BtnClose_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnPrint, btnClose });

            this.Controls.AddRange(new Control[] { dgvReports, statsPanel, buttonPanel, topPanel });
        }

        private void CreateStatistics(Panel panel)
        {
            Label lblStats = new Label();
            lblStats.Text = "الإحصائيات: جاري التحميل...";
            lblStats.Font = new Font("Arial", 11, FontStyle.Bold);
            lblStats.ForeColor = Color.DarkBlue;
            lblStats.TextAlign = ContentAlignment.MiddleCenter;
            lblStats.Dock = DockStyle.Fill;
            panel.Controls.Add(lblStats);
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            string reportType = cmbReportType.SelectedItem.ToString();
            DateTime fromDate = dtpFrom.Value;
            DateTime toDate = dtpTo.Value;

            try
            {
                DataTable reportData = GenerateReport(reportType, fromDate, toDate);
                dgvReports.DataSource = reportData;
                
                UpdateStatistics(reportType, reportData);
                
                MessageBox.Show($"تم توليد تقرير {reportType} بنجاح", "نجاح");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في توليد التقرير: {ex.Message}", "خطأ");
            }
        }

        private DataTable GenerateReport(string reportType, DateTime fromDate, DateTime toDate)
        {
            string query = "";
            
            switch (reportType)
            {
                case "تقرير المبيعات":
                    query = @"
                        SELECT 
                            so.Order_ID as 'رقم الطلب',
                            c.customer_name as 'اسم العميل',
                            so.order_date as 'تاريخ الطلب',
                            so.total_amount as 'المبلغ الإجمالي',
                            so.status as 'الحالة'
                        FROM Sales_Order so
                        INNER JOIN Customer c ON so.Customer_ID = c.Customer_ID
                        WHERE so.order_date BETWEEN @FromDate AND @ToDate
                        ORDER BY so.order_date DESC";
                    break;

                case "تقرير المنتجات":
                    query = @"
                        SELECT 
                            Product_ID as 'رقم المنتج',
                            product_name as 'اسم المنتج',
                            available_quantity as 'الكمية المتاحة',
                            cost_price as 'سعر التكلفة',
                            selling_price as 'سعر البيع',
                            (selling_price - cost_price) as 'هامش الربح'
                        FROM Product
                        WHERE is_active = 1
                        ORDER BY product_name";
                    break;

                case "تقرير العملاء":
                    query = @"
                        SELECT 
                            customer_name as 'اسم العميل',
                            customer_type as 'نوع العميل',
                            phone_number as 'الهاتف',
                            email as 'البريد الإلكتروني',
                            preferred_currency as 'العملة',
                            created_at as 'تاريخ التسجيل'
                        FROM Customer
                        ORDER BY customer_name";
                    break;

                case "تقرير الموظفين":
                    query = @"
                        SELECT 
                            e.full_name as 'اسم الموظف',
                            d.department_name as 'القسم',
                            e.hire_date as 'تاريخ التعيين',
                            e.base_salary as 'الراتب الأساسي',
                            e.phone_number as 'الهاتف',
                            e.email as 'البريد الإلكتروني'
                        FROM Employee e
                        LEFT JOIN Department d ON e.Dept_ID = d.Dept_ID
                        ORDER BY e.full_name";
                    break;

                case "تقرير المخزون":
                    query = @"
                        SELECT 
                            p.product_name as 'اسم المنتج',
                            p.available_quantity as 'الكمية المتاحة',
                            p.reorder_level as 'حد إعادة الطلب',
                            p.unit_of_measure as 'الوحدة',
                            CASE WHEN p.available_quantity <= p.reorder_level THEN 'نعم' ELSE 'لا' END as 'يحتاج إعادة طلب'
                        FROM Product p
                        WHERE p.is_active = 1
                        ORDER BY p.available_quantity ASC";
                    break;

                case "تقرير المبيعات الشهري":
                    query = @"
                        SELECT 
                            FORMAT(so.order_date, 'yyyy-MM') as 'الشهر',
                            COUNT(*) as 'عدد الطلبات',
                            SUM(so.total_amount) as 'إجمالي المبيعات',
                            AVG(so.total_amount) as 'متوسط قيمة الطلب'
                        FROM Sales_Order so
                        WHERE so.order_date BETWEEN @FromDate AND @ToDate
                        GROUP BY FORMAT(so.order_date, 'yyyy-MM')
                        ORDER BY 'الشهر' DESC";
                    break;
            }

            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };

            return databaseService.ExecuteQuery(query, parameters);
        }

        private void UpdateStatistics(string reportType, DataTable data)
        {
            // تحديث الإحصائيات حسب نوع التقرير
            string stats = $"إحصائيات {reportType}: ";
            
            if (data.Rows.Count > 0)
            {
                switch (reportType)
                {
                    case "تقرير المبيعات":
                        decimal totalSales = 0;
                        foreach (DataRow row in data.Rows)
                        {
                            totalSales += Convert.ToDecimal(row["المبلغ الإجمالي"]);
                        }
                        stats += $"{data.Rows.Count} طلب - إجمالي المبيعات: {totalSales:C0}";
                        break;

                    case "تقرير المنتجات":
                        stats += $"{data.Rows.Count} منتج نشط";
                        break;

                    case "تقرير العملاء":
                        stats += $"{data.Rows.Count} عميل مسجل";
                        break;

                    case "تقرير الموظفين":
                        stats += $"{data.Rows.Count} موظف";
                        break;

                    default:
                        stats += $"{data.Rows.Count} سجل";
                        break;
                }
            }
            else
            {
                stats += "لا توجد بيانات";
            }

            // تحديث النص في لوحة الإحصائيات
            if (this.Controls.Find("panelStats", true).Length > 0)
            {
                Panel statsPanel = (Panel)this.Controls.Find("panelStats", true)[0];
                if (statsPanel.Controls.Count > 0)
                {
                    statsPanel.Controls[0].Text = stats;
                }
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (dgvReports.DataSource != null)
            {
                try
                {
                    // تصدير البيانات لـ Excel (تنفيذ مبسط)
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.Filter = "Excel Files|*.xlsx";
                    saveDialog.FileName = $"تقرير_{cmbReportType.SelectedItem}_{DateTime.Now:yyyyMMdd}.xlsx";
                    
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show($"سيتم تصدير التقرير إلى: {saveDialog.FileName}", "تصدير Excel");
                        // هنا يمكن إضافة كود التصدير الفعلي لـ Excel
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطأ في التصدير: {ex.Message}", "خطأ");
                }
            }
            else
            {
                MessageBox.Show("لا توجد بيانات للتصدير", "تحذير");
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (dgvReports.DataSource != null)
            {
                MessageBox.Show("سيتم فتح نافذة الطباعة", "طباعة التقرير");
                // هنا يمكن إضافة كود الطباعة
            }
            else
            {
                MessageBox.Show("لا توجد بيانات للطباعة", "تحذير");
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
