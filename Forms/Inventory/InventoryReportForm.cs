using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MIS_System_App.Services;
using MIS_System_App.Models;

namespace MIS_System_App.Forms.Inventory
{
    public partial class InventoryReportForm : Form
    {
        private User currentUser;
        private DatabaseService databaseService;
        private InventoryService inventoryService;
        private DataGridView dgvInventory;
        private ComboBox cmbReportType;
        private Button btnGenerate, btnExport, btnPrint;

        public InventoryReportForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            databaseService = new DatabaseService();
            inventoryService = new InventoryService();
            LoadInventoryData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "تقارير المخزون";
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
            titleLabel.Text = "تقارير المخزون والمستودعات";
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
                "المخزون بالمستودعات",
                "المنتجات تحت الطلب",
                "حركة المخزون"
            });
            cmbReportType.SelectedIndex = 0;
            cmbReportType.SelectedIndexChanged += CmbReportType_SelectedIndexChanged;

            btnGenerate = new Button();
            btnGenerate.Text = "توليد التقرير";
            btnGenerate.Location = new Point(520, 15);
            btnGenerate.Size = new Size(120, 25);
            btnGenerate.BackColor = Color.LightBlue;
            btnGenerate.Click += BtnGenerate_Click;

            btnExport = new Button();
            btnExport.Text = "تصدير Excel";
            btnExport.Location = new Point(390, 15);
            btnExport.Size = new Size(120, 25);
            btnExport.BackColor = Color.LightGreen;
            btnExport.Click += BtnExport_Click;

            filterPanel.Controls.AddRange(new Control[] {
                lblReportType, cmbReportType,
                btnGenerate, btnExport
            });

            topPanel.Controls.AddRange(new Control[] { titleLabel, filterPanel });

            // شبكة البيانات
            dgvInventory = new DataGridView();
            dgvInventory.Dock = DockStyle.Fill;
            dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInventory.ReadOnly = true;
            dgvInventory.RowHeadersVisible = false;

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
            btnClose.Location = new Point(850, 12);
            btnClose.BackColor = Color.LightCoral;
            btnClose.Click += BtnClose_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnPrint, btnClose });

            this.Controls.AddRange(new Control[] { dgvInventory, buttonPanel, topPanel });
        }

        private void LoadInventoryData()
        {
            DataTable inventoryData = inventoryService.GetInventoryReport();
            dgvInventory.DataSource = inventoryData;
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
                    case "المخزون بالمستودعات":
                        reportData = inventoryService.GetInventoryReport();
                        break;
                    case "المنتجات تحت الطلب":
                        reportData = inventoryService.GetProductsNeedReorder();
                        break;
                    case "حركة المخزون":
                        reportData = GetInventoryMovement();
                        break;
                }

                if (reportData != null)
                {
                    dgvInventory.DataSource = reportData;
                    MessageBox.Show($"تم توليد تقرير {reportType} بنجاح", "نجاح");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في توليد التقرير: {ex.Message}", "خطأ");
            }
        }

        private DataTable GetInventoryMovement()
        {
            string query = @"
                SELECT 
                    p.product_name as 'اسم المنتج',
                    w.warehouse_name as 'المستودع',
                    'حركة افتراضية' as 'نوع الحركة',
                    10 as 'الكمية',
                    GETDATE() as 'التاريخ'
                FROM Product p
                CROSS JOIN Warehouse w
                WHERE p.is_active = 1
                LIMIT 20";

            return databaseService.ExecuteQuery(query);
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (dgvInventory.DataSource != null)
            {
                try
                {
                    // تنفيذ تصدير Excel (تنفيذ مبسط)
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.Filter = "Excel Files|*.csv";
                    saveDialog.FileName = $"تقرير_المخزون_{DateTime.Now:yyyyMMdd}.csv";
                    
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show($"سيتم تصدير التقرير إلى: {saveDialog.FileName}", "تصدير Excel");
                        // هنا يمكن إضافة كود التصدير الفعلي
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
            if (dgvInventory.DataSource != null)
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