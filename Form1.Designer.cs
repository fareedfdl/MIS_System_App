namespace MIS_System_App
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnExit;
        private Label label1;
        private Label label2;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtUsername = new TextBox();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.btnExit = new Button();
            this.label1 = new Label();
            this.label2 = new Label();
            this.SuspendLayout();

            // النموذج
            this.ClientSize = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // عنوان النموذج
            this.label1 = new Label();
            this.label1.Text = "تسجيل الدخول - نظام إدارة المعلومات";
            this.label1.Font = new Font("Arial", 14, FontStyle.Bold);
            this.label1.TextAlign = ContentAlignment.MiddleCenter;
            this.label1.Location = new Point(50, 30);
            this.label1.Size = new Size(300, 30);
            this.Controls.Add(this.label1);

            // اسم المستخدم
            this.label2 = new Label();
            this.label2.Text = "اسم المستخدم:";
            this.label2.Location = new Point(250, 100);
            this.label2.Size = new Size(100, 20);
            this.Controls.Add(this.label2);

            this.txtUsername = new TextBox();
            this.txtUsername.Location = new Point(50, 100);
            this.txtUsername.Size = new Size(200, 20);
            this.txtUsername.TabIndex = 0;
            this.Controls.Add(this.txtUsername);

            // كلمة المرور
            Label label3 = new Label();
            label3.Text = "كلمة المرور:";
            label3.Location = new Point(250, 140);
            label3.Size = new Size(100, 20);
            this.Controls.Add(label3);

            this.txtPassword = new TextBox();
            this.txtPassword.Location = new Point(50, 140);
            this.txtPassword.Size = new Size(200, 20);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.TabIndex = 1;
            this.Controls.Add(this.txtPassword);

            // زر عرض بيانات الاختبار
            Button btnTestData = new Button();
            btnTestData.Text = "عرض بيانات الاختبار";
            btnTestData.Location = new Point(50, 230);
            btnTestData.Size = new Size(120, 30);
            btnTestData.BackColor = Color.LightGreen;
            btnTestData.Click += new EventHandler(this.btnTestData_Click);
            this.Controls.Add(btnTestData);

            // زر تسجيل الدخول
            this.btnLogin = new Button();
            this.btnLogin.Text = "تسجيل الدخول";
            this.btnLogin.Location = new Point(150, 190);
            this.btnLogin.Size = new Size(100, 30);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.BackColor = Color.LightBlue;
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);
            this.Controls.Add(this.btnLogin);

            // زر خروج
            this.btnExit = new Button();
            this.btnExit.Text = "خروج";
            this.btnExit.Location = new Point(50, 190);
            this.btnExit.Size = new Size(80, 30);
            this.btnExit.TabIndex = 3;
            this.btnExit.BackColor = Color.LightCoral;
            this.btnExit.Click += new EventHandler(this.btnExit_Click);
            this.Controls.Add(this.btnExit);

            this.ResumeLayout(false);
        }

        
    }
}