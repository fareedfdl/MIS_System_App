using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient; // إضافة هذا السطر
using MIS_System_App.Services;
using MIS_System_App.Models;

namespace MIS_System_App.Forms
{
    public partial class DashboardForm : Form
    {
        private User currentUser;
        private DatabaseService databaseService;
        private Label lblTotalCustomers, lblTotalProducts, lblTotalEmployees, lblTotalSales;
        private Label lblPendingOrders, lblLowStock, lblTodaySales, lblMonthlySales;
        private System.Windows.Forms.Timer refreshTimer; // تحديد النوع بوضوح

        public DashboardForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            databaseService = new DatabaseService();
            LoadDashboardData();
            StartAutoRefresh();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "لوحة التحكم الرئيسية";
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
            topPanel.BackColor = Color.FromArgb(52, 152, 219);

            Label titleLabel = new Label();
            titleLabel.Text = "لوحة التحكم - نظام إدارة المعلومات المتكامل";
            titleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Fill;
            topPanel.Controls.Add(titleLabel);

            // لوحة الإحصائيات السريعة
            Panel statsPanel = new Panel();
            statsPanel.Dock = DockStyle.Top;
            statsPanel.Height = 200;
            statsPanel.Padding = new Padding(20);
            statsPanel.BackColor = Color.FromArgb(248, 248, 248);

            CreateQuickStats(statsPanel);

            // لوحة الإحصائيات التفصيلية
            Panel detailedStatsPanel = new Panel();
            detailedStatsPanel.Dock = DockStyle.Top;
            detailedStatsPanel.Height = 200;
            detailedStatsPanel.Padding = new Padding(20);
            detailedStatsPanel.BackColor = Color.White;

            CreateDetailedStats(detailedStatsPanel);

            // لوحة الأخبار والتنبيهات
            Panel alertsPanel = new Panel();
            alertsPanel.Dock = DockStyle.Fill;
            alertsPanel.Padding = new Padding(20);
            alertsPanel.BackColor = Color.FromArgb(248, 248, 248);

            CreateAlertsPanel(alertsPanel);

            this.Controls.AddRange(new Control[] { alertsPanel, detailedStatsPanel, statsPanel, topPanel });
        }

        private void CreateQuickStats(Panel panel)
        {
            Label sectionTitle = new Label();
            sectionTitle.Text = "الإحصائيات السريعة";
            sectionTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            sectionTitle.ForeColor = Color.FromArgb(52, 73, 94);
            sectionTitle.Dock = DockStyle.Top;
            sectionTitle.Height = 30;

            TableLayoutPanel tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.RowCount = 1;
            tableLayout.ColumnCount = 4;
            tableLayout.Padding = new Padding(10);
            tableLayout.Margin = new Padding(0, 40, 0, 0);

            // العملاء
            Panel customersCard = CreateDashboardCard("👥 العملاء", "0", Color.FromArgb(46, 204, 113), "إجمالي العملاء");
            lblTotalCustomers = (Label)customersCard.Controls[1];
            tableLayout.Controls.Add(customersCard, 0, 0);

            // المنتجات
            Panel productsCard = CreateDashboardCard("📦 المنتجات", "0", Color.FromArgb(155, 89, 182), "إجمالي المنتجات");
            lblTotalProducts = (Label)productsCard.Controls[1];
            tableLayout.Controls.Add(productsCard, 1, 0);

            // الموظفين
            Panel employeesCard = CreateDashboardCard("👨‍💼 الموظفين", "0", Color.FromArgb(52, 152, 219), "إجمالي الموظفين");
            lblTotalEmployees = (Label)employeesCard.Controls[1];
            tableLayout.Controls.Add(employeesCard, 2, 0);

            // المبيعات
            Panel salesCard = CreateDashboardCard("💰 المبيعات", "0", Color.FromArgb(241, 196, 15), "إجمالي المبيعات");
            lblTotalSales = (Label)salesCard.Controls[1];
            tableLayout.Controls.Add(salesCard, 3, 0);

            panel.Controls.AddRange(new Control[] { tableLayout, sectionTitle });
        }

