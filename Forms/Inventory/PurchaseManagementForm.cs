using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MIS_System_App.Services;
using MIS_System_App.Models;

namespace MIS_System_App.Forms.Inventory
{
    public partial class PurchaseManagementForm : Form
    {
        private User currentUser;
        private DatabaseService databaseService;

        public PurchaseManagementForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            databaseService = new DatabaseService();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "إدارة المشتريات";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            // إنشاء واجهة بسيطة
            Label titleLabel = new Label();
            titleLabel.Text = "وحدة إدارة المشتريات - قيد التطوير";
            titleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Fill;

            Label descLabel = new Label();
            descLabel.Text = "هذه الوحدة قيد التطوير وسيتم إضافتها قريباً\n\nالوظائف المتوقعة:\n• طلبات الشراء من الموردين\n• فواتير المشتريات\n• متابعة أوامر الشراء\n• إدارة الموردين";
            descLabel.Font = new Font("Arial", 12, FontStyle.Regular);
            descLabel.TextAlign = ContentAlignment.MiddleCenter;
            descLabel.Dock = DockStyle.Fill;

            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.Controls.Add(descLabel);
            panel.Controls.Add(titleLabel);

            this.Controls.Add(panel);
            this.ResumeLayout(false);
        }
    }
}