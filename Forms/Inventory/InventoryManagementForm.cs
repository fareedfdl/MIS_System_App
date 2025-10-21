using System;
using System.Drawing;
using System.Windows.Forms;
using MIS_System_App.Forms.Reports;
using MIS_System_App.Models;

namespace MIS_System_App.Forms.Inventory
{
    public partial class InventoryManagementForm : Form
    {
        private User currentUser;

        public InventoryManagementForm(User user)
        {
            InitializeComponent();
            currentUser = user;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "إدارة المخزون والمشتريات";
            this.Size = new Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            CreateInventoryInterface();

            this.ResumeLayout(false);
        }

        private void CreateInventoryInterface()
        {
            // لوحة العنوان
            Panel titlePanel = new Panel();
            titlePanel.Dock = DockStyle.Top;
            titlePanel.Height = 60;
            titlePanel.BackColor = Color.LightGoldenrodYellow;

            Label titleLabel = new Label();
            titleLabel.Text = "نظام إدارة المخزون والمشتريات";
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

            string[] buttonTexts = { "إدارة المنتجات", "المخزون", "الموردين", "المشتريات", "تقارير المخزون", "العودة" };
            Color[] buttonColors = { Color.LightGreen, Color.LightBlue, Color.LightYellow, Color.LightPink, Color.LightGray, Color.LightCoral };

            for (int i = 0; i < 6; i++)
            {
                Button button = new Button();
                button.Text = buttonTexts[i];
                button.BackColor = buttonColors[i];
                button.Dock = DockStyle.Fill;
                button.Font = new Font("Arial", 10, FontStyle.Bold);
                button.Click += InventoryButton_Click;

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
            contentLabel.Text = "مرحباً بك في وحدة المخزون\n\nهنا يمكنك:\n• إدارة المنتجات والتصنيفات\n• متابعة مستويات المخزون\n• إدارة الموردين\n• تسجيل عمليات الشراء\n• تنبيهات نقص المخزون";
            contentLabel.Font = new Font("Arial", 12, FontStyle.Regular);
            contentLabel.TextAlign = ContentAlignment.MiddleCenter;
            contentLabel.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(contentLabel);

            this.Controls.AddRange(new Control[] { contentPanel, buttonPanel, titlePanel });
        }

        // استبدال دالة InventoryButton_Click الحالية
        private void InventoryButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            string buttonText = clickedButton.Text;

            try
            {
                switch (buttonText)
                {
                    case "إدارة المنتجات":
                        ProductManagementForm productForm = new ProductManagementForm(currentUser);
                        productForm.ShowDialog();
                        break;
                    case "المخزون":
                        InventoryReportForm inventoryForm = new InventoryReportForm(currentUser);
                        inventoryForm.ShowDialog();
                        break;
                    case "الموردين":
                        SupplierManagementForm supplierForm = new SupplierManagementForm(currentUser);
                        supplierForm.ShowDialog();
                        break;
                    case "المستودعات":
                        WarehouseManagementForm warehouseForm = new WarehouseManagementForm(currentUser);
                        warehouseForm.ShowDialog();
                        break;
                    case "المشتريات":
                        PurchaseManagementForm purchaseForm = new PurchaseManagementForm(currentUser);
                        purchaseForm.ShowDialog();
                        break;
                    case "تقارير المخزون":
                        ReportsForm reportsForm = new ReportsForm(currentUser);
                        reportsForm.ShowDialog();
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