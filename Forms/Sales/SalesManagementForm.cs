using System;
using System.Drawing;
using System.Windows.Forms;
using MIS_System_App.Forms.Reports;
using MIS_System_App.Models;

namespace MIS_System_App.Forms.Sales
{
    public partial class SalesManagementForm : Form
    {
        private User currentUser;

        public SalesManagementForm(User user)
        {
            InitializeComponent();
            currentUser = user;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "إدارة المبيعات والعملاء";
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            // إنشاء واجهة المبيعات
            CreateSalesInterface();

            this.ResumeLayout(false);
        }

        private void CreateSalesInterface()
        {
            // لوحة العنوان
            Panel titlePanel = new Panel();
            titlePanel.Dock = DockStyle.Top;
            titlePanel.Height = 60;
            titlePanel.BackColor = Color.LightSkyBlue;

            Label titleLabel = new Label();
            titleLabel.Text = "نظام إدارة المبيعات والعملاء";
            titleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Fill;
            titlePanel.Controls.Add(titleLabel);

            // لوحة الأزرار
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Top;
            buttonPanel.Height = 100;

            TableLayoutPanel buttonsLayout = new TableLayoutPanel();
            buttonsLayout.Dock = DockStyle.Fill;
            buttonsLayout.RowCount = 2;
            buttonsLayout.ColumnCount = 3;
            buttonsLayout.Padding = new Padding(10);

            string[] buttonTexts = { "إدارة العملاء", "طلبات البيع", "الفواتير", "التقارير", "الإحصائيات", "العودة" };
            Color[] buttonColors = { Color.LightGreen, Color.LightBlue, Color.LightYellow, Color.LightPink, Color.LightGray, Color.LightCoral };

            for (int i = 0; i < 6; i++)
            {
                Button button = new Button();
                button.Text = buttonTexts[i];
                button.BackColor = buttonColors[i];
                button.Dock = DockStyle.Fill;
                button.Font = new Font("Arial", 10, FontStyle.Bold);
                button.Click += SalesButton_Click;
                
                int row = i / 3;
                int col = i % 3;
                buttonsLayout.Controls.Add(button, col, row);
            }

            buttonPanel.Controls.Add(buttonsLayout);

            // لوحة المحتوى
            Panel contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = Color.White;

            Label contentLabel = new Label();
            contentLabel.Text = "مرحباً بك في وحدة المبيعات\n\nهنا يمكنك:\n• إدارة بيانات العملاء\n• إنشاء ومتابعة طلبات البيع\n• إصدار الفواتير\n• عرض تقارير المبيعات";
            contentLabel.Font = new Font("Arial", 12, FontStyle.Regular);
            contentLabel.TextAlign = ContentAlignment.MiddleCenter;
            contentLabel.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(contentLabel);

            this.Controls.AddRange(new Control[] { contentPanel, buttonPanel, titlePanel });
        }

        // استبدال دالة SalesButton_Click الحالية
// تحديث دالة SalesButton_Click
private void SalesButton_Click(object sender, EventArgs e)
{
    Button clickedButton = (Button)sender;
    string buttonText = clickedButton.Text;
    
    try
    {
        switch (buttonText)
        {
            case "إدارة العملاء":
                CustomerManagementForm customerForm = new CustomerManagementForm(currentUser);
                customerForm.ShowDialog();
                break;
            case "طلبات البيع":
                SalesOrderForm salesOrderForm = new SalesOrderForm();
                salesOrderForm.ShowDialog();
                break;
            case "الفواتير":
                InvoiceManagementForm invoiceForm = new InvoiceManagementForm(currentUser);
                invoiceForm.ShowDialog();
                break;
            case "التقارير":
                ReportsForm reportsForm = new ReportsForm(currentUser);
                reportsForm.ShowDialog();
                break;
            case "الإحصائيات":
                SalesDashboardForm dashboardForm = new SalesDashboardForm(currentUser);
                dashboardForm.ShowDialog();
                break;
            case "العودة":
                this.Close();
                break;
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"حدث خطأ: {ex.Message}", "خطأ");
    }
}
    }
}