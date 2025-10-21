using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MIS_System_App.Services;
using MIS_System_App.Models;

namespace MIS_System_App.Forms.HR
{
    public partial class PayrollManagementForm : Form
    {
        private User currentUser;
        private DatabaseService databaseService;
        private HRService hrService;
        private DataGridView dgvPayroll;
        private DateTimePicker dtpFrom, dtpTo;
        private Button btnAdd, btnRefresh, btnExport;

        public PayrollManagementForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            databaseService = new DatabaseService();
            hrService = new HRService();
            LoadPayrollData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "إدارة الرواتب";
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
            topPanel.Height = 100;
            topPanel.BackColor = Color.LightBlue;
            topPanel.Padding = new Padding(10);

            Label titleLabel = new Label();
            titleLabel.Text = "إدارة الرواتب والمسيرات";
            titleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 40;

            // لوحة الفلاتر
            Panel filterPanel = new Panel();
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Height = 60;
            filterPanel.Padding = new Padding(10);

            Label lblFrom = new Label();
            lblFrom.Text = "من تاريخ:";
            lblFrom.Location = new Point(900, 15);
            lblFrom.Size = new Size(70, 25);

            dtpFrom = new DateTimePicker();
            dtpFrom.Location = new Point(800, 15);
            dtpFrom.Size = new Size(90, 25);
            dtpFrom.Value = DateTime.Today.AddMonths(-1);

            Label lblTo = new Label();
            lblTo.Text = "إلى تاريخ:";
            lblTo.Location = new Point(700, 15);
            lblTo.Size = new Size(70, 25);

            dtpTo = new DateTimePicker();
            dtpTo.Location = new Point(600, 15);
            dtpTo.Size = new Size(90, 25);
            dtpTo.Value = DateTime.Today;

            btnRefresh = new Button();
            btnRefresh.Text = "تحديث البيانات";
            btnRefresh.Location = new Point(480, 15);
            btnRefresh.Size = new Size(110, 25);
            btnRefresh.BackColor = Color.LightGreen;
            btnRefresh.Click += BtnRefresh_Click;

            filterPanel.Controls.AddRange(new Control[] {
                lblFrom, dtpFrom,
                lblTo, dtpTo,
                btnRefresh
            });

            topPanel.Controls.AddRange(new Control[] { titleLabel, filterPanel });

            // شبكة البيانات
            dgvPayroll = new DataGridView();
            dgvPayroll.Dock = DockStyle.Fill;
            dgvPayroll.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPayroll.ReadOnly = true;
            dgvPayroll.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPayroll.RowHeadersVisible = false;

            // أزرار التحكم
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);

            btnAdd = new Button();
            btnAdd.Text = "إضافة راتب";
            btnAdd.Size = new Size(120, 35);
            btnAdd.Location = new Point(10, 12);
            btnAdd.BackColor = Color.LightGreen;
            btnAdd.Click += BtnAdd_Click;

            btnExport = new Button();
            btnExport.Text = "تصدير Excel";
            btnExport.Size = new Size(120, 35);
            btnExport.Location = new Point(140, 12);
            btnExport.BackColor = Color.LightBlue;
            btnExport.Click += BtnExport_Click;

            Button btnClose = new Button();
            btnClose.Text = "إغلاق";
            btnClose.Size = new Size(80, 35);
            btnClose.Location = new Point(1050, 12);
            btnClose.BackColor = Color.LightCoral;
            btnClose.Click += BtnClose_Click;

            buttonPanel.Controls.AddRange(new Control[] { 
                btnAdd, btnExport, btnClose 
            });

            this.Controls.AddRange(new Control[] { dgvPayroll, buttonPanel, topPanel });
        }

        private void LoadPayrollData()
        {
            DataTable payrollData = hrService.GetPayrollHistory();
            dgvPayroll.DataSource = payrollData;

            // تنسيق الأعمدة
            if (dgvPayroll.Columns.Count > 0)
            {
                dgvPayroll.Columns["الراتب الإجمالي"].DefaultCellStyle.Format = "C0";
                dgvPayroll.Columns["الضرائب"].DefaultCellStyle.Format = "C0";
                dgvPayroll.Columns["المكافآت"].DefaultCellStyle.Format = "C0";
                dgvPayroll.Columns["الخصومات"].DefaultCellStyle.Format = "C0";
                dgvPayroll.Columns["صافي الراتب"].DefaultCellStyle.Format = "C0";
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            PayrollForm payrollForm = new PayrollForm();
            if (payrollForm.ShowDialog() == DialogResult.OK)
            {
                LoadPayrollData();
                MessageBox.Show("تم إضافة الراتب بنجاح", "نجاح");
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadPayrollData();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Excel Files|*.csv";
                saveDialog.FileName = $"كشوف_الرواتب_{DateTime.Now:yyyyMMdd}.csv";
                
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show($"سيتم تصدير البيانات إلى: {saveDialog.FileName}", "تصدير Excel");
                    // كود التصدير الفعلي
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في التصدير: {ex.Message}", "خطأ");
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}