        private void CreateDetailedStats(Panel panel)
        {
            Label sectionTitle = new Label();
            sectionTitle.Text = "الإحصائيات التفصيلية";
            sectionTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            sectionTitle.ForeColor = Color.FromArgb(52, 73, 94);
            sectionTitle.Dock = DockStyle.Top;
            sectionTitle.Height = 30;

            TableLayoutPanel tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.RowCount = 1;
            tableLayout.ColumnCount = 4;
            tableLayout.Padding = new Padding(10);
            tableLayout.Margin = new Padding(0, 40, 0, 0);

            // طلبات معلقة
            Panel pendingCard = CreateDashboardCard("⏳ طلبات معلقة", "0", Color.FromArgb(230, 126, 34), "تتطلب المتابعة");
            lblPendingOrders = (Label)pendingCard.Controls[1];
            tableLayout.Controls.Add(pendingCard, 0, 0);

            // مخزون منخفض
            Panel lowStockCard = CreateDashboardCard("⚠️ مخزون منخفض", "0", Color.FromArgb(231, 76, 60), "تحتاج إعادة طلب");
            lblLowStock = (Label)lowStockCard.Controls[1];
            tableLayout.Controls.Add(lowStockCard, 1, 0);

            // مبيعات اليوم
            Panel todaySalesCard = CreateDashboardCard("📊 مبيعات اليوم", "0", Color.FromArgb(46, 204, 113), "إيرادات اليوم");
            lblTodaySales = (Label)todaySalesCard.Controls[1];
            tableLayout.Controls.Add(todaySalesCard, 2, 0);

            // مبيعات الشهر
            Panel monthlySalesCard = CreateDashboardCard("📈 مبيعات الشهر", "0", Color.FromArgb(142, 68, 173), "إيرادات الشهر");
            lblMonthlySales = (Label)monthlySalesCard.Controls[1];
            tableLayout.Controls.Add(monthlySalesCard, 3, 0);

            panel.Controls.AddRange(new Control[] { tableLayout, sectionTitle });
        }

        private void CreateAlertsPanel(Panel panel)
        {
            Label sectionTitle = new Label();
            sectionTitle.Text = "التنبيهات والأخبار الحديثة";
            sectionTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            sectionTitle.ForeColor = Color.FromArgb(52, 73, 94);
            sectionTitle.Dock = DockStyle.Top;
            sectionTitle.Height = 30;

            DataGridView alertsGrid = new DataGridView();
            alertsGrid.Dock = DockStyle.Fill;
            alertsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            alertsGrid.ReadOnly = true;
            alertsGrid.RowHeadersVisible = false;
            alertsGrid.Margin = new Padding(0, 40, 0, 0);
            alertsGrid.BackgroundColor = Color.White;

            // تحميل التنبيهات
            LoadAlerts(alertsGrid);

            panel.Controls.AddRange(new Control[] { alertsGrid, sectionTitle });
        }

        private Panel CreateDashboardCard(string title, string value, Color color, string description)
        {
            Panel card = new Panel();
            card.BackColor = color;
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Padding = new Padding(15);
            card.Margin = new Padding(5);
            card.Cursor = Cursors.Hand;

            // تأثير عند المرور
            card.MouseEnter += (s, e) => { card.BackColor = ControlPaint.Light(color); };
            card.MouseLeave += (s, e) => { card.BackColor = color; };

            Label titleLabel = new Label();
            titleLabel.Text = title;
            titleLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.TextAlign = ContentAlignment.MiddleRight;
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 25;

            Label valueLabel = new Label();
            valueLabel.Text = value;
            valueLabel.Font = new Font("Arial", 24, FontStyle.Bold);
            valueLabel.ForeColor = Color.White;
            valueLabel.TextAlign = ContentAlignment.MiddleCenter;
            valueLabel.Dock = DockStyle.Fill;
            valueLabel.Padding = new Padding(0, 10, 0, 10);

            Label descLabel = new Label();
            descLabel.Text = description;
            descLabel.Font = new Font("Arial", 9, FontStyle.Italic);
            descLabel.ForeColor = Color.White;
            descLabel.TextAlign = ContentAlignment.MiddleCenter;
            descLabel.Dock = DockStyle.Bottom;
            descLabel.Height = 20;

            card.Controls.AddRange(new Control[] { descLabel, valueLabel, titleLabel });

            return card;
        }

