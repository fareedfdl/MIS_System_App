
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;
using MIS_System_App.Models;

namespace MIS_System_App.Forms.Settings
{
    public partial class ChangePasswordForm : Form
    {
        private User currentUser;
        private UserService userService;
        private TextBox txtCurrentPassword, txtNewPassword, txtConfirmPassword;
        private Button btnSave, btnCancel;

        public ChangePasswordForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            userService = new UserService();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "تغيير كلمة المرور";
            this.Size = new Size(400, 300);
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
            titleLabel.Text = "تغيير كلمة المرور";
            titleLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 50;
            titleLabel.BackColor = Color.LightBlue;

            // لوحة المحتوى
            Panel contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Padding = new Padding(20);

            int yPos = 20;

            // كلمة المرور الحالية
            Label lblCurrent = new Label();
            lblCurrent.Text = "كلمة المرور الحالية:";
            lblCurrent.Location = new Point(250, yPos);
            lblCurrent.Size = new Size(120, 25);

            txtCurrentPassword = new TextBox();
            txtCurrentPassword.Location = new Point(50, yPos);
            txtCurrentPassword.Size = new Size(190, 25);
            txtCurrentPassword.PasswordChar = '*';
            yPos += 40;

            // كلمة المرور الجديدة
            Label lblNew = new Label();
            lblNew.Text = "كلمة المرور الجديدة:";
            lblNew.Location = new Point(250, yPos);
            lblNew.Size = new Size(120, 25);

            txtNewPassword = new TextBox();
            txtNewPassword.Location = new Point(50, yPos);
            txtNewPassword.Size = new Size(190, 25);
            txtNewPassword.PasswordChar = '*';
            yPos += 40;

            // تأكيد كلمة المرور
            Label lblConfirm = new Label();
            lblConfirm.Text = "تأكيد كلمة المرور:";
            lblConfirm.Location = new Point(250, yPos);
            lblConfirm.Size = new Size(120, 25);

            txtConfirmPassword = new TextBox();
            txtConfirmPassword.Location = new Point(50, yPos);
            txtConfirmPassword.Size = new Size(190, 25);
            txtConfirmPassword.PasswordChar = '*';
            yPos += 50;

            // متطلبات كلمة المرور
            Label lblRequirements = new Label();
            lblRequirements.Text = "يجب أن تحتوي كلمة المرور على:\n• 8 أحرف على الأقل\n• حرف كبير وصغير\n• رقم واحد على الأقل\n• رمز خاص واحد على الأقل";
            lblRequirements.Location = new Point(50, yPos);
            lblRequirements.Size = new Size(300, 80);
            lblRequirements.Font = new Font("Arial", 9, FontStyle.Italic);
            lblRequirements.ForeColor = Color.DarkGray;
            yPos += 90;

            // أزرار الحفظ والإلغاء
            btnSave = new Button();
            btnSave.Text = "حفظ التغييرات";
            btnSave.Size = new Size(120, 35);
            btnSave.Location = new Point(150, yPos);
            btnSave.BackColor = Color.LightGreen;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button();
            btnCancel.Text = "إلغاء";
            btnCancel.Size = new Size(80, 35);
            btnCancel.Location = new Point(50, yPos);
            btnCancel.BackColor = Color.LightCoral;
            btnCancel.Click += BtnCancel_Click;

            contentPanel.Controls.AddRange(new Control[] {
                lblCurrent, txtCurrentPassword,
                lblNew, txtNewPassword,
                lblConfirm, txtConfirmPassword,
                lblRequirements,
                btnSave, btnCancel
            });

            this.Controls.AddRange(new Control[] { contentPanel, titleLabel });
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    // التحقق من كلمة المرور الحالية
                    User user = userService.Login(currentUser.Username, txtCurrentPassword.Text);
                    if (user == null)
                    {
                        MessageBox.Show("كلمة المرور الحالية غير صحيحة", "خطأ");
                        return;
                    }

                    // تحديث كلمة المرور
                    string encryptedPassword = userService.EncryptPassword(txtNewPassword.Text);
                    
                    string query = "UPDATE [User] SET encrypted_password = @NewPassword WHERE User_ID = @UserID";
                    SqlParameter[] parameters = {
                        new SqlParameter("@NewPassword", encryptedPassword),
                        new SqlParameter("@UserID", currentUser.User_ID)
                    };

                    DatabaseService dbService = new DatabaseService();
                    int rowsAffected = dbService.ExecuteNonQuery(query, parameters);

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("تم تغيير كلمة المرور بنجاح", "نجاح");
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("فشل في تغيير كلمة المرور", "خطأ");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"حدث خطأ أثناء تغيير كلمة المرور: {ex.Message}", "خطأ");
                }
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(txtCurrentPassword.Text))
            {
                MessageBox.Show("يرجى إدخال كلمة المرور الحالية", "تحذير");
                txtCurrentPassword.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtNewPassword.Text))
            {
                MessageBox.Show("يرجى إدخال كلمة المرور الجديدة", "تحذير");
                txtNewPassword.Focus();
                return false;
            }

            if (txtNewPassword.Text.Length < 8)
            {
                MessageBox.Show("كلمة المرور يجب أن تكون 8 أحرف على الأقل", "تحذير");
                txtNewPassword.Focus();
                return false;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("كلمة المرور الجديدة وتأكيدها غير متطابقين", "تحذير");
                txtConfirmPassword.Focus();
                return false;
            }

            // التحقق من قوة كلمة المرور
            if (!IsPasswordStrong(txtNewPassword.Text))
            {
                MessageBox.Show("كلمة المرور ضعيفة. يجب أن تحتوي على حروف كبيرة وصغيرة وأرقام ورموز", "تحذير");
                txtNewPassword.Focus();
                return false;
            }

            return true;
        }

        private bool IsPasswordStrong(string password)
        {
            // التحقق من وجود حرف كبير
            bool hasUpper = false;
            // التحقق من وجود حرف صغير
            bool hasLower = false;
            // التحقق من وجود رقم
            bool hasDigit = false;
            // التحقق من وجود رمز خاص
            bool hasSpecial = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                if (char.IsLower(c)) hasLower = true;
                if (char.IsDigit(c)) hasDigit = true;
                if (char.IsPunctuation(c) || char.IsSymbol(c)) hasSpecial = true;
            }

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
