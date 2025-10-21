using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.Accounting
{
    public partial class AccountingForm : Form
    {
        private DatabaseService databaseService;
        private AccountingService accountingService;
        private ComboBox cmbDebitAccount, cmbCreditAccount;
        private TextBox txtAmount, txtDescription, txtReference;
        private DateTimePicker dtpTransactionDate;
        private Button btnSave, btnCancel;

        public AccountingForm()
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            accountingService = new AccountingService();
            LoadAccounts();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "تسجيل معاملة محاسبية";
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

            // تاريخ المعاملة
            Label lblDate = new Label();
            lblDate.Text = "تاريخ المعاملة:";
            lblDate.Location = new Point(350, yPos);
            lblDate.Size = new Size(labelWidth, 25);

            dtpTransactionDate = new DateTimePicker();
            dtpTransactionDate.Location = new Point(40, yPos);
            dtpTransactionDate.Size = new Size(controlWidth, 25);
            dtpTransactionDate.Value = DateTime.Today;
            yPos += 40;

            // الحساب المدين
            Label lblDebit = new Label();
            lblDebit.Text = "الحساب المدين:";
            lblDebit.Location = new Point(350, yPos);
            lblDebit.Size = new Size(labelWidth, 25);

            cmbDebitAccount = new ComboBox();
            cmbDebitAccount.Location = new Point(40, yPos);
            cmbDebitAccount.Size = new Size(controlWidth, 25);
            cmbDebitAccount.DropDownStyle = ComboBoxStyle.DropDownList;
            yPos += 40;

            // الحساب الدائن
            Label lblCredit = new Label();
            lblCredit.Text = "الحساب الدائن:";
            lblCredit.Location = new Point(350, yPos);
            lblCredit.Size = new Size(labelWidth, 25);

            cmbCreditAccount = new ComboBox();
            cmbCreditAccount.Location = new Point(40, yPos);
            cmbCreditAccount.Size = new Size(controlWidth, 25);
            cmbCreditAccount.DropDownStyle = ComboBoxStyle.DropDownList;
            yPos += 40;

            // المبلغ
            Label lblAmount = new Label();
            lblAmount.Text = "المبلغ:";
            lblAmount.Location = new Point(350, yPos);
            lblAmount.Size = new Size(labelWidth, 25);

            txtAmount = new TextBox();
            txtAmount.Location = new Point(40, yPos);
            txtAmount.Size = new Size(controlWidth, 25);
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
            txtDescription.Height = 60;
            yPos += 80;

            // رقم المرجع
            Label lblReference = new Label();
            lblReference.Text = "رقم المرجع:";
            lblReference.Location = new Point(350, yPos);
            lblReference.Size = new Size(labelWidth, 25);

            txtReference = new TextBox();
            txtReference.Location = new Point(40, yPos);
            txtReference.Size = new Size(controlWidth, 25);
            yPos += 50;

            // أزرار الحفظ والإلغاء
            btnSave = new Button();
            btnSave.Text = "حفظ المعاملة";
            btnSave.Size = new Size(120, 35);
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
                lblDate, dtpTransactionDate,
                lblDebit, cmbDebitAccount,
                lblCredit, cmbCreditAccount,
                lblAmount, txtAmount,
                lblDescription, txtDescription,
                lblReference, txtReference,
                btnSave, btnCancel
            });
        }

        private void LoadAccounts()
        {
            string query = "SELECT Account_ID, account_name + ' - ' + account_type as display_name FROM Account WHERE is_active = 1 ORDER BY account_name";
            DataTable accounts = databaseService.ExecuteQuery(query);
            
            cmbDebitAccount.DisplayMember = "display_name";
            cmbDebitAccount.ValueMember = "Account_ID";
            cmbDebitAccount.DataSource = accounts;

            cmbCreditAccount.DisplayMember = "display_name";
            cmbCreditAccount.ValueMember = "Account_ID";
            cmbCreditAccount.DataSource = accounts.Copy();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    int debitAccountId = Convert.ToInt32(cmbDebitAccount.SelectedValue);
                    int creditAccountId = Convert.ToInt32(cmbCreditAccount.SelectedValue);
                    decimal amount = decimal.Parse(txtAmount.Text);
                    string description = txtDescription.Text.Trim();
                    string reference = txtReference.Text.Trim();

                    bool success = accountingService.RecordTransaction(
                        debitAccountId, creditAccountId, amount, description, reference);

                    if (success)
                    {
                        MessageBox.Show("تم تسجيل المعاملة المحاسبية بنجاح", "نجاح");
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("فشل في تسجيل المعاملة", "خطأ");
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
            if (cmbDebitAccount.SelectedValue == null || cmbCreditAccount.SelectedValue == null)
            {
                MessageBox.Show("يرجى اختيار الحسابات", "تحذير");
                return false;
            }

            if (cmbDebitAccount.SelectedValue.Equals(cmbCreditAccount.SelectedValue))
            {
                MessageBox.Show("لا يمكن أن يكون الحساب المدين والدائن نفس الحساب", "تحذير");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAmount.Text) || !decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("يرجى إدخال مبلغ صحيح", "تحذير");
                txtAmount.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("يرجى إدخال وصف للمعاملة", "تحذير");
                txtDescription.Focus();
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