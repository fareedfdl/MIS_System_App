using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MIS_System_App.Services;
using MIS_System_App.Models;

namespace MIS_System_App.Forms.Sales
{
    public partial class SalesDashboardForm : Form
    {
        private User currentUser;
        private DatabaseService databaseService;
        private SalesService salesService;
        private Label lblTotalSales, lblTotalCustomers, lblPendingOrders, lblCompletedOrders;

        public SalesDashboardForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            databaseService = new DatabaseService();
            salesService = new SalesService();
            LoadDashboardData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "لوحة تحكم المبيعات";
            this.Size = new Size(900, 600);
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
            topPanel.BackColor = Color.LightSkyBlue;

            Label titleLabel = new Label();
            titleLabel.Text = "لوحة تحكم المبيعات - نظرة عامة";
            titleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Fill;
            topPanel.Controls.Add(titleLabel);

            // لوحة الإحصائيات
            Panel statsPanel = new Panel();
            statsPanel.Dock = DockStyle.Top;
            statsPanel.Height = 150;
            statsPanel.Padding = new Padding(20);

            CreateStatisticsCards(statsPanel);

            // شبكة البيانات
            DataGridView dgvRecentOrders = new DataGridView();
            dgvRecentOrders.Dock = DockStyle.Fill;
            dgvRecentOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRecentOrders.ReadOnly = true;
            dgvRecentOrders.RowHeadersVisible = false;

            LoadRecentOrders(dgvRecentOrders);

            // أزرار التحكم
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);

            Button btnRefresh = new Button();
            btnRefresh.Text = "تحديث البيانات";
            btnRefresh.Size = new Size(120, 35);
            btnRefresh.Location = new Point(10, 12);
            btnRefresh.BackColor = Color.LightBlue;
            btnRefresh.Click += BtnRefresh_Click;

            Button btnClose = new Button();
            btnClose.Text = "إغلاق";
            btnClose.Size = new Size(80, 35);
            btnClose.Location = new Point(750, 12);
            btnClose.BackColor = Color.LightCoral;
            btnClose.Click += BtnClose_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnRefresh, btnClose });

            this.Controls.AddRange(new Control[] { dgvRecentOrders, statsPanel, buttonPanel, topPanel });
        }

        private void CreateStatisticsCards(Panel panel)
        {
            TableLayoutPanel tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.RowCount = 1;
            tableLayout.ColumnCount = 4;
            tableLayout.Padding = new Padding(10);

            // بطاقة إجمالي المبيعات
            Panel salesCard = CreateStatCard("إجمالي المبيعات", "0", Color.LightGreen);
            lblTotalSales = (Label)salesCard.Controls[1];
            tableLayout.Controls.Add(salesCard, 0, 0);

            // بطاقة عدد العملاء
            Panel customersCard = CreateStatCard("عدد العملاء", "0", Color.LightBlue);
            lblTotalCustomers = (Label)customersCard.Controls[1];
            tableLayout.Controls.Add(customersCard, 1, 0);

            // بطاقة الطلبات المعلقة
            Panel pendingCard = CreateStatCard("طلبات معلقة", "0", Color.LightYellow);
            lblPendingOrders = (Label)pendingCard.Controls[1];
            tableLayout.Controls.Add(pendingCard, 2, 0);

            // بطاقة الطلبات المكتملة
            Panel completedCard = CreateStatCard("طلبات مكتملة", "0", Color.LightPink);
            lblCompletedOrders = (Label)completedCard.Controls[1];
            tableLayout.Controls.Add(completedCard, 3, 0);

            panel.Controls.Add(tableLayout);
        }

        private Panel CreateStatCard(string title, string value, Color color)
        {
            Panel card = new Panel();
            card.BackColor = color;
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Padding = new Padding(10);
            card.Margin = new Padding(5);

            Label titleLabel = new Label();
            titleLabel.Text = title;
            titleLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 30;

            Label valueLabel = new Label();
            valueLabel.Text = value;
            valueLabel.Font = new Font("Arial", 18, FontStyle.Bold);
            valueLabel.TextAlign = ContentAlignment.MiddleCenter;
            valueLabel.Dock = DockStyle.Fill;
            valueLabel.ForeColor = Color.DarkBlue;

            card.Controls.Add(valueLabel);
            card.Controls.Add(titleLabel);

            return card;
        }

        private void LoadDashboardData()
        {
            try
            {
                // إجمالي المبيعات
                string salesQuery = "SELECT ISNULL(SUM(total_amount), 0) FROM Sales_Order WHERE MONTH(order_date) = MONTH(GETDATE())";
                object salesResult = databaseService.ExecuteScalar(salesQuery);
                lblTotalSales.Text = $"{Convert.ToDecimal(salesResult):C0}";

                // عدد العملاء
                string customersQuery = "SELECT COUNT(*) FROM Customer";
                object customersResult = databaseService.ExecuteScalar(customersQuery);
                lblTotalCustomers.Text = customersResult.ToString();

                // الطلبات المعلقة
                string pendingQuery = "SELECT COUNT(*) FROM Sales_Order WHERE status = 'Pending'";
                object pendingResult = databaseService.ExecuteScalar(pendingQuery);
                lblPendingOrders.Text = pendingResult.ToString();

                // الطلبات المكتملة
                string completedQuery = "SELECT COUNT(*) FROM Sales_Order WHERE status = 'Completed'";
                object completedResult = databaseService.ExecuteScalar(completedQuery);
                lblCompletedOrders.Text = completedResult.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل بيانات اللوحة: {ex.Message}", "خطأ");
            }
        }

        private void LoadRecentOrders(DataGridView dgv)
        {
            string query = @"
                SELECT TOP 10
                    Order_ID as 'رقم الطلب',
                    customer_name as 'اسم العميل',
                    order_date as 'تاريخ الطلب',
                    total_amount as 'المبلغ الإجمالي',
                    status as 'الحالة'
                FROM Sales_Order so
                INNER JOIN Customer c ON so.Customer_ID = c.Customer_ID
                ORDER BY order_date DESC";

            DataTable orders = databaseService.ExecuteQuery(query);
            dgv.DataSource = orders;

            // تنسيق الأعمدة
            if (dgv.Columns.Count > 0)
            {
                dgv.Columns["المبلغ الإجمالي"].DefaultCellStyle.Format = "C0";
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadDashboardData();
            MessageBox.Show("تم تحديث البيانات", "تحديث");
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}