        private void LoadDashboardData()
        {
            try
            {
                // العملاء
                string customersQuery = "SELECT COUNT(*) FROM Customer";
                object customersResult = databaseService.ExecuteScalar(customersQuery);
                lblTotalCustomers.Text = customersResult?.ToString() ?? "0";

                // المنتجات
                string productsQuery = "SELECT COUNT(*) FROM Product WHERE is_active = 1";
                object productsResult = databaseService.ExecuteScalar(productsQuery);
                lblTotalProducts.Text = productsResult?.ToString() ?? "0";

                // الموظفين
                string employeesQuery = "SELECT COUNT(*) FROM Employee";
                object employeesResult = databaseService.ExecuteScalar(employeesQuery);
                lblTotalEmployees.Text = employeesResult?.ToString() ?? "0";

                // إجمالي المبيعات
                string totalSalesQuery = "SELECT ISNULL(SUM(total_amount), 0) FROM Sales_Order";
                object totalSalesResult = databaseService.ExecuteScalar(totalSalesQuery);
                lblTotalSales.Text = $"{Convert.ToDecimal(totalSalesResult):N0}";

                // طلبات معلقة
                string pendingQuery = "SELECT COUNT(*) FROM Sales_Order WHERE status = 'Pending'";
                object pendingResult = databaseService.ExecuteScalar(pendingQuery);
                lblPendingOrders.Text = pendingResult?.ToString() ?? "0";

                // مخزون منخفض
                string lowStockQuery = "SELECT COUNT(*) FROM Product WHERE available_quantity <= reorder_level AND is_active = 1";
                object lowStockResult = databaseService.ExecuteScalar(lowStockQuery);
                lblLowStock.Text = lowStockResult?.ToString() ?? "0";

                // مبيعات اليوم
                string todaySalesQuery = "SELECT ISNULL(SUM(total_amount), 0) FROM Sales_Order WHERE CAST(order_date AS DATE) = CAST(GETDATE() AS DATE)";
                object todaySalesResult = databaseService.ExecuteScalar(todaySalesQuery);
                lblTodaySales.Text = $"{Convert.ToDecimal(todaySalesResult):N0}";

                // مبيعات الشهر
                string monthlySalesQuery = "SELECT ISNULL(SUM(total_amount), 0) FROM Sales_Order WHERE MONTH(order_date) = MONTH(GETDATE()) AND YEAR(order_date) = YEAR(GETDATE())";
                object monthlySalesResult = databaseService.ExecuteScalar(monthlySalesQuery);
                lblMonthlySales.Text = $"{Convert.ToDecimal(monthlySalesResult):N0}";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل بيانات اللوحة: {ex.Message}", "خطأ");
            }
        }

        private void LoadAlerts(DataGridView grid)
        {
            try
            {
                string query = @"
                    -- منتجات تحتاج إعادة طلب
                    SELECT '⚠️ مخزون منخفض' as 'النوع', 
                           product_name + ' - الكمية: ' + CAST(available_quantity as nvarchar) as 'الوصف',
                           'high' as 'الأهمية'
                    FROM Product 
                    WHERE available_quantity <= reorder_level AND is_active = 1
                    
                    UNION ALL
                    
                    -- طلبات معلقة
                    SELECT '⏳ طلب معلق' as 'النوع',
                           'طلب رقم ' + CAST(Order_ID as nvarchar) + ' - العميل: ' + customer_name as 'الوصف',
                           'medium' as 'الأهمية'
                    FROM Sales_Order so
                    INNER JOIN Customer c ON so.Customer_ID = c.Customer_ID
                    WHERE so.status = 'Pending'
                    
                    UNION ALL
                    
                    -- فواتير متأخرة
                    SELECT '💰 فاتورة متأخرة' as 'النوع',
                           'فاتورة رقم ' + CAST(Invoice_ID as nvarchar) + ' - العميل: ' + customer_name as 'الوصف',
                           'high' as 'الأهمية'
                    FROM Invoice i
                    INNER JOIN Customer c ON i.Customer_ID = c.Customer_ID
                    WHERE i.status != 'Paid' AND i.due_date < GETDATE()
                    
                    ORDER BY 
                        CASE WHEN الأهمية = 'high' THEN 1
                             WHEN الأهمية = 'medium' THEN 2
                             ELSE 3 END,
                        النوع";

                DataTable alerts = databaseService.ExecuteQuery(query);
                grid.DataSource = alerts;

                // تنسيق الألوان حسب الأهمية
                grid.RowPrePaint += (s, e) =>
                {
                    if (e.RowIndex >= 0 && e.RowIndex < grid.Rows.Count)
                    {
                        string importance = grid.Rows[e.RowIndex].Cells["الأهمية"].Value?.ToString();
                        if (importance == "high")
                        {
                            grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightPink;
                        }
                        else if (importance == "medium")
                        {
                            grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل التنبيهات: {ex.Message}", "خطأ");
            }
        }

        private void StartAutoRefresh()
        {
            refreshTimer = new System.Windows.Forms.Timer(); // استخدام System.Windows.Forms.Timer
            refreshTimer.Interval = 30000; // 30 ثانية
            refreshTimer.Tick += (s, e) => LoadDashboardData();
            refreshTimer.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            refreshTimer?.Stop();
            refreshTimer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}