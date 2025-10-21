using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.Accounting
{
    public partial class FinancialReportsForm : Form
    {
        private AccountingService accountingService;
        private DataGridView dgvReport;
        private ComboBox cmbReportType;
        private DateTimePicker dtpFrom, dtpTo;
        private Button btnGenerate, btnExport, btnPrint;

        public FinancialReportsForm()
        {
            InitializeComponent();
            accountingService = new AccountingService();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "التقارير المالية";
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
            topPanel.Height = 100;
            topPanel.BackColor = Color.LightGoldenrodYellow;
            topPanel.Padding = new Padding(10);

            Label titleLabel = new Label();
            titleLabel.Text = "التقارير المالية والمحاسبية";
            titleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 40;

            // لوحة الفلاتر
            Panel filterPanel = new Panel();
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Height = 60;
            filterPanel.Padding = new Padding(10);

            Label lblReportType = new Label();
            lblReportType.Text = "نوع التقرير:";
            lblReportType.Location = new Point(800, 15);
            lblReportType.Size = new Size(80, 25);

            cmbReportType = new ComboBox();
            cmbReportType.Location = new Point(650, 15);
            cmbReportType.Size = new Size(140, 25);
            cmbReportType.Items.AddRange(new string[] {
                "الميزانية العمومية",
                "قائمة الدخل",
                "حركة الحسابات"
            });
            cmbReportType.SelectedIndex = 0;

            Label lblFrom = new Label();
            lblFrom.Text = "من تاريخ:";
            lblFrom.Location = new Point(550, 15);
            lblFrom.Size = new Size(70, 25);

            dtpFrom = new DateTimePicker();
            dtpFrom.Location = new Point(450, 15);
            dtpFrom.Size = new Size(90, 25);
            dtpFrom.Value = DateTime.Today.AddMonths(-1);

            Label lblTo = new Label();
            lblTo.Text = "إلى تاريخ:";
            lblTo.Location = new Point(350, 15);
            lblTo.Size = new Size(70, 25);

            dtpTo = new DateTimePicker();
            dtpTo.Location = new Point(250, 15);
            dtpTo.Size = new Size(90, 25);
            dtpTo.Value = DateTime.Today;

            btnGenerate = new Button();
            btnGenerate.Text = "توليد التقرير";
            btnGenerate.Location = new Point(120, 15);
            btnGenerate.Size = new Size(120, 25);
            btnGenerate.BackColor = Color.LightBlue;
            btnGenerate.Click += BtnGenerate_Click;

            filterPanel.Controls.AddRange(new Control[] {
                lblReportType, cmbReportType,
                lblFrom, dtpFrom,
                lblTo, dtpTo,
                btnGenerate
            });

            topPanel.Controls.AddRange(new Control[] { titleLabel, filterPanel });

            // شبكة البيانات
            dgvReport = new DataGridView();
            dgvReport.Dock = DockStyle.Fill;
            dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReport.ReadOnly = true;
            dgvReport.RowHeadersVisible = false;

            // أزرار التحكم
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);

            btnExport = new Button();
            btnExport.Text = "تصدير Excel";
            btnExport.Size = new Size(120, 35);
            btnExport.Location = new Point(10, 12);
            btnExport.BackColor = Color.LightGreen;
            btnExport.Click += BtnExport_Click;

            btnPrint = new Button();
            btnPrint.Text = "طباعة التقرير";
            btnPrint.Size = new Size(120, 35);
            btnPrint.Location = new Point(140, 12);
            btnPrint.BackColor = Color.LightYellow;
            btnPrint.Click += BtnPrint_Click;

            Button btnClose = new Button();
            btnClose.Text = "إغلاق";
            btnClose.Size = new Size(80, 35);
            btnClose.Location = new Point(850, 12);
            btnClose.BackColor = Color.LightCoral;
            btnClose.Click += BtnClose_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnExport, btnPrint, btnClose });

            this.Controls.AddRange(new Control[] { dgvReport, buttonPanel, topPanel });
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            string reportType = cmbReportType.SelectedItem.ToString();
            DateTime fromDate = dtpFrom.Value;
            DateTime toDate = dtpTo.Value;

            try
            {
                DataTable reportData = null;

                switch (reportType)
                {
                    case "الميزانية العمومية":
                        reportData = accountingService.GetBalanceSheet(toDate);
                        break;
                    case "قائمة الدخل":
                        reportData = accountingService.GetIncomeStatement(fromDate, toDate);
                        break;
                    case "حركة الحسابات":
                        reportData = GetAccountMovement(fromDate, toDate);
                        break;
                }

                if (reportData != null)
                {
                    dgvReport.DataSource = reportData;
                    
                    // تنسيق الأعمدة الرقمية
                    foreach (DataGridViewColumn column in dgvReport.Columns)
                    {
                        if (column.Name.Contains("مبلغ") || column.Name.Contains("رصيد") || 
                            column.Name.Contains("مدين") || column.Name.Contains("دائن"))
                        {
                            column.DefaultCellStyle.Format = "C0";
                        }
                    }
                    
                    MessageBox.Show($"تم توليد تقرير {reportType} بنجاح", "نجاح");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في توليد التقرير: {ex.Message}", "خطأ");
            }
        }

        private DataTable GetAccountMovement(DateTime fromDate, DateTime toDate)
        {
            string query = @"
                SELECT 
                    a.account_name as 'اسم الحساب',
                    (SELECT ISNULL(SUM(amount), 0) FROM [Transaction] t WHERE t.debit_account_id = a.Account_ID AND t.transaction_date BETWEEN @FromDate AND @ToDate) as 'إجمالي المدين',
                    (SELECT ISNULL(SUM(amount), 0) FROM [Transaction] t WHERE t.credit_account_id = a.Account_ID AND t.transaction_date BETWEEN @FromDate AND @ToDate) as 'إجمالي الدائن'
                FROM Account a
                WHERE a.is_active = 1
                ORDER BY a.account_name";

            return accountingService.GetAccountMovement(fromDate, toDate);
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (dgvReport.DataSource != null)
            {
                try
                {
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.Filter = "Excel Files|*.csv";
                    saveDialog.FileName = $"تقرير_مالي_{DateTime.Now:yyyyMMdd}.csv";
                    
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show($"سيتم تصدير التقرير إلى: {saveDialog.FileName}", "تصدير Excel");
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
            if (dgvReport.DataSource != null)
            {
                MessageBox.Show("سيتم فتح نافذة الطباعة", "طباعة التقرير");
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