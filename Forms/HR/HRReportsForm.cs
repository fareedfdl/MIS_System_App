using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MIS_System_App.Services;
using MIS_System_App.Models;
using System.Data.SqlClient;

namespace MIS_System_App.Forms.HR
{
    public partial class HRReportsForm : Form
    {
        private User currentUser;
        private DatabaseService databaseService;
        private HRService hrService;
        private DataGridView dgvReports;
        private ComboBox cmbReportType;
        private DateTimePicker dtpFrom, dtpTo;
        private Button btnGenerate, btnExport, btnPrint;

        public HRReportsForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            databaseService = new DatabaseService();
            hrService = new HRService();
            LoadEmployeeStatistics();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "تقارير الموارد البشرية";
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
            topPanel.BackColor = Color.LightBlue;
            topPanel.Padding = new Padding(10);

            Label titleLabel = new Label();
            titleLabel.Text = "تقارير الموارد البشرية";
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
                "إحصائيات الموظفين",
                "توزيع الموظفين بالأقسام",
                "سجل الرواتب",
                "المكافآت والخصومات"
            });
            cmbReportType.SelectedIndex = 0;
            cmbReportType.SelectedIndexChanged += CmbReportType_SelectedIndexChanged;

            Label lblFrom = new Label();
            lblFrom.Text = "من تاريخ:";
            lblFrom.Location = new Point(550, 15);
            lblFrom.Size = new Size(70, 25);

            dtpFrom = new DateTimePicker();
            dtpFrom.Location = new Point(450, 15);
            dtpFrom.Size = new Size(90, 25);
            dtpFrom.Value = DateTime.Today.AddMonths(-3);

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
            dgvReports = new DataGridView();
            dgvReports.Dock = DockStyle.Fill;
            dgvReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReports.ReadOnly = true;
            dgvReports.RowHeadersVisible = false;

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

            this.Controls.AddRange(new Control[] { dgvReports, buttonPanel, topPanel });
        }

        private void LoadEmployeeStatistics()
        {
            DataTable stats = hrService.GetEmployeeStatistics();
            dgvReports.DataSource = stats;

            // تنسيق الأعمدة
            if (dgvReports.Columns.Count > 0)
            {
                dgvReports.Columns["متوسط الراتب"].DefaultCellStyle.Format = "C0";
                dgvReports.Columns["إجمالي الرواتب"].DefaultCellStyle.Format = "C0";
            }
        }

        private void CmbReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void GenerateReport()
        {
            string reportType = cmbReportType.SelectedItem.ToString();
            
            try
            {
                DataTable reportData = null;

                switch (reportType)
                {
                    case "إحصائيات الموظفين":
                        reportData = hrService.GetEmployeeStatistics();
                        break;
                    case "توزيع الموظفين بالأقسام":
                        reportData = GetDepartmentDistribution();
                        break;
                    case "سجل الرواتب":
                        reportData = hrService.GetPayrollHistory();
                        break;
                    case "المكافآت والخصومات":
                        reportData = GetBonusesDeductions();
                        break;
                }

                if (reportData != null)
                {
                    dgvReports.DataSource = reportData;
                    
                    // تنسيق الأعمدة الرقمية
                    foreach (DataGridViewColumn column in dgvReports.Columns)
                    {
                        if (column.Name.Contains("راتب") || column.Name.Contains("مبلغ") || 
                            column.Name.Contains("مكافأة") || column.Name.Contains("خصم"))
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

        private DataTable GetDepartmentDistribution()
        {
            string query = @"
                SELECT 
                    d.department_name as 'اسم القسم',
                    COUNT(e.Emp_ID) as 'عدد الموظفين',
                    FORMAT(COUNT(e.Emp_ID) * 100.0 / (SELECT COUNT(*) FROM Employee), 'N2') + ' %' as 'النسبة'
                FROM Department d
                LEFT JOIN Employee e ON d.Dept_ID = e.Dept_ID
                GROUP BY d.department_name
                ORDER BY COUNT(e.Emp_ID) DESC";

            return databaseService.ExecuteQuery(query);
        }

        private DataTable GetBonusesDeductions()
        {
            string query = @"
                SELECT 
                    e.full_name as 'اسم الموظف',
                    SUM(p.bonuses) as 'إجمالي المكافآت',
                    SUM(p.deductions) as 'إجمالي الخصومات',
                    SUM(p.bonuses - p.deductions) as 'صافي الإضافات'
                FROM Payroll p
                INNER JOIN Employee e ON p.Emp_ID = e.Emp_ID
                WHERE p.payment_date BETWEEN @FromDate AND @ToDate
                GROUP BY e.full_name
                ORDER BY SUM(p.bonuses - p.deductions) DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@FromDate", dtpFrom.Value),
                new SqlParameter("@ToDate", dtpTo.Value)
            };

            return databaseService.ExecuteQuery(query, parameters);
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (dgvReports.DataSource != null)
            {
                try
                {
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.Filter = "Excel Files|*.csv";
                    saveDialog.FileName = $"تقرير_الموارد_البشرية_{DateTime.Now:yyyyMMdd}.csv";
                    
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
            if (dgvReports.DataSource != null)
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