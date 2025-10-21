using System;
using System.Windows.Forms;
using MIS_System_App.Models;
using MIS_System_App.Forms.HR;
using MIS_System_App.Forms.Sales;
using MIS_System_App.Forms.Inventory;
using MIS_System_App.Forms.Settings;
using MIS_System_App.Forms.Reports;

namespace MIS_System_App
{
    public partial class MainForm : Form
    {
        private User currentUser;

        public MainForm(User user)
        {
            InitializeComponent();
            currentUser = user;

            // ربط الأحداث يدوياً بعد InitializeComponent
            BindEvents();

            InitializeMainForm();
        }

        private void BindEvents()
        {
            // ربط أحداث الأزرار
            if (btnEmployees != null) btnEmployees.Click += BtnEmployees_Click;
            if (btnSales != null) btnSales.Click += BtnSales_Click;
            if (btnInventory != null) btnInventory.Click += BtnInventory_Click;
            if (btnReports != null) btnReports.Click += BtnReports_Click;
            if (btnSettings != null) btnSettings.Click += BtnSettings_Click;
            if (btnLogout != null) btnLogout.Click += BtnLogout_Click;

            // ربط أحداث البطاقات
            BindCardEvents();
        }

        private void BindCardEvents()
        {
            // البحث عن جميع البطاقات وربط أحداثها
            foreach (Control control in panelContent.Controls)
            {
                if (control is TableLayoutPanel cardsGrid)
                {
                    foreach (Control card in cardsGrid.Controls)
                    {
                        if (card is Panel panel && panel.Tag == null)
                        {
                            // تحديد نوع البطاقة من محتواها
                            string cardType = DetermineCardType(panel);
                            if (!string.IsNullOrEmpty(cardType))
                            {
                                panel.Click += (s, e) => HandleCardClick(cardType, s, e);

                                // ربط الحدث لعناصر البطاقة أيضاً
                                foreach (Control child in panel.Controls)
                                {
                                    child.Click += (s, e) => HandleCardClick(cardType, s, e);
                                }
                            }
                        }
                    }
                }
            }
        }

        private string DetermineCardType(Panel card)
        {
            foreach (Control control in card.Controls)
            {
                if (control is Label label && label.Font.Size >= 14)
                {
                    string text = label.Text;
                    if (text.Contains("الموارد البشرية")) return "employees";
                    if (text.Contains("المبيعات")) return "sales";
                    if (text.Contains("المخزون")) return "inventory";
                    if (text.Contains("التقارير")) return "reports";
                    if (text.Contains("الإعدادات")) return "settings";
                    if (text.Contains("الإشعارات")) return "notifications";
                }
            }
            return null;
        }

        private void HandleCardClick(string cardType, object sender, EventArgs e)
        {
            switch (cardType)
            {
                case "employees": BtnEmployees_Click(sender, e); break;
                case "sales": BtnSales_Click(sender, e); break;
                case "inventory": BtnInventory_Click(sender, e); break;
                case "reports": BtnReports_Click(sender, e); break;
                case "settings": BtnSettings_Click(sender, e); break;
                case "notifications": ShowNotifications(); break;
            }
        }

        private void InitializeMainForm()
        {
            this.Text = $"نظام إدارة المعلومات المتكامل - {currentUser.Username}";

            // تحديث معلومات المستخدم في الواجهة
            if (lblWelcome != null && lblRole != null)
            {
                lblWelcome.Text = $"مرحباً بك، {currentUser.Username}";
                lblRole.Text = $"الدور: {currentUser.RoleName}";
            }

            // تعطيل الأزرار حسب الصلاحيات
            ApplyPermissions();

            // تحديث الوقت فوراً
            UpdateDateTime();

            // ربط أحداث الأزرار الجديدة
            if (btnReports != null) btnReports.Click += BtnReports_Click;
            if (btnSettings != null) btnSettings.Click += BtnSettings_Click;
        }

        private void UpdateDateTime()
        {
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

        private void ApplyPermissions()
        {
            // حسب دور المستخدم، يمكن تعطيل بعض الوظائف
            if (btnEmployees != null && btnSales != null && btnInventory != null && btnReports != null && btnSettings != null)
            {
                switch (currentUser.RoleName)
                {
                    case "مدير مبيعات":
                        btnEmployees.Enabled = false;
                        btnInventory.Enabled = false;
                        btnReports.Enabled = false;
                        btnSettings.Enabled = false;
                        break;
                    case "موظف مخزون":
                        btnEmployees.Enabled = false;
                        btnSales.Enabled = false;
                        btnReports.Enabled = false;
                        btnSettings.Enabled = false;
                        break;
                    case "مدير النظام":
                        // جميع الصلاحيات مفعلة
                        break;
                    default:
                        // للمستخدمين الجدد، تعطيل كل شيء حتى يتم تعيين الصلاحيات
                        btnEmployees.Enabled = false;
                        btnSales.Enabled = false;
                        btnInventory.Enabled = false;
                        btnReports.Enabled = false;
                        btnSettings.Enabled = false;
                        break;
                }
            }
        }

        private void BtnEmployees_Click(object sender, EventArgs e)
        {
            try
            {
                EmployeeManagementForm empForm = new EmployeeManagementForm(currentUser);
                empForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء فتح وحدة الموارد البشرية: {ex.Message}",
                              "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSales_Click(object sender, EventArgs e)
        {
            try
            {
                SalesManagementForm salesForm = new SalesManagementForm(currentUser);
                salesForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء فتح وحدة المبيعات: {ex.Message}",
                              "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnInventory_Click(object sender, EventArgs e)
        {
            try
            {
                InventoryManagementForm invForm = new InventoryManagementForm(currentUser);
                invForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء فتح وحدة المخزون: {ex.Message}",
                              "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReports_Click(object sender, EventArgs e)
        {
            try
            {
                ReportsForm reportsForm = new ReportsForm(currentUser);
                reportsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء فتح التقارير: {ex.Message}", "خطأ");
            }
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                SettingsForm settingsForm = new SettingsForm(currentUser);
                settingsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء فتح الإعدادات: {ex.Message}", "خطأ",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "هل أنت متأكد من تسجيل الخروج من النظام؟",
                "تأكيد تسجيل الخروج",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                try
                {
                    if (timer != null)
                    {
                        timer.Stop();
                        timer.Dispose();
                    }

                    Application.Restart();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"حدث خطأ أثناء تسجيل الخروج: {ex.Message}",
                                  "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowNotifications()
        {
            MessageBox.Show("وحدة الإشعارات - قيد التطوير\n\nسيتم إضافة:\n• تنبيهات المبيعات\n• تنبيهات المخزون\n• إشعارات النظام",
                          "تحت التطوير", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show(
                    "هل تريد تسجيل الخروج وإغلاق النظام؟",
                    "تأكيد الإغلاق",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }

            base.OnFormClosing(e);
        }
    }
}