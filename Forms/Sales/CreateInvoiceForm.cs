using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.Sales
{
    public partial class CreateInvoiceForm : Form
    {
        private DatabaseService databaseService;
        private SalesService salesService;
        private ComboBox cmbCustomer, cmbOrder;
        private DateTimePicker dtpIssueDate, dtpDueDate;
        private TextBox txtTaxRate, txtNotes;
        private Button btnCreate, btnCancel;

        public CreateInvoiceForm()
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            salesService = new SalesService();
            LoadCustomers();
            LoadOrders();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = "إنشاء فاتورة جديدة";
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

            // العميل
            Label lblCustomer = new Label();
            lblCustomer.Text = "العميل:";
            lblCustomer.Location = new Point(350, yPos);
            lblCustomer.Size = new Size(labelWidth, 25);

            cmbCustomer = new ComboBox();
            cmbCustomer.Location = new Point(40, yPos);
            cmbCustomer.Size = new Size(controlWidth, 25);
            cmbCustomer.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCustomer.SelectedIndexChanged += CmbCustomer_SelectedIndexChanged;
            yPos += 40;

            // طلب البيع
            Label lblOrder = new Label();
            lblOrder.Text = "طلب البيع:";
            lblOrder.Location = new Point(350, yPos);
            lblOrder.Size = new Size(labelWidth, 25);

            cmbOrder = new ComboBox();
            cmbOrder.Location = new Point(40, yPos);
            cmbOrder.Size = new Size(controlWidth, 25);
            cmbOrder.DropDownStyle = ComboBoxStyle.DropDownList;
            yPos += 40;

            // تاريخ الإصدار
            Label lblIssueDate = new Label();
            lblIssueDate.Text = "تاريخ الإصدار:";
            lblIssueDate.Location = new Point(350, yPos);
            lblIssueDate.Size = new Size(labelWidth, 25);

            dtpIssueDate = new DateTimePicker();
            dtpIssueDate.Location = new Point(40, yPos);
            dtpIssueDate.Size = new Size(controlWidth, 25);
            dtpIssueDate.Value = DateTime.Today;
            yPos += 40;

            // تاريخ الاستحقاق
            Label lblDueDate = new Label();
            lblDueDate.Text = "تاريخ الاستحقاق:";
            lblDueDate.Location = new Point(350, yPos);
            lblDueDate.Size = new Size(labelWidth, 25);

            dtpDueDate = new DateTimePicker();
            dtpDueDate.Location = new Point(40, yPos);
            dtpDueDate.Size = new Size(controlWidth, 25);
            dtpDueDate.Value = DateTime.Today.AddDays(30);
            yPos += 40;

            // نسبة الضريبة
            Label lblTaxRate = new Label();
            lblTaxRate.Text = "نسبة الضريبة %:";
            lblTaxRate.Location = new Point(350, yPos);
            lblTaxRate.Size = new Size(labelWidth, 25);

            txtTaxRate = new TextBox();
            txtTaxRate.Location = new Point(40, yPos);
            txtTaxRate.Size = new Size(controlWidth, 25);
            txtTaxRate.Text = "15";
            yPos += 40;

            // ملاحظات
            Label lblNotes = new Label();
            lblNotes.Text = "ملاحظات:";
            lblNotes.Location = new Point(350, yPos);
            lblNotes.Size = new Size(labelWidth, 25);

            txtNotes = new TextBox();
            txtNotes.Location = new Point(40, yPos);
            txtNotes.Size = new Size(controlWidth, 25);
            txtNotes.Multiline = true;
            txtNotes.Height = 60;
            yPos += 80;

            // أزرار التحكم
            btnCreate = new Button();
            btnCreate.Text = "إنشاء الفاتورة";
            btnCreate.Size = new Size(120, 35);
            btnCreate.Location = new Point(200, yPos);
            btnCreate.BackColor = Color.LightGreen;
            btnCreate.Click += BtnCreate_Click;

            btnCancel = new Button();
            btnCancel.Text = "إلغاء";
            btnCancel.Size = new Size(100, 35);
            btnCancel.Location = new Point(80, yPos);
            btnCancel.BackColor = Color.LightCoral;
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[] {
                lblCustomer, cmbCustomer,
                lblOrder, cmbOrder,
                lblIssueDate, dtpIssueDate,
                lblDueDate, dtpDueDate,
                lblTaxRate, txtTaxRate,
                lblNotes, txtNotes,
                btnCreate, btnCancel
            });
        }

        private void LoadCustomers()
        {
            string query = "SELECT Customer_ID, customer_name FROM Customer ORDER BY customer_name";
            DataTable customers = databaseService.ExecuteQuery(query);
            
            cmbCustomer.DisplayMember = "customer_name";
            cmbCustomer.ValueMember = "Customer_ID";
            cmbCustomer.DataSource = customers;
        }

        private void LoadOrders()
        {
            string query = @"SELECT Order_ID, 'طلب رقم ' + CAST(Order_ID as nvarchar) + ' - ' + customer_name as display_text
                           FROM Sales_Order so
                           INNER JOIN Customer c ON so.Customer_ID = c.Customer_ID
                           WHERE so.status = 'Confirmed'
                           ORDER BY so.Order_ID DESC";
            DataTable orders = databaseService.ExecuteQuery(query);
            
            cmbOrder.DisplayMember = "display_text";
            cmbOrder.ValueMember = "Order_ID";
            cmbOrder.DataSource = orders;
        }

        private void CmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCustomer.SelectedValue != null)
            {
                int customerId = Convert.ToInt32(cmbCustomer.SelectedValue);
                LoadCustomerOrders(customerId);
            }
        }

        private void LoadCustomerOrders(int customerId)
        {
            string query = @"SELECT Order_ID, 'طلب رقم ' + CAST(Order_ID as nvarchar) + ' - ' + CAST(total_amount as nvarchar) + ' ' + status as display_text
                           FROM Sales_Order 
                           WHERE Customer_ID = @CustomerID AND status = 'Confirmed'
                           ORDER BY Order_ID DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@CustomerID", customerId)
            };

            DataTable orders = databaseService.ExecuteQuery(query, parameters);
            
            cmbOrder.DisplayMember = "display_text";
            cmbOrder.ValueMember = "Order_ID";
            cmbOrder.DataSource = orders;
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    int customerId = Convert.ToInt32(cmbCustomer.SelectedValue);
                    int orderId = Convert.ToInt32(cmbOrder.SelectedValue);
                    DateTime issueDate = dtpIssueDate.Value;
                    DateTime dueDate = dtpDueDate.Value;
                    decimal taxRate = decimal.Parse(txtTaxRate.Text);
                    string notes = txtNotes.Text.Trim();

                    // الحصول على مبلغ الطلب
                    string orderQuery = "SELECT total_amount FROM Sales_Order WHERE Order_ID = @OrderID";
                    SqlParameter[] orderParams = {
                        new SqlParameter("@OrderID", orderId)
                    };
                    
                    object orderAmount = databaseService.ExecuteScalar(orderQuery, orderParams);
                    decimal totalAmount = orderAmount != null ? Convert.ToDecimal(orderAmount) : 0;
                    decimal taxAmount = totalAmount * (taxRate / 100);

                    // إنشاء الفاتورة
                    string query = @"INSERT INTO Invoice (issue_date, due_date, Customer_ID, status, total_amount, tax_amount, Org_ID, created_at)
                                   VALUES (@IssueDate, @DueDate, @CustomerID, 'Sent', @TotalAmount, @TaxAmount, 1, GETDATE())";

                    SqlParameter[] parameters = {
                        new SqlParameter("@IssueDate", issueDate),
                        new SqlParameter("@DueDate", dueDate),
                        new SqlParameter("@CustomerID", customerId),
                        new SqlParameter("@TotalAmount", totalAmount),
                        new SqlParameter("@TaxAmount", taxAmount)
                    };

                    int result = databaseService.ExecuteNonQuery(query, parameters);
                    
                    if (result > 0)
                    {
                        MessageBox.Show("تم إنشاء الفاتورة بنجاح", "نجاح");
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"حدث خطأ أثناء إنشاء الفاتورة: {ex.Message}", "خطأ");
                }
            }
        }

        private bool ValidateForm()
        {
            if (cmbCustomer.SelectedValue == null)
            {
                MessageBox.Show("يرجى اختيار عميل", "تحذير");
                return false;
            }

            if (cmbOrder.SelectedValue == null)
            {
                MessageBox.Show("يرجى اختيار طلب بيع", "تحذير");
                return false;
            }

            if (!decimal.TryParse(txtTaxRate.Text, out decimal taxRate) || taxRate < 0 || taxRate > 100)
            {
                MessageBox.Show("يرجى إدخال نسبة ضريبة صحيحة بين 0 و 100", "تحذير");
                txtTaxRate.Focus();
                return false;
            }

            if (dtpDueDate.Value <= dtpIssueDate.Value)
            {
                MessageBox.Show("تاريخ الاستحقاق يجب أن يكون بعد تاريخ الإصدار", "تحذير");
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