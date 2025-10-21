using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.Settings
{
    public partial class RoleForm : Form
    {
        private DatabaseService databaseService;
        private int? roleId;
        private TextBox txtName, txtDescription;
        private Button btnSave, btnCancel;

        public RoleForm(int? rId = null)
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            roleId = rId;
            
            if (roleId.HasValue)
            {
                LoadRoleData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = roleId.HasValue ? "تعديل بيانات الدور" : "إضافة دور جديد";
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            CreateControls();
            this.ResumeLayout(false);
        }

        private void CreateControls()
        {
            int yPos = 20;
            int labelWidth = 120;
            int controlWidth = 300;

            // اسم الدور
            Label lblName = new Label();
            lblName.Text = "اسم الدور:";
            lblName.Location = new Point(350, yPos);
            lblName.Size = new Size(labelWidth, 25);

            txtName = new TextBox();
            txtName.Location = new Point(40, yPos);
            txtName.Size = new Size(controlWidth, 25);
            yPos += 40;

            // الوصف
            Label lblDescription = new Label();
            lblDescription.Text = "الوصف:";
            lblDescription.Location = new Point(350, yPos);
            lblDescription.Size = new Size(labelWidth, 25);

            txtDescription = new TextBox();
            txtDescription.Location = new Point(40, yPos);
            txtDescription.Size = new Size(controlWidth, 25);
            txtDescription.Multiline = true;
            txtDescription.Height = 80;
            yPos += 100;

            // أزرار الحفظ والإلغاء
            btnSave = new Button();
            btnSave.Text = "حفظ";
            btnSave.Size = new Size(100, 35);
            btnSave.Location = new Point(200, yPos);
            btnSave.BackColor = Color.LightGreen;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button();
            btnCancel.Text = "إلغاء";
            btnCancel.Size = new Size(100, 35);
            btnCancel.Location = new Point(80, yPos);
            btnCancel.BackColor = Color.LightCoral;
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[] {
                lblName, txtName,
                lblDescription, txtDescription,
                btnSave, btnCancel
            });
        }

        private void LoadRoleData()
        {
            string query = "SELECT role_name, role_description FROM [Role] WHERE Role_ID = @RoleID";
            
            SqlParameter[] parameters = {
                new SqlParameter("@RoleID", roleId.Value)
            };

            DataTable result = databaseService.ExecuteQuery(query, parameters);
            
            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                txtName.Text = row["role_name"].ToString();
                txtDescription.Text = row["role_description"].ToString();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    if (roleId.HasValue)
                    {
                        UpdateRole();
                    }
                    else
                    {
                        InsertRole();
                    }
                    
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"حدث خطأ أثناء الحفظ: {ex.Message}", "خطأ");
                }
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("يرجى إدخال اسم الدور", "تحذير");
                txtName.Focus();
                return false;
            }

            return true;
        }

        private void InsertRole()
        {
            string query = @"INSERT INTO [Role] (role_name, role_description, created_at)
                           VALUES (@Name, @Description, GETDATE())";

            SqlParameter[] parameters = {
                new SqlParameter("@Name", txtName.Text.Trim()),
                new SqlParameter("@Description", txtDescription.Text.Trim())
            };

            databaseService.ExecuteNonQuery(query, parameters);
        }

        private void UpdateRole()
        {
            string query = @"UPDATE [Role] SET 
                           role_name = @Name,
                           role_description = @Description
                           WHERE Role_ID = @RoleID";

            SqlParameter[] parameters = {
                new SqlParameter("@Name", txtName.Text.Trim()),
                new SqlParameter("@Description", txtDescription.Text.Trim()),
                new SqlParameter("@RoleID", roleId.Value)
            };

            databaseService.ExecuteNonQuery(query, parameters);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}