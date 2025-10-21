using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.Accounting
{
    public partial class AccountForm : Form
    {
        private DatabaseService databaseService;
        private int? accountId;
        private TextBox txtName, txtCode;
        private ComboBox cmbType, cmbParent;
        private CheckBox chkActive;
        private Button btnSave, btnCancel;

        public AccountForm(int? accId = null)
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            accountId = accId;
            
            if (accountId.HasValue)
            {
                LoadAccountData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = accountId.HasValue ? "تعديل بيانات الحساب" : "إضافة حساب جديد";
            this.Size = new Size(500, 350);
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

            // اسم الحساب
            Label lblName = new Label();
            lblName.Text = "اسم الحساب:";
            lblName.Location = new Point(350, yPos);
            lblName.Size = new Size(labelWidth, 25);

            txtName = new TextBox();
            txtName.Location = new Point(40, yPos);
            txtName.Size = new Size(controlWidth, 25);
            yPos += 40;

            // كود الحساب
            Label lblCode = new Label();
            lblCode.Text = "كود الحساب:";
            lblCode.Location = new Point(350, yPos);
            lblCode.Size = new Size(labelWidth, 25);

            txtCode = new TextBox();
            txtCode.Location = new Point(40, yPos);
            txtCode.Size = new Size(controlWidth, 25);
            yPos += 40;

            // نوع الحساب
            Label lblType = new Label();
            lblType.Text = "نوع الحساب:";
            lblType.Location = new Point(350, yPos);
            lblType.Size = new Size(labelWidth, 25);

            cmbType = new ComboBox();
            cmbType.Location = new Point(40, yPos);
            cmbType.Size = new Size(controlWidth, 25);
            cmbType.Items.AddRange(new string[] { "Asset", "Liability", "Equity", "Revenue", "Expense" });
            cmbType.SelectedIndex = 0;
            yPos += 40;

            // الحساب الرئيسي
            Label lblParent = new Label();
            lblParent.Text = "الحساب الرئيسي:";
            lblParent.Location = new Point(350, yPos);
            lblParent.Size = new Size(labelWidth, 25);

            cmbParent = new ComboBox();
            cmbParent.Location = new Point(40, yPos);
            cmbParent.Size = new Size(controlWidth, 25);
            cmbParent.DropDownStyle = ComboBoxStyle.DropDownList;
            LoadParentAccounts();
            yPos += 40;

            // الحالة
            chkActive = new CheckBox();
            chkActive.Text = "الحساب نشط";
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

            this.Controls.AddRange(new Control[] {
                lblName, txtName,
                lblCode, txtCode,
                lblType, cmbType,
                lblParent, cmbParent,
                chkActive,
                btnSave, btnCancel
            });
        }

        private void LoadParentAccounts()
        {
            string query = "SELECT Account_ID, account_name FROM Account WHERE parent_account_id IS NULL AND is_active = 1 ORDER BY account_name";
            DataTable accounts = databaseService.ExecuteQuery(query);
            
            cmbParent.DisplayMember = "account_name";
            cmbParent.ValueMember = "Account_ID";
            cmbParent.DataSource = accounts;
        }

        private void LoadAccountData()
        {
            string query = @"SELECT account_name, account_code, account_type, parent_account_id, is_active
                           FROM Account WHERE Account_ID = @AccountID";
            
            SqlParameter[] parameters = {
                new SqlParameter("@AccountID", accountId.Value)
            };

            DataTable result = databaseService.ExecuteQuery(query, parameters);
            
            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                txtName.Text = row["account_name"].ToString();
                txtCode.Text = row["account_code"].ToString();
                cmbType.Text = row["account_type"].ToString();
                chkActive.Checked = Convert.ToBoolean(row["is_active"]);
                
                if (row["parent_account_id"] != DBNull.Value)
                {
                    cmbParent.SelectedValue = Convert.ToInt32(row["parent_account_id"]);
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    if (accountId.HasValue)
                    {
                        UpdateAccount();
                    }
                    else
                    {
                        InsertAccount();
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
                MessageBox.Show("يرجى إدخال اسم الحساب", "تحذير");
                txtName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                MessageBox.Show("يرجى إدخال كود الحساب", "تحذير");
                txtCode.Focus();
                return false;
            }

            return true;
        }

        private void InsertAccount()
        {
            string query = @"INSERT INTO Account (account_name, account_code, account_type, parent_account_id, Org_ID, is_active, created_at)
                           VALUES (@Name, @Code, @Type, @ParentID, 1, @Active, GETDATE())";

            SqlParameter[] parameters = {
                new SqlParameter("@Name", txtName.Text.Trim()),
                new SqlParameter("@Code", txtCode.Text.Trim()),
                new SqlParameter("@Type", cmbType.Text),
                new SqlParameter("@ParentID", cmbParent.SelectedValue ?? (object)DBNull.Value),
                new SqlParameter("@Active", chkActive.Checked)
            };

            databaseService.ExecuteNonQuery(query, parameters);
        }

        private void UpdateAccount()
        {
            string query = @"UPDATE Account SET 
                           account_name = @Name,
                           account_code = @Code,
                           account_type = @Type,
                           parent_account_id = @ParentID,
                           is_active = @Active
                           WHERE Account_ID = @AccountID";

            SqlParameter[] parameters = {
                new SqlParameter("@Name", txtName.Text.Trim()),
                new SqlParameter("@Code", txtCode.Text.Trim()),
                new SqlParameter("@Type", cmbType.Text),
                new SqlParameter("@ParentID", cmbParent.SelectedValue ?? (object)DBNull.Value),
                new SqlParameter("@Active", chkActive.Checked),
                new SqlParameter("@AccountID", accountId.Value)
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