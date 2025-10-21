using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.HR
{
    public partial class PayrollForm : Form
    {
        private DatabaseService databaseService;
        private HRService hrService;
        private ComboBox cmbEmployee;
        private TextBox txtTotalSalary, txtTaxes, txtBonuses, txtDeductions, txtNetSalary;
        private DateTimePicker dtpPaymentDate;
        private Button btnCalculate, btnSave, btnCancel;

        public PayrollForm()
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            hrService = new HRService();
            LoadEmployees();
            CalculateNetSalary();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "تسجيل راتب موظف";
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

            // الموظف
            Label lblEmployee = new Label();
            lblEmployee.Text = "الموظف:";
            lblEmployee.Location = new Point(350, yPos);
            lblEmployee.Size = new Size(labelWidth, 25);

            cmbEmployee = new ComboBox();
            cmbEmployee.Location = new Point(40, yPos);
            cmbEmployee.Size = new Size(controlWidth, 25);
            cmbEmployee.DropDownStyle = ComboBoxStyle.DropDownList;
            yPos += 40;

            // تاريخ الدفع
            Label lblPaymentDate = new Label();
            lblPaymentDate.Text = "تاريخ الدفع:";
            lblPaymentDate.Location = new Point(350, yPos);
            lblPaymentDate.Size = new Size(labelWidth, 25);

            dtpPaymentDate = new DateTimePicker();
            dtpPaymentDate.Location = new Point(40, yPos);
            dtpPaymentDate.Size = new Size(controlWidth, 25);
            dtpPaymentDate.Value = DateTime.Today;
            yPos += 40;

            // الراتب الإجمالي
            Label lblTotalSalary = new Label();
            lblTotalSalary.Text = "الراتب الإجمالي:";
            lblTotalSalary.Location = new Point(350, yPos);
            lblTotalSalary.Size = new Size(labelWidth, 25);

            txtTotalSalary = new TextBox();
            txtTotalSalary.Location = new Point(40, yPos);
            txtTotalSalary.Size = new Size(controlWidth, 25);
            txtTotalSalary.TextChanged += SalaryFields_TextChanged;
            yPos += 40;

            // الضرائب
            Label lblTaxes = new Label();
            lblTaxes.Text = "الضرائب:";
            lblTaxes.Location = new Point(350, yPos);
            lblTaxes.Size = new Size(labelWidth, 25);

            txtTaxes = new TextBox();
            txtTaxes.Location = new Point(40, yPos);
            txtTaxes.Size = new Size(controlWidth, 25);
            txtTaxes.Text = "0";
            txtTaxes.TextChanged += SalaryFields_TextChanged;
            yPos += 40;

            // المكافآت
            Label lblBonuses = new Label();
            lblBonuses.Text = "المكافآت:";
            lblBonuses.Location = new Point(350, yPos);
            lblBonuses.Size = new Size(labelWidth, 25);

            txtBonuses = new TextBox();
            txtBonuses.Location = new Point(40, yPos);
            txtBonuses.Size = new Size(controlWidth, 25);
            txtBonuses.Text = "0";
            txtBonuses.TextChanged += SalaryFields_TextChanged;
            yPos += 40;

            // الخصومات
            Label lblDeductions = new Label();
            lblDeductions.Text = "الخصومات:";
            lblDeductions.Location = new Point(350, yPos);
            lblDeductions.Size = new Size(labelWidth, 25);

            txtDeductions = new TextBox();
            txtDeductions.Location = new Point(40, yPos);
            txtDeductions.Size = new Size(controlWidth, 25);
            txtDeductions.Text = "0";
            txtDeductions.TextChanged += SalaryFields_TextChanged;
            yPos += 40;

            // صافي الراتب
            Label lblNetSalary = new Label();
            lblNetSalary.Text = "صافي الراتب:";
            lblNetSalary.Location = new Point(350, yPos);
            lblNetSalary.Size = new Size(labelWidth, 25);
            lblNetSalary.Font = new Font("Arial", 10, FontStyle.Bold);

            txtNetSalary = new TextBox();
            txtNetSalary.Location = new Point(40, yPos);
            txtNetSalary.Size = new Size(controlWidth, 25);
            txtNetSalary.ReadOnly = true;
            txtNetSalary.BackColor = Color.LightYellow;
            txtNetSalary.Font = new Font("Arial", 10, FontStyle.Bold);
            yPos += 50;

            // أزرار التحكم
            btnCalculate = new Button();
            btnCalculate.Text = "حساب الصافي";
            btnCalculate.Size = new Size(100, 35);
            btnCalculate.Location = new Point(280, yPos);
            btnCalculate.BackColor = Color.LightBlue;
            btnCalculate.Click += BtnCalculate_Click;

            btnSave = new Button();
            btnSave.Text = "حفظ الراتب";
            btnSave.Size = new Size(100, 35);
            btnSave.Location = new Point(160, yPos);
            btnSave.BackColor = Color.LightGreen;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button();
            btnCancel.Text = "إلغاء";
            btnCancel.Size = new Size(100, 35);
            btnCancel.Location = new Point(40, yPos);
            btnCancel.BackColor = Color.LightCoral;
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[] {
                lblEmployee, cmbEmployee,
                lblPaymentDate, dtpPaymentDate,
                lblTotalSalary, txtTotalSalary,
                lblTaxes, txtTaxes,
                lblBonuses, txtBonuses,
                lblDeductions, txtDeductions,
                lblNetSalary, txtNetSalary,
                btnCalculate, btnSave, btnCancel
            });
        }

        private void LoadEmployees()
        {
            string query = "SELECT Emp_ID, full_name FROM Employee ORDER BY full_name";
            DataTable employees = databaseService.ExecuteQuery(query);
            
            cmbEmployee.DisplayMember = "full_name";
            cmbEmployee.ValueMember = "Emp_ID";
            cmbEmployee.DataSource = employees;
        }

        private void SalaryFields_TextChanged(object sender, EventArgs e)
        {
            CalculateNetSalary();
        }

        private void CalculateNetSalary()
        {
            try
            {
                decimal totalSalary = decimal.Parse(txtTotalSalary.Text.Length > 0 ? txtTotalSalary.Text : "0");
                decimal taxes = decimal.Parse(txtTaxes.Text.Length > 0 ? txtTaxes.Text : "0");
                decimal bonuses = decimal.Parse(txtBonuses.Text.Length > 0 ? txtBonuses.Text : "0");
                decimal deductions = decimal.Parse(txtDeductions.Text.Length > 0 ? txtDeductions.Text : "0");

                decimal netSalary = totalSalary + bonuses - taxes - deductions;
                txtNetSalary.Text = netSalary.ToString("F2");
            }
            catch
            {
                txtNetSalary.Text = "0";
            }
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            CalculateNetSalary();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    int empId = Convert.ToInt32(cmbEmployee.SelectedValue);
                    DateTime paymentDate = dtpPaymentDate.Value;
                    decimal totalSalary = decimal.Parse(txtTotalSalary.Text);
                    decimal taxes = decimal.Parse(txtTaxes.Text);
                    decimal bonuses = decimal.Parse(txtBonuses.Text);
                    decimal deductions = decimal.Parse(txtDeductions.Text);

                    bool success = hrService.RecordPayroll(empId, paymentDate, totalSalary, taxes, bonuses, deductions);

                    if (success)
                    {
                        MessageBox.Show("تم تسجيل الراتب بنجاح", "نجاح");
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("فشل في تسجيل الراتب", "خطأ");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"حدث خطأ أثناء الحفظ: {ex.Message}", "خطأ");
                }
            }
        }

        private bool ValidateForm()
        {
            if (cmbEmployee.SelectedValue == null)
            {
                MessageBox.Show("يرجى اختيار موظف", "تحذير");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTotalSalary.Text) || !decimal.TryParse(txtTotalSalary.Text, out decimal salary) || salary <= 0)
            {
                MessageBox.Show("يرجى إدخال راتب إجمالي صحيح", "تحذير");
                txtTotalSalary.Focus();
                return false;
            }

            return true;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}