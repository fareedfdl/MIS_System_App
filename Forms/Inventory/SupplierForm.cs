using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.Inventory
{
    public partial class SupplierForm : Form
    {
        private DatabaseService databaseService;
        private int? supplierId;
        private TextBox txtName, txtContactInfo, txtPhone, txtEmail, txtAddress;
        private Button btnSave, btnCancel;

        public SupplierForm(int? supId = null)
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            supplierId = supId;
            
            if (supplierId.HasValue)
            {
                LoadSupplierData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = supplierId.HasValue ? "تعديل بيانات المورد" : "إضافة مورد جديد";
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

            // اسم المورد
            Label lblName = new Label();
            lblName.Text = "اسم المورد:";
            lblName.Location = new Point(350, yPos);
            lblName.Size = new Size(labelWidth, 25);

            txtName = new TextBox();
            txtName.Location = new Point(40, yPos);
            txtName.Size = new Size(controlWidth, 25);
            yPos += 40;

            // معلومات الاتصال
            Label lblContactInfo = new Label();
            lblContactInfo.Text = "معلومات الاتصال:";
            lblContactInfo.Location = new Point(350, yPos);
            lblContactInfo.Size = new Size(labelWidth, 25);

            txtContactInfo = new TextBox();
            txtContactInfo.Location = new Point(40, yPos);
            txtContactInfo.Size = new Size(controlWidth, 25);
            txtContactInfo.Multiline = true;
            txtContactInfo.Height = 40;
            yPos += 60;

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

            // العنوان
            Label lblAddress = new Label();
            lblAddress.Text = "العنوان:";
            lblAddress.Location = new Point(350, yPos);
            lblAddress.Size = new Size(labelWidth, 25);

            txtAddress = new TextBox();
            txtAddress.Location = new Point(40, yPos);
            txtAddress.Size = new Size(controlWidth, 25);
            txtAddress.Multiline = true;
            txtAddress.Height = 40;
            yPos += 60;

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
                lblContactInfo, txtContactInfo,
                lblPhone, txtPhone,
                lblEmail, txtEmail,
                lblAddress, txtAddress,
                btnSave, btnCancel
            });
        }

        private void LoadSupplierData()
        {
            string query = @"SELECT supplier_name, contact_info, phone_number, email, address
                           FROM Supplier WHERE Supplier_ID = @SupplierID";
            
            SqlParameter[] parameters = {
                new SqlParameter("@SupplierID", supplierId.Value)
            };

            DataTable result = databaseService.ExecuteQuery(query, parameters);
            
            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                txtName.Text = row["supplier_name"].ToString();
                txtContactInfo.Text = row["contact_info"].ToString();
                txtPhone.Text = row["phone_number"].ToString();
                txtEmail.Text = row["email"].ToString();
                txtAddress.Text = row["address"].ToString();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    if (supplierId.HasValue)
                    {
                        UpdateSupplier();
                    }
                    else
                    {
                        InsertSupplier();
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
                MessageBox.Show("يرجى إدخال اسم المورد", "تحذير");
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

        private void InsertSupplier()
        {
            string query = @"INSERT INTO Supplier (supplier_name, contact_info, phone_number, email, address, Org_ID, created_at)
                           VALUES (@Name, @ContactInfo, @Phone, @Email, @Address, 1, GETDATE())";

            SqlParameter[] parameters = {
                new SqlParameter("@Name", txtName.Text.Trim()),
                new SqlParameter("@ContactInfo", txtContactInfo.Text.Trim()),
                new SqlParameter("@Phone", txtPhone.Text.Trim()),
                new SqlParameter("@Email", txtEmail.Text.Trim()),
                new SqlParameter("@Address", txtAddress.Text.Trim())
            };

            databaseService.ExecuteNonQuery(query, parameters);
        }

        private void UpdateSupplier()
        {
            string query = @"UPDATE Supplier SET 
                           supplier_name = @Name,
                           contact_info = @ContactInfo,
                           phone_number = @Phone,
                           email = @Email,
                           address = @Address
                           WHERE Supplier_ID = @SupplierID";

            SqlParameter[] parameters = {
                new SqlParameter("@Name", txtName.Text.Trim()),
                new SqlParameter("@ContactInfo", txtContactInfo.Text.Trim()),
                new SqlParameter("@Phone", txtPhone.Text.Trim()),
                new SqlParameter("@Email", txtEmail.Text.Trim()),
                new SqlParameter("@Address", txtAddress.Text.Trim()),
                new SqlParameter("@SupplierID", supplierId.Value)
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