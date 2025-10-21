using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.HR
{
    public partial class EmployeeForm : Form
    {
        private DatabaseService databaseService;
        private int? employeeId;
        private ComboBox cmbDepartment;
        private TextBox txtFullName, txtEmail, txtPhone, txtAddress, txtSalary;
        private DateTimePicker dtpHireDate;
        private Button btnSave, btnCancel;

        public EmployeeForm(int? empId = null)
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            employeeId = empId;

            try
            {
                LoadDepartments();

                if (employeeId.HasValue)
                {
                    LoadEmployeeData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل البيانات: {ex.Message}", "خطأ");
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = employeeId.HasValue ? "تعديل بيانات الموظف" : "إضافة موظف جديد";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            CreateControls();

            this.ResumeLayout(false);
        }

        private void CreateControls()
        {
            int yPos = 20;
            int labelWidth = 120;
            int controlWidth = 300;

            // الاسم الكامل
            Label lblFullName = new Label();
            lblFullName.Text = "الاسم الكامل:";
            lblFullName.Location = new Point(350, yPos);
            lblFullName.Size = new Size(labelWidth, 25);
            lblFullName.TextAlign = ContentAlignment.MiddleLeft;

            txtFullName = new TextBox();
            txtFullName.Location = new Point(40, yPos);
            txtFullName.Size = new Size(controlWidth, 25);
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

            // الهاتف
            Label lblPhone = new Label();
            lblPhone.Text = "رقم الهاتف:";
            lblPhone.Location = new Point(350, yPos);
            lblPhone.Size = new Size(labelWidth, 25);

            txtPhone = new TextBox();
            txtPhone.Location = new Point(40, yPos);
            txtPhone.Size = new Size(controlWidth, 25);
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

            // القسم
            Label lblDepartment = new Label();
            lblDepartment.Text = "القسم:";
            lblDepartment.Location = new Point(350, yPos);
            lblDepartment.Size = new Size(labelWidth, 25);

            cmbDepartment = new ComboBox();
            cmbDepartment.Location = new Point(40, yPos);
            cmbDepartment.Size = new Size(controlWidth, 25);
            cmbDepartment.DropDownStyle = ComboBoxStyle.DropDownList;
            yPos += 40;

            // الراتب الأساسي
            Label lblSalary = new Label();
            lblSalary.Text = "الراتب الأساسي:";
            lblSalary.Location = new Point(350, yPos);
            lblSalary.Size = new Size(labelWidth, 25);

            txtSalary = new TextBox();
            txtSalary.Location = new Point(40, yPos);
            txtSalary.Size = new Size(controlWidth, 25);
            yPos += 40;

            // تاريخ التعيين
            Label lblHireDate = new Label();
            lblHireDate.Text = "تاريخ التعيين:";
            lblHireDate.Location = new Point(350, yPos);
            lblHireDate.Size = new Size(labelWidth, 25);

            dtpHireDate = new DateTimePicker();
            dtpHireDate.Location = new Point(40, yPos);
            dtpHireDate.Size = new Size(controlWidth, 25);
            dtpHireDate.Value = DateTime.Today;
            yPos += 50;

            // أزرار الحفظ والإلغاء
            btnSave = new Button();
            btnSave.Text = "حفظ";
            btnSave.Size = new Size(100, 35);
            btnSave.Location = new Point(200, yPos);
            btnSave.BackColor = Color.LightGreen;
            btnSave.Font = new Font("Arial", 10, FontStyle.Bold);
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button();
            btnCancel.Text = "إلغاء";
            btnCancel.Size = new Size(100, 35);
            btnCancel.Location = new Point(80, yPos);
            btnCancel.BackColor = Color.LightCoral;
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[] {
                lblFullName, txtFullName,
                lblEmail, txtEmail,
                lblPhone, txtPhone,
                lblAddress, txtAddress,
                lblDepartment, cmbDepartment,
                lblSalary, txtSalary,
                lblHireDate, dtpHireDate,
                btnSave, btnCancel
            });
        }

        private void LoadDepartments()
        {
            try
            {
                string query = "SELECT Dept_ID, department_name FROM Department ORDER BY department_name";
                DataTable departments = databaseService.ExecuteQuery(query);

                if (departments != null && departments.Rows.Count > 0)
                {
                    cmbDepartment.DisplayMember = "department_name";
                    cmbDepartment.ValueMember = "Dept_ID";
                    cmbDepartment.DataSource = departments;
                }
                else
                {
                    // إضافة قسم افتراضي إذا لم توجد أقسام
                    DataTable defaultDept = new DataTable();
                    defaultDept.Columns.Add("Dept_ID", typeof(int));
                    defaultDept.Columns.Add("department_name", typeof(string));
                    defaultDept.Rows.Add(1, "قسم افتراضي");

                    cmbDepartment.DisplayMember = "department_name";
                    cmbDepartment.ValueMember = "Dept_ID";
                    cmbDepartment.DataSource = defaultDept;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الأقسام: {ex.Message}", "خطأ");
            }
        }

        private void LoadEmployeeData()
        {
            try
            {
                string query = @"
                    SELECT full_name, email, phone_number, address, Dept_ID, base_salary, hire_date
                    FROM Employee WHERE Emp_ID = @EmpID";

                SqlParameter[] parameters = {
                    new SqlParameter("@EmpID", employeeId.Value)
                };

                DataTable result = databaseService.ExecuteQuery(query, parameters);

                if (result != null && result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    txtFullName.Text = row["full_name"]?.ToString() ?? "";
                    txtEmail.Text = row["email"]?.ToString() ?? "";
                    txtPhone.Text = row["phone_number"]?.ToString() ?? "";
                    txtAddress.Text = row["address"]?.ToString() ?? "";
                    txtSalary.Text = row["base_salary"]?.ToString() ?? "0";

                    if (row["hire_date"] != DBNull.Value)
                    {
                        dtpHireDate.Value = Convert.ToDateTime(row["hire_date"]);
                    }

                    if (row["Dept_ID"] != DBNull.Value && cmbDepartment.Items.Count > 0)
                    {
                        cmbDepartment.SelectedValue = Convert.ToInt32(row["Dept_ID"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل بيانات الموظف: {ex.Message}", "خطأ");
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    if (employeeId.HasValue)
                    {
                        UpdateEmployee();
                    }
                    else
                    {
                        InsertEmployee();
                    }

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"حدث خطأ أثناء الحفظ: {ex.Message}", "خطأ",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("يرجى إدخال الاسم الكامل", "تحذير",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSalary.Text) || !decimal.TryParse(txtSalary.Text, out _))
            {
                MessageBox.Show("يرجى إدخال راتب أساسي صحيح", "تحذير",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSalary.Focus();
                return false;
            }

            if (cmbDepartment.SelectedValue == null)
            {
                MessageBox.Show("يرجى اختيار قسم", "تحذير",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void InsertEmployee()
        {
            string query = @"
                INSERT INTO Employee (full_name, hire_date, Dept_ID, base_salary, phone_number, email, address, created_at)
                VALUES (@FullName, @HireDate, @DeptID, @BaseSalary, @Phone, @Email, @Address, GETDATE())";

            SqlParameter[] parameters = {
                new SqlParameter("@FullName", txtFullName.Text.Trim()),
                new SqlParameter("@HireDate", dtpHireDate.Value),
                new SqlParameter("@DeptID", cmbDepartment.SelectedValue),
                new SqlParameter("@BaseSalary", decimal.Parse(txtSalary.Text)),
                new SqlParameter("@Phone", txtPhone.Text.Trim()),
                new SqlParameter("@Email", txtEmail.Text.Trim()),
                new SqlParameter("@Address", txtAddress.Text.Trim())
            };

            int result = databaseService.ExecuteNonQuery(query, parameters);

            if (result > 0)
            {
                MessageBox.Show("تم إضافة الموظف بنجاح", "نجاح");
            }
        }

        private void UpdateEmployee()
        {
            string query = @"
                UPDATE Employee 
                SET full_name = @FullName,
                    hire_date = @HireDate,
                    Dept_ID = @DeptID,
                    base_salary = @BaseSalary,
                    phone_number = @Phone,
                    email = @Email,
                    address = @Address
                WHERE Emp_ID = @EmpID";

            SqlParameter[] parameters = {
                new SqlParameter("@FullName", txtFullName.Text.Trim()),
                new SqlParameter("@HireDate", dtpHireDate.Value),
                new SqlParameter("@DeptID", cmbDepartment.SelectedValue),
                new SqlParameter("@BaseSalary", decimal.Parse(txtSalary.Text)),
                new SqlParameter("@Phone", txtPhone.Text.Trim()),
                new SqlParameter("@Email", txtEmail.Text.Trim()),
                new SqlParameter("@Address", txtAddress.Text.Trim()),
                new SqlParameter("@EmpID", employeeId.Value)
            };

            int result = databaseService.ExecuteNonQuery(query, parameters);

            if (result > 0)
            {
                MessageBox.Show("تم تحديث بيانات الموظف بنجاح", "نجاح");
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}