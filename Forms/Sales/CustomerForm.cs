
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.Sales
{
    public partial class CustomerForm : Form
    {
        private DatabaseService databaseService;
        private int? customerId;
        private TextBox txtName, txtAddress, txtPhone, txtEmail;
        private ComboBox cmbCurrency, cmbType;
        private Button btnSave, btnCancel;

        public CustomerForm(int? custId = null)
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            customerId = custId;
            
            if (customerId.HasValue)
            {
                LoadCustomerData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = customerId.HasValue ? "تعديل بيانات العميل" : "إضافة عميل جديد";
            this.Size = new Size(500, 400);
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

            // اسم العميل
            Label lblName = new Label();
            lblName.Text = "اسم العميل:";
            lblName.Location = new Point(350, yPos);
            lblName.Size = new Size(labelWidth, 25);

            txtName = new TextBox();
            txtName.Location = new Point(40, yPos);
            txtName.Size = new Size(controlWidth, 25);
            yPos += 40;

            // العنوان
            Label lblAddress = new Label();
            lblAddress.Text = "العنوان:";
            lblAddress.Location = new Point(350, yPos);
            lblAddress.Size = new Size(labelWidth, 25);

            txtAddress = new TextBox();
            txtAddress.Location = new Point(40, yPos);
            txtAddress.Size = new Size(controlWidth, 25);
            txtAddress.Multiline = true;
            txtAddress.Height = 60;
            yPos += 80;

            // الهاتف
            Label lblPhone = new Label();
            lblPhone.Text = "رقم الهاتف:";
            lblPhone.Location = new Point(350, yPos);
            lblPhone.Size = new Size(labelWidth, 25);

            txtPhone = new TextBox();
            txtPhone.Location = new Point(40, yPos);
            txtPhone.Size = new Size(controlWidth, 25);
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

            // العملة المفضلة
            Label lblCurrency = new Label();
            lblCurrency.Text = "العملة المفضلة:";
            lblCurrency.Location = new Point(350, yPos);
            lblCurrency.Size = new Size(labelWidth, 25);

            cmbCurrency = new ComboBox();
            cmbCurrency.Location = new Point(40, yPos);
            cmbCurrency.Size = new Size(controlWidth, 25);
            cmbCurrency.Items.AddRange(new string[] { "SAR", "USD", "EUR", "YER" });
            cmbCurrency.SelectedIndex = 0;
            yPos += 40;

            // نوع العميل
            Label lblType = new Label();
            lblType.Text = "نوع العميل:";
            lblType.Location = new Point(350, yPos);
            lblType.Size = new Size(labelWidth, 25);

            cmbType = new ComboBox();
            cmbType.Location = new Point(40, yPos);
            cmbType.Size = new Size(controlWidth, 25);
            cmbType.Items.AddRange(new string[] { "Individual", "Company" });
            cmbType.SelectedIndex = 0;
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

            this.Controls.AddRange(new Control[] {
                lblName, txtName,
                lblAddress, txtAddress,
                lblPhone, txtPhone,
                lblEmail, txtEmail,
                lblCurrency, cmbCurrency,
                lblType, cmbType,
                btnSave, btnCancel
            });
        }

        private void LoadCustomerData()
        {
            string query = @"SELECT customer_name, customer_address, phone_number, email, 
                           preferred_currency, customer_type
                           FROM Customer WHERE Customer_ID = @CustomerID";
            
            SqlParameter[] parameters = {
                new SqlParameter("@CustomerID", customerId.Value)
            };

            DataTable result = databaseService.ExecuteQuery(query, parameters);
            
            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                txtName.Text = row["customer_name"].ToString();
                txtAddress.Text = row["customer_address"].ToString();
                txtPhone.Text = row["phone_number"].ToString();
                txtEmail.Text = row["email"].ToString();
                cmbCurrency.Text = row["preferred_currency"].ToString();
                cmbType.Text = row["customer_type"].ToString();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    if (customerId.HasValue)
                    {
                        UpdateCustomer();
                    }
                    else
                    {
                        InsertCustomer();
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
                MessageBox.Show("يرجى إدخال اسم العميل", "تحذير");
                txtName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("يرجى إدخال رقم الهاتف", "تحذير");
                txtPhone.Focus();
                return false;
            }

            return true;
        }

        private void InsertCustomer()
        {
            string query = @"INSERT INTO Customer (customer_name, customer_address, phone_number, email, 
                           preferred_currency, customer_type, Org_ID, created_at)
                           VALUES (@Name, @Address, @Phone, @Email, @Currency, @Type, 1, GETDATE())";

            SqlParameter[] parameters = {
                new SqlParameter("@Name", txtName.Text.Trim()),
                new SqlParameter("@Address", txtAddress.Text.Trim()),
                new SqlParameter("@Phone", txtPhone.Text.Trim()),
                new SqlParameter("@Email", txtEmail.Text.Trim()),
                new SqlParameter("@Currency", cmbCurrency.Text),
                new SqlParameter("@Type", cmbType.Text)
            };

            databaseService.ExecuteNonQuery(query, parameters);
        }

        private void UpdateCustomer()
        {
            string query = @"UPDATE Customer SET 
                           customer_name = @Name,
                           customer_address = @Address,
                           phone_number = @Phone,
                           email = @Email,
                           preferred_currency = @Currency,
                           customer_type = @Type
                           WHERE Customer_ID = @CustomerID";

            SqlParameter[] parameters = {
                new SqlParameter("@Name", txtName.Text.Trim()),
                new SqlParameter("@Address", txtAddress.Text.Trim()),
                new SqlParameter("@Phone", txtPhone.Text.Trim()),
                new SqlParameter("@Email", txtEmail.Text.Trim()),
                new SqlParameter("@Currency", cmbCurrency.Text),
                new SqlParameter("@Type", cmbType.Text),
                new SqlParameter("@CustomerID", customerId.Value)
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
