using System;
using System.Drawing;
using System.Windows.Forms;
using System.Timers;

namespace MIS_System_App
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel panelSidebar;
        private Panel panelHeader;
        private Panel panelContent;
        private Label lblWelcome;
        private Label lblRole;
        private Label lblDateTime;
        private System.Timers.Timer timer;
        private Button btnEmployees, btnSales, btnInventory, btnReports, btnSettings, btnLogout;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ClientSize = new Size(1300, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "نظام إدارة المعلومات المتكامل";
            this.Font = new Font("Segoe UI", 9);
            this.BackColor = Color.White;

            // إعداد التايمر لتحديث الوقت
            this.timer = new System.Timers.Timer();
            this.timer.Interval = 1000;
            this.timer.Elapsed += Timer_Tick;
            this.timer.Start();

            CreateSidebar();
            CreateHeader();
            CreateContent();

            this.ResumeLayout(false);
        }

        private void CreateSidebar()
        {
            // اللوحة الجانبية
            panelSidebar = new Panel();
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Width = 280;
            panelSidebar.BackColor = Color.FromArgb(34, 40, 49);
            panelSidebar.Padding = new Padding(10);

            // شعار النظام
            Panel logoPanel = new Panel();
            logoPanel.Dock = DockStyle.Top;
            logoPanel.Height = 120;
            logoPanel.BackColor = Color.FromArgb(34, 40, 49);

            Label logoLabel = new Label();
            logoLabel.Text = "MIS\nSystem";
            logoLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            logoLabel.ForeColor = Color.White;
            logoLabel.TextAlign = ContentAlignment.MiddleCenter;
            logoLabel.Dock = DockStyle.Fill;
            logoPanel.Controls.Add(logoLabel);

            // أزرار القائمة
            Panel menuPanel = new Panel();
            menuPanel.Dock = DockStyle.Fill;
            menuPanel.BackColor = Color.FromArgb(34, 40, 49);

            btnEmployees = CreateMenuButton("الموارد البشرية", "👥", 0);
            btnSales = CreateMenuButton("المبيعات والعملاء", "💰", 1);
            btnInventory = CreateMenuButton("المخزون والمشتريات", "📦", 2);
            btnReports = CreateMenuButton("التقارير والإحصائيات", "📊", 3);
            btnSettings = CreateMenuButton("الإعدادات", "⚙️", 4);

            // زر تسجيل الخروج
            btnLogout = new Button();
            btnLogout.Text = "🚪 تسجيل الخروج";
            btnLogout.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnLogout.ForeColor = Color.White;
            btnLogout.BackColor = Color.FromArgb(231, 76, 60);
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Height = 50;
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.TextAlign = ContentAlignment.MiddleLeft;
            btnLogout.Padding = new Padding(15, 0, 0, 0);

            menuPanel.Controls.AddRange(new Control[] {
                btnEmployees, btnSales, btnInventory, btnReports, btnSettings, btnLogout
            });

            panelSidebar.Controls.AddRange(new Control[] { menuPanel, logoPanel });
            this.Controls.Add(panelSidebar);
        }

        private Button CreateMenuButton(string text, string icon, int order)
        {
            Button button = new Button();
            button.Text = $"{icon} {text}";
            button.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            button.ForeColor = Color.White;
            button.BackColor = Color.FromArgb(34, 40, 49);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Height = 55;
            button.Dock = DockStyle.Top;
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Padding = new Padding(15, 0, 0, 0);
            button.Tag = order;

            // أحداث الماوس لتأثير Hover
            button.MouseEnter += (s, e) =>
            {
                button.BackColor = Color.FromArgb(45, 52, 64);
                button.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            };

            button.MouseLeave += (s, e) =>
            {
                button.BackColor = Color.FromArgb(34, 40, 49);
                button.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            };

            return button;
        }

        private void CreateHeader()
        {
            // رأس الصفحة
            panelHeader = new Panel();
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 80;
            panelHeader.BackColor = Color.FromArgb(248, 248, 248);
            panelHeader.BorderStyle = BorderStyle.FixedSingle;

            // معلومات المستخدم
            Panel userPanel = new Panel();
            userPanel.Dock = DockStyle.Right;
            userPanel.Width = 400;
            userPanel.Padding = new Padding(20, 0, 20, 0);

            lblWelcome = new Label();
            lblWelcome.Text = "مرحباً بك، ...";
            lblWelcome.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblWelcome.ForeColor = Color.FromArgb(52, 73, 94);
            lblWelcome.TextAlign = ContentAlignment.MiddleRight;
            lblWelcome.Dock = DockStyle.Top;
            lblWelcome.Height = 30;

            lblRole = new Label();
            lblRole.Text = "الدور: ...";
            lblRole.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblRole.ForeColor = Color.FromArgb(127, 140, 141);
            lblRole.TextAlign = ContentAlignment.MiddleRight;
            lblRole.Dock = DockStyle.Top;
            lblRole.Height = 25;

            userPanel.Controls.AddRange(new Control[] { lblRole, lblWelcome });

            // الوقت والتاريخ
            Panel timePanel = new Panel();
            timePanel.Dock = DockStyle.Left;
            timePanel.Width = 300;
            timePanel.Padding = new Padding(20, 0, 0, 0);

            lblDateTime = new Label();
            lblDateTime.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy\nhh:mm:ss tt");
            lblDateTime.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblDateTime.ForeColor = Color.FromArgb(52, 73, 94);
            lblDateTime.TextAlign = ContentAlignment.MiddleLeft;
            lblDateTime.Dock = DockStyle.Fill;

            timePanel.Controls.Add(lblDateTime);

            panelHeader.Controls.AddRange(new Control[] { userPanel, timePanel });
            this.Controls.Add(panelHeader);
        }

        private void CreateContent()
        {
            // منطقة المحتوى الرئيسية
            panelContent = new Panel();
            panelContent.Dock = DockStyle.Fill;
            panelContent.BackColor = Color.FromArgb(248, 248, 248);
            panelContent.Padding = new Padding(30);

            // إنشاء لوحة التحكم
            CreateDashboard();

            this.Controls.Add(panelContent);
        }

        private void CreateDashboard()
        {
            // عنوان اللوحة
            Label dashboardTitle = new Label();
            dashboardTitle.Text = "لوحة التحكم الرئيسية";
            dashboardTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            dashboardTitle.ForeColor = Color.FromArgb(52, 73, 94);
            dashboardTitle.Dock = DockStyle.Top;
            dashboardTitle.Height = 60;
            dashboardTitle.TextAlign = ContentAlignment.MiddleRight;

            // شبكة البطاقات
            TableLayoutPanel cardsGrid = new TableLayoutPanel();
            cardsGrid.Dock = DockStyle.Fill;
            cardsGrid.RowCount = 2;
            cardsGrid.ColumnCount = 3;
            cardsGrid.Padding = new Padding(10);
            cardsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            cardsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            cardsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            cardsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            cardsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            // إنشاء البطاقات
            cardsGrid.Controls.Add(CreateDashboardCard("👥 إدارة الموارد البشرية",
                "إدارة الموظفين، الرواتب، والإجازات", Color.FromArgb(52, 152, 219), "employees"), 0, 0);

            cardsGrid.Controls.Add(CreateDashboardCard("💰 إدارة المبيعات",
                "العملاء، الطلبات، والفواتير", Color.FromArgb(46, 204, 113), "sales"), 1, 0);

            cardsGrid.Controls.Add(CreateDashboardCard("📦 إدارة المخزون",
                "المنتجات، الموردين، والمشتريات", Color.FromArgb(155, 89, 182), "inventory"), 2, 0);

            cardsGrid.Controls.Add(CreateDashboardCard("📊 التقارير والإحصائيات",
                "تقارير شاملة وتحليلات متقدمة", Color.FromArgb(241, 196, 15), "reports"), 0, 1);

            cardsGrid.Controls.Add(CreateDashboardCard("⚙️ إعدادات النظام",
                "إعدادات المستخدمين والصلاحيات", Color.FromArgb(230, 126, 34), "settings"), 1, 1);

            cardsGrid.Controls.Add(CreateDashboardCard("🔔 الإشعارات",
                "آخر التحديثات والتنبيهات", Color.FromArgb(231, 76, 60), "notifications"), 2, 1);

            panelContent.Controls.AddRange(new Control[] { cardsGrid, dashboardTitle });
        }

        private Panel CreateDashboardCard(string title, string description, Color color, string cardType)
        {
            Panel card = new Panel();
            card.BackColor = Color.White;
            card.BorderStyle = BorderStyle.FixedSingle;
            card.Padding = new Padding(20);
            card.Margin = new Padding(10);
            card.Cursor = Cursors.Hand;

            // تأثير عند المرور بالماوس
            card.MouseEnter += (s, e) =>
            {
                card.BackColor = Color.FromArgb(245, 245, 245);
            };

            card.MouseLeave += (s, e) =>
            {
                card.BackColor = Color.White;
            };

            // أيقونة البطاقة
            Label iconLabel = new Label();
            iconLabel.Text = title.Split(' ')[0]; // أخذ الإيموجي فقط
            iconLabel.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            iconLabel.ForeColor = color;
            iconLabel.Location = new Point(20, 20);
            iconLabel.Size = new Size(60, 60);
            iconLabel.TextAlign = ContentAlignment.MiddleCenter;

            // عنوان البطاقة
            Label titleLabel = new Label();
            titleLabel.Text = title;
            titleLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(52, 73, 94);
            titleLabel.Location = new Point(90, 20);
            titleLabel.Size = new Size(180, 30);
            titleLabel.TextAlign = ContentAlignment.MiddleRight;

            // وصف البطاقة
            Label descLabel = new Label();
            descLabel.Text = description;
            descLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            descLabel.ForeColor = Color.FromArgb(127, 140, 141);
            descLabel.Location = new Point(20, 90);
            descLabel.Size = new Size(250, 40);
            descLabel.TextAlign = ContentAlignment.MiddleRight;

            card.Controls.AddRange(new Control[] { iconLabel, titleLabel, descLabel });
            return card;
        }

        private void Timer_Tick(object sender, ElapsedEventArgs e)
        {
            // تحديث الوقت في thread الآمن
            if (lblDateTime != null && !lblDateTime.IsDisposed)
            {
                if (lblDateTime.InvokeRequired)
                {
                    lblDateTime.Invoke(new Action(() =>
                    {
                        lblDateTime.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy\nhh:mm:ss tt");
                    }));
                }
                else
                {
                    lblDateTime.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy\nhh:mm:ss tt");
                }
            }
        }
    }
}