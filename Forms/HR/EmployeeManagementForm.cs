using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Models;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.HR
{
    public partial class EmployeeManagementForm : Form
    {
        private User currentUser;
        private DatabaseService databaseService;
        private DataGridView dgvEmployees;
        private TextBox txtSearch;
        private Button btnAdd, btnEdit, btnDelete, btnRefresh;

        public EmployeeManagementForm(User user)
        {
            InitializeComponent();
            currentUser = user;
            databaseService = new DatabaseService();

            try
            {
                LoadEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل بيانات الموظفين: {ex.Message}", "خطأ");
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // إعداد النموذج
            this.Text = "إدارة الموظفين - الموارد البشرية";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Arial", 10);

            // إنشاء عناصر التحكم
            CreateControls();

            this.ResumeLayout(false);
        }

        private void CreateControls()
        {
            // لوحة العنوان
            Panel topPanel = new Panel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 80;
            topPanel.BackColor = Color.LightBlue;
            topPanel.Padding = new Padding(10);

            Label titleLabel = new Label();
            titleLabel.Text = "إدارة الموظفين - الموارد البشرية";
            titleLabel.Font = new Font("Arial", 16, FontStyle.Bold);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Height = 40;

            // شريط البحث
            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Height = 40;

            Label lblSearch = new Label();
            lblSearch.Text = "بحث:";
            lblSearch.Location = new Point(10, 10);
            lblSearch.Size = new Size(40, 20);

            txtSearch = new TextBox();
            txtSearch.Location = new Point(60, 8);
            txtSearch.Size = new Size(200, 25);
            txtSearch.TextChanged += TxtSearch_TextChanged;

            searchPanel.Controls.AddRange(new Control[] { lblSearch, txtSearch });

            topPanel.Controls.AddRange(new Control[] { titleLabel, searchPanel });

            // شبكة البيانات
            dgvEmployees = new DataGridView();
            dgvEmployees.Dock = DockStyle.Fill;
            dgvEmployees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvEmployees.ReadOnly = true;
            dgvEmployees.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEmployees.RowHeadersVisible = false;
            dgvEmployees.AllowUserToAddRows = false;
            dgvEmployees.AllowUserToDeleteRows = false;

            // أزرار التحكم
            Panel buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.Padding = new Padding(10);

            btnAdd = new Button();
            btnAdd.Text = "إضافة موظف جديد";
            btnAdd.Size = new Size(120, 35);
            btnAdd.Location = new Point(10, 12);
            btnAdd.BackColor = Color.LightGreen;
            btnAdd.Font = new Font("Arial", 10, FontStyle.Bold);
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button();
            btnEdit.Text = "تعديل البيانات";
            btnEdit.Size = new Size(120, 35);
            btnEdit.Location = new Point(140, 12);
            btnEdit.BackColor = Color.LightYellow;
            btnEdit.Font = new Font("Arial", 10, FontStyle.Bold);
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button();
            btnDelete.Text = "حذف الموظف";
            btnDelete.Size = new Size(120, 35);
            btnDelete.Location = new Point(270, 12);
            btnDelete.BackColor = Color.LightCoral;
            btnDelete.Font = new Font("Arial", 10, FontStyle.Bold);
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new Button();
            btnRefresh.Text = "تحديث البيانات";
            btnRefresh.Size = new Size(120, 35);
            btnRefresh.Location = new Point(400, 12);
            btnRefresh.BackColor = Color.LightGray;
            btnRefresh.Font = new Font("Arial", 10, FontStyle.Bold);
            btnRefresh.Click += BtnRefresh_Click;

            Button btnClose = new Button();
            btnClose.Text = "إغلاق";
            btnClose.Size = new Size(80, 35);
            btnClose.Location = new Point(1050, 12);
            btnClose.BackColor = Color.LightCoral;
            btnClose.Click += BtnClose_Click;

            buttonPanel.Controls.AddRange(new Control[] {
                btnAdd, btnEdit, btnDelete, btnRefresh, btnClose
            });

            // إضافة العناصر للنموذج
            this.Controls.AddRange(new Control[] { dgvEmployees, buttonPanel, topPanel });
        }

        private void LoadEmployees()
        {
            try
            {
                string query = @"
                    SELECT 
                        e.Emp_ID,
                        e.full_name as 'الاسم الكامل',
                        e.hire_date as 'تاريخ التعيين',
                        e.base_salary as 'الراتب الأساسي',
                        ISNULL(d.department_name, 'غير محدد') as 'القسم',
                        e.phone_number as 'الهاتف',
                        e.email as 'البريد الإلكتروني',
                        e.address as 'العنوان'
                    FROM Employee e
                    LEFT JOIN Department d ON e.Dept_ID = d.Dept_ID
                    ORDER BY e.full_name";

                DataTable employees = databaseService.ExecuteQuery(query);

                if (employees != null)
                {
                    dgvEmployees.DataSource = employees;

                    // تنسيق الأعمدة
                    if (dgvEmployees.Columns.Count > 0)
                    {
                        dgvEmployees.Columns["الراتب الأساسي"].DefaultCellStyle.Format = "C0";
                        dgvEmployees.Columns["تاريخ التعيين"].DefaultCellStyle.Format = "yyyy-MM-dd";
                    }
                }
                else
                {
                    dgvEmployees.DataSource = new DataTable();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل بيانات الموظفين: {ex.Message}", "خطأ");
                dgvEmployees.DataSource = new DataTable();
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dgvEmployees.DataSource is DataTable dataTable && dataTable != null)
            {
                string searchText = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    dataTable.DefaultView.RowFilter =
                        $"`الاسم الكامل` LIKE '%{searchText}%' OR " +
                        $"`البريد الإلكتروني` LIKE '%{searchText}%' OR " +
                        $"`الهاتف` LIKE '%{searchText}%' OR " +
                        $"`القسم` LIKE '%{searchText}%'";
                }
                else
                {
                    dataTable.DefaultView.RowFilter = "";
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                EmployeeForm employeeForm = new EmployeeForm();
                if (employeeForm.ShowDialog() == DialogResult.OK)
                {
                    LoadEmployees();
                    MessageBox.Show("تم إضافة الموظف بنجاح", "نجاح",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إضافة الموظف: {ex.Message}", "خطأ");
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvEmployees.SelectedRows.Count > 0 && dgvEmployees.SelectedRows[0] != null)
                {
                    int employeeId = Convert.ToInt32(dgvEmployees.SelectedRows[0].Cells["Emp_ID"].Value);
                    EmployeeForm employeeForm = new EmployeeForm(employeeId);
                    if (employeeForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadEmployees();
                        MessageBox.Show("تم تحديث بيانات الموظف بنجاح", "نجاح",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("يرجى اختيار موظف للتعديل", "تحذير",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تعديل الموظف: {ex.Message}", "خطأ");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvEmployees.SelectedRows.Count > 0 && dgvEmployees.SelectedRows[0] != null)
                {
                    int employeeId = Convert.ToInt32(dgvEmployees.SelectedRows[0].Cells["Emp_ID"].Value);
                    string employeeName = dgvEmployees.SelectedRows[0].Cells["الاسم الكامل"].Value?.ToString() ?? "غير معروف";

                    DialogResult result = MessageBox.Show(
                        $"هل أنت متأكد من حذف الموظف: {employeeName}؟\nهذا الإجراء لا يمكن التراجع عنه.",
                        "تأكيد الحذف",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        string query = "DELETE FROM Employee WHERE Emp_ID = @EmpID";
                        SqlParameter[] parameters = {
                            new SqlParameter("@EmpID", employeeId)
                        };

                        int rowsAffected = databaseService.ExecuteNonQuery(query, parameters);

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("تم حذف الموظف بنجاح", "نجاح",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadEmployees();
                        }
                        else
                        {
                            MessageBox.Show("حدث خطأ أثناء حذف الموظف", "خطأ",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("يرجى اختيار موظف للحذف", "تحذير",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حذف الموظف: {ex.Message}", "خطأ");
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadEmployees();
                txtSearch.Clear();
                MessageBox.Show("تم تحديث البيانات", "تحديث",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحديث البيانات: {ex.Message}", "خطأ");
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}