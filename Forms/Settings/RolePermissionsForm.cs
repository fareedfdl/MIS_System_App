using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.Settings
{
    public partial class RolePermissionsForm : Form
    {
        private DatabaseService databaseService;
        private int roleId;
        private string roleName;
        private CheckedListBox chkListPermissions;
        private Button btnSave, btnCancel;

        public RolePermissionsForm(int rId, string rName)
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            roleId = rId;
            roleName = rName;
            LoadPermissions();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = $"إدارة صلاحيات الدور: {roleName}";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            CreateControls();
            this.ResumeLayout(false);
        }

        private void CreateControls()
        {
            // عنوان النموذج
            Label titleLabel = new Label();
            titleLabel.Text = $"إدارة صلاحيات الدور: {roleName}";
            titleLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 50;
            titleLabel.BackColor = Color.LightSteelBlue;

            // قائمة الصلاحيات
            Label lblPermissions = new Label();
            lblPermissions.Text = "الصلاحيات المتاحة:";
            lblPermissions.Font = new Font("Arial", 11, FontStyle.Bold);
            lblPermissions.Dock = DockStyle.Top;
            lblPermissions.Height = 30;
            lblPermissions.Padding = new Padding(10, 5, 0, 0);

            chkListPermissions = new CheckedListBox();
            chkListPermissions.Dock = DockStyle.Fill;
            chkListPermissions.CheckOnClick = true;
            chkListPermissions.Font = new Font("Arial", 10);

            // أزرار التحكم
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);

            btnSave = new Button();
            btnSave.Text = "حفظ الصلاحيات";
            btnSave.Size = new Size(120, 35);
            btnSave.Location = new Point(200, 12);
            btnSave.BackColor = Color.LightGreen;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button();
            btnCancel.Text = "إلغاء";
            btnCancel.Size = new Size(100, 35);
            btnCancel.Location = new Point(80, 12);
            btnCancel.BackColor = Color.LightCoral;
            btnCancel.Click += BtnCancel_Click;

            buttonPanel.Controls.AddRange(new Control[] { btnSave, btnCancel });

            Panel contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Padding = new Padding(10);
            contentPanel.Controls.AddRange(new Control[] { chkListPermissions, lblPermissions });

            this.Controls.AddRange(new Control[] { contentPanel, buttonPanel, titleLabel });
        }

        private void LoadPermissions()
        {
            try
            {
                // تحميل جميع الصلاحيات المتاحة
                string permissionsQuery = "SELECT Permission_ID, permission_name, permission_description FROM Permission ORDER BY permission_name";
                DataTable allPermissions = databaseService.ExecuteQuery(permissionsQuery);

                // تحميل الصلاحيات الممنوحة للدور
                string rolePermissionsQuery = "SELECT Permission_ID FROM Role_Permission WHERE Role_ID = @RoleID";
                SqlParameter[] parameters = {
                    new SqlParameter("@RoleID", roleId)
                };
                DataTable rolePermissions = databaseService.ExecuteQuery(rolePermissionsQuery, parameters);

                // تعبئة قائمة الصلاحيات
                chkListPermissions.Items.Clear();
                foreach (DataRow row in allPermissions.Rows)
                {
                    int permissionId = Convert.ToInt32(row["Permission_ID"]);
                    string displayText = $"{row["permission_name"]} - {row["permission_description"]}";
                    
                    chkListPermissions.Items.Add(displayText, false);
                    chkListPermissions.SetItemChecked(chkListPermissions.Items.Count - 1, 
                        IsPermissionGranted(rolePermissions, permissionId));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الصلاحيات: {ex.Message}", "خطأ");
            }
        }

        private bool IsPermissionGranted(DataTable rolePermissions, int permissionId)
        {
            foreach (DataRow row in rolePermissions.Rows)
            {
                if (Convert.ToInt32(row["Permission_ID"]) == permissionId)
                    return true;
            }
            return false;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // حذف جميع الصلاحيات الحالية للدور
                string deleteQuery = "DELETE FROM Role_Permission WHERE Role_ID = @RoleID";
                SqlParameter[] deleteParams = {
                    new SqlParameter("@RoleID", roleId)
                };
                databaseService.ExecuteNonQuery(deleteQuery, deleteParams);

                // إضافة الصلاحيات المحددة
                string permissionsQuery = "SELECT Permission_ID, permission_name FROM Permission ORDER BY permission_name";
                DataTable allPermissions = databaseService.ExecuteQuery(permissionsQuery);

                for (int i = 0; i < chkListPermissions.Items.Count; i++)
                {
                    if (chkListPermissions.GetItemChecked(i))
                    {
                        int permissionId = Convert.ToInt32(allPermissions.Rows[i]["Permission_ID"]);
                        
                        string insertQuery = "INSERT INTO Role_Permission (Role_ID, Permission_ID, assigned_date) VALUES (@RoleID, @PermissionID, GETDATE())";
                        SqlParameter[] insertParams = {
                            new SqlParameter("@RoleID", roleId),
                            new SqlParameter("@PermissionID", permissionId)
                        };
                        databaseService.ExecuteNonQuery(insertQuery, insertParams);
                    }
                }

                MessageBox.Show("تم حفظ الصلاحيات بنجاح", "نجاح");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء حفظ الصلاحيات: {ex.Message}", "خطأ");
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}