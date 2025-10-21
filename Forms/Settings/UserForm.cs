using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.Settings
{
    public partial class UserForm : Form
    {
        private DatabaseService databaseService;
        private UserService userService;
        private int? userId;
        private TextBox txtUsername, txtEmail, txtPassword, txtConfirmPassword;
        private ComboBox cmbRole;
        private CheckBox chkActive;
        private Button btnSave, btnCancel;
        private Control lblPassword;
        private Control lblConfirmPassword;

        public UserForm(int? uId = null)
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            userService = new UserService();
            userId = uId;

            if (userId.HasValue)
            {
                LoadUserData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = userId.HasValue ? "تعديل بيانات المستخدم" : "إضافة مستخدم جديد";
            this.Size = new Size(500, 450);
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

            // اسم المستخدم
            Label lblUsername = new Label();
            lblUsername.Text = "اسم المستخدم:";
            lblUsername.Location = new Point(350, yPos);
            lblUsername.Size = new Size(labelWidth, 25);

            txtUsername = new TextBox();
            txtUsername.Location = new Point(40, yPos);
            txtUsername.Size = new Size(controlWidth, 25);
            yPos += 40;

            // البريد الإلكتروني
            Label lblEmail = new Label();
            lblEmail.Text = "البريد الإلكتروني:";
            lblEmail.Location = new Point(350, yPos);
            lblEmail.Size = new Size(labelWidth, 25);

            txtEmail = new TextBox();
            txtEmail.Location = new Point(40, yPos);
            txtEmail.Size = new Size(controlWidth, 25);
            yPos += 40;

            // كلمة المرور (للمستخدمين الجدد فقط)
            if (!userId.HasValue)
            {
                Label lblPassword = new Label();
                lblPassword.Text = "كلمة المرور:";
                lblPassword.Location = new Point(350, yPos);
                lblPassword.Size = new Size(labelWidth, 25);

                txtPassword = new TextBox();
                txtPassword.Location = new Point(40, yPos);
                txtPassword.Size = new Size(controlWidth, 25);
                txtPassword.PasswordChar = '*';
                yPos += 40;

                Label lblConfirmPassword = new Label();
                lblConfirmPassword.Text = "تأكيد كلمة المرور:";
                lblConfirmPassword.Location = new Point(350, yPos);
                lblConfirmPassword.Size = new Size(labelWidth, 25);

                txtConfirmPassword = new TextBox();
                txtConfirmPassword.Location = new Point(40, yPos);
                txtConfirmPassword.Size = new Size(controlWidth, 25);
                txtConfirmPassword.PasswordChar = '*';
                yPos += 40;
            }

            // الدور
            Label lblRole = new Label();
            lblRole.Text = "الدور:";
            lblRole.Location = new Point(350, yPos);
            lblRole.Size = new Size(labelWidth, 25);

            cmbRole = new ComboBox();
            cmbRole.Location = new Point(40, yPos);
            cmbRole.Size = new Size(controlWidth, 25);
            cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            LoadRoles();
            yPos += 40;

            // الحالة
            chkActive = new CheckBox();
            chkActive.Text = "المستخدم نشط";
            chkActive.Location = new Point(40, yPos);
            chkActive.Size = new Size(150, 25);
            chkActive.Checked = true;
            yPos += 50;

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

            var controls = new System.Collections.Generic.List<Control>
            {
                lblUsername, txtUsername,
                lblEmail, txtEmail,
                lblRole, cmbRole,
                chkActive,
                btnSave, btnCancel
            };

            if (!userId.HasValue)
            {
                controls.AddRange(new Control[] {
                    lblPassword, txtPassword,
                    lblConfirmPassword, txtConfirmPassword
                });
            }

            this.Controls.AddRange(controls.ToArray());
        }

        private void LoadRoles()
        {
            string query = "SELECT Role_ID, role_name FROM [Role] ORDER BY role_name";
            DataTable roles = databaseService.ExecuteQuery(query);

            cmbRole.DisplayMember = "role_name";
            cmbRole.ValueMember = "Role_ID";
            cmbRole.DataSource = roles;
        }

        private void LoadUserData()
        {
            string query = @"
                SELECT u.username, u.email, u.is_active, r.Role_ID
                FROM [User] u
                INNER JOIN User_Role ur ON u.User_ID = ur.User_ID
                INNER JOIN [Role] r ON ur.Role_ID = r.Role_ID
                WHERE u.User_ID = @UserID";

            SqlParameter[] parameters = {
                new SqlParameter("@UserID", userId.Value)
            };

            DataTable result = databaseService.ExecuteQuery(query, parameters);

            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                txtUsername.Text = row["username"].ToString();
                txtEmail.Text = row["email"].ToString();
                chkActive.Checked = Convert.ToBoolean(row["is_active"]);
                cmbRole.SelectedValue = Convert.ToInt32(row["Role_ID"]);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    if (userId.HasValue)
                    {
                        UpdateUser();
                    }
                    else
                    {
                        InsertUser();
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
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("يرجى إدخال اسم المستخدم", "تحذير");
                txtUsername.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("يرجى إدخال البريد الإلكتروني", "تحذير");
                txtEmail.Focus();
                return false;
            }

            if (!userId.HasValue)
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("يرجى إدخال كلمة المرور", "تحذير");
                    txtPassword.Focus();
                    return false;
                }

                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    MessageBox.Show("كلمة المرور وتأكيدها غير متطابقين", "تحذير");
                    txtConfirmPassword.Focus();
                    return false;
                }

                if (txtPassword.Text.Length < 6)
                {
                    MessageBox.Show("كلمة المرور يجب أن تكون 6 أحرف على الأقل", "تحذير");
                    txtPassword.Focus();
                    return false;
                }
            }

            if (cmbRole.SelectedValue == null)
            {
                MessageBox.Show("يرجى اختيار دور للمستخدم", "تحذير");
                return false;
            }

            return true;
        }

        private void InsertUser()
        {
            string encryptedPassword = userService.EncryptPassword(txtPassword.Text);

            // إدخال المستخدم
            string userQuery = @"
                INSERT INTO [User] (username, email, encrypted_password, Org_ID, is_active, created_at)
                VALUES (@Username, @Email, @Password, 1, @Active, GETDATE());
                SELECT SCOPE_IDENTITY();";

            SqlParameter[] userParameters = {
                new SqlParameter("@Username", txtUsername.Text.Trim()),
                new SqlParameter("@Email", txtEmail.Text.Trim()),
                new SqlParameter("@Password", encryptedPassword),
                new SqlParameter("@Active", chkActive.Checked)
            };

            object userIdResult = databaseService.ExecuteScalar(userQuery, userParameters);

            if (userIdResult != null)
            {
                int newUserId = Convert.ToInt32(userIdResult);

                // ربط المستخدم بالدور
                string roleQuery = "INSERT INTO User_Role (User_ID, Role_ID, assigned_date) VALUES (@UserID, @RoleID, GETDATE())";
                SqlParameter[] roleParameters = {
                    new SqlParameter("@UserID", newUserId),
                    new SqlParameter("@RoleID", cmbRole.SelectedValue)
                };

                databaseService.ExecuteNonQuery(roleQuery, roleParameters);
            }
        }

        private void UpdateUser()
        {
            // تحديث بيانات المستخدم
            string userQuery = @"
                UPDATE [User] 
                SET username = @Username,
                    email = @Email,
                    is_active = @Active
                WHERE User_ID = @UserID";

            SqlParameter[] userParameters = {
                new SqlParameter("@Username", txtUsername.Text.Trim()),
                new SqlParameter("@Email", txtEmail.Text.Trim()),
                new SqlParameter("@Active", chkActive.Checked),
                new SqlParameter("@UserID", userId.Value)
            };

            databaseService.ExecuteNonQuery(userQuery, userParameters);

            // تحديث الدور
            string roleQuery = @"
                UPDATE User_Role 
                SET Role_ID = @RoleID
                WHERE User_ID = @UserID";

            SqlParameter[] roleParameters = {
                new SqlParameter("@RoleID", cmbRole.SelectedValue),
                new SqlParameter("@UserID", userId.Value)
            };

            databaseService.ExecuteNonQuery(roleQuery, roleParameters);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}