using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;
using MIS_System_App.Forms.Sales; // Add this line if SelectProductForm is in this namespace

namespace MIS_System_App.Forms.Sales
{
    public partial class SalesOrderForm : Form
    {
        private DatabaseService databaseService;
        private int? orderId;
        private ComboBox cmbCustomer;
        private DataGridView dgvOrderItems;
        private TextBox txtTotalAmount;
        private DateTimePicker dtpOrderDate;
        private Button btnAddItem, btnRemoveItem, btnSave, btnCancel;

        public SalesOrderForm(int? ordId = null)
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            orderId = ordId;

            try
            {
                LoadCustomers();

                if (orderId.HasValue)
                {
                    LoadOrderData();
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

            this.Text = orderId.HasValue ? "تعديل طلب بيع" : "إنشاء طلب بيع جديد";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            CreateControls();
            this.ResumeLayout(false);
        }

        private void CreateControls()
        {
            // العميل
            Label lblCustomer = new Label();
            lblCustomer.Text = "العميل:";
            lblCustomer.Location = new Point(650, 20);
            lblCustomer.Size = new Size(80, 25);

            cmbCustomer = new ComboBox();
            cmbCustomer.Location = new Point(350, 20);
            cmbCustomer.Size = new Size(280, 25);
            cmbCustomer.DropDownStyle = ComboBoxStyle.DropDownList;
            LoadCustomers();

            // تاريخ الطلب
            Label lblOrderDate = new Label();
            lblOrderDate.Text = "تاريخ الطلب:";
            lblOrderDate.Location = new Point(650, 60);
            lblOrderDate.Size = new Size(80, 25);

            dtpOrderDate = new DateTimePicker();
            dtpOrderDate.Location = new Point(350, 60);
            dtpOrderDate.Size = new Size(280, 25);
            dtpOrderDate.Value = DateTime.Today;

            // عناصر الطلب
            Label lblItems = new Label();
            lblItems.Text = "عناصر الطلب:";
            lblItems.Location = new Point(650, 100);
            lblItems.Size = new Size(80, 25);

            dgvOrderItems = new DataGridView();
            dgvOrderItems.Location = new Point(20, 130);
            dgvOrderItems.Size = new Size(740, 300);
            dgvOrderItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            SetupOrderItemsGrid();

            // أزرار إدارة العناصر
            btnAddItem = new Button();
            btnAddItem.Text = "إضافة منتج";
            btnAddItem.Location = new Point(20, 440);
            btnAddItem.Size = new Size(100, 30);
            btnAddItem.BackColor = Color.LightGreen;
            btnAddItem.Click += BtnAddItem_Click;

            btnRemoveItem = new Button();
            btnRemoveItem.Text = "حذف منتج";
            btnRemoveItem.Location = new Point(130, 440);
            btnRemoveItem.Size = new Size(100, 30);
            btnRemoveItem.BackColor = Color.LightCoral;
            btnRemoveItem.Click += BtnRemoveItem_Click;

            // المبلغ الإجمالي
            Label lblTotal = new Label();
            lblTotal.Text = "المبلغ الإجمالي:";
            lblTotal.Location = new Point(650, 480);
            lblTotal.Size = new Size(100, 25);
            lblTotal.Font = new Font("Arial", 10, FontStyle.Bold);

            txtTotalAmount = new TextBox();
            txtTotalAmount.Location = new Point(500, 480);
            txtTotalAmount.Size = new Size(130, 25);
            txtTotalAmount.Text = "0.00";
            txtTotalAmount.ReadOnly = true;
            txtTotalAmount.Font = new Font("Arial", 10, FontStyle.Bold);
            txtTotalAmount.BackColor = Color.LightYellow;

            // أزرار الحفظ والإلغاء
            btnSave = new Button();
            btnSave.Text = "حفظ الطلب";
            btnSave.Size = new Size(120, 35);
            btnSave.Location = new Point(350, 520);
            btnSave.BackColor = Color.LightGreen;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button();
            btnCancel.Text = "إلغاء";
            btnCancel.Size = new Size(120, 35);
            btnCancel.Location = new Point(200, 520);
            btnCancel.BackColor = Color.LightCoral;
            btnCancel.Click += BtnCancel_Click;

            this.Controls.AddRange(new Control[] {
                lblCustomer, cmbCustomer,
                lblOrderDate, dtpOrderDate,
                lblItems, dgvOrderItems,
                btnAddItem, btnRemoveItem,
                lblTotal, txtTotalAmount,
                btnSave, btnCancel
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

        private void SetupOrderItemsGrid()
        {
            dgvOrderItems.Columns.Clear();

            dgvOrderItems.Columns.Add("ProductID", "رقم المنتج");
            dgvOrderItems.Columns.Add("ProductName", "اسم المنتج");
            dgvOrderItems.Columns.Add("Quantity", "الكمية");
            dgvOrderItems.Columns.Add("UnitPrice", "سعر الوحدة");
            dgvOrderItems.Columns.Add("Total", "المجموع");

            dgvOrderItems.Columns["ProductID"].Visible = false;
            dgvOrderItems.Columns["UnitPrice"].DefaultCellStyle.Format = "C0";
            dgvOrderItems.Columns["Total"].DefaultCellStyle.Format = "C0";
        }

        private void LoadOrderData()
        {
            // سيتم تنفيذ هذا لتحميل بيانات الطلب الحالي
        }

        private void BtnAddItem_Click(object sender, EventArgs e)
        {
            // فتح نموذج لإضافة منتج للطلب
            SelectProductForm productForm = new SelectProductForm();
            if (productForm.ShowDialog() == DialogResult.OK)
            {
                // إضافة المنتج المختار للطلب
            }
        }

        private void BtnRemoveItem_Click(object sender, EventArgs e)
        {
            if (dgvOrderItems.SelectedRows.Count > 0)
            {
                dgvOrderItems.Rows.RemoveAt(dgvOrderItems.SelectedRows[0].Index);
                CalculateTotal();
            }
        }

        private void CalculateTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dgvOrderItems.Rows)
            {
                if (row.Cells["Total"].Value != null)
                {
                    total += Convert.ToDecimal(row.Cells["Total"].Value);
                }
            }
            txtTotalAmount.Text = total.ToString("C0");
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    SaveOrder();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"حدث خطأ أثناء حفظ الطلب: {ex.Message}", "خطأ");
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

            if (dgvOrderItems.Rows.Count == 0)
            {
                MessageBox.Show("يرجى إضافة منتجات للطلب", "تحذير");
                return false;
            }

            return true;
        }

        // إضافة هذا الكود في دالة SaveOrder
        private void SaveOrder()
        {
            // حفظ الطلب في قاعدة البيانات
            string query = @"INSERT INTO Sales_Order (Customer_ID, order_date, total_amount, status, shipping_address, Org_ID, created_at)
                   VALUES (@CustomerID, @OrderDate, @TotalAmount, 'Pending', @ShippingAddress, 1, GETDATE());
                   SELECT SCOPE_IDENTITY();";

            decimal totalAmount = decimal.Parse(txtTotalAmount.Text.Replace("$", "").Replace("₡", "").Replace(",", ""));

            SqlParameter[] parameters = {
        new SqlParameter("@CustomerID", cmbCustomer.SelectedValue),
        new SqlParameter("@OrderDate", dtpOrderDate.Value),
        new SqlParameter("@TotalAmount", totalAmount),
        new SqlParameter("@ShippingAddress", GetCustomerAddress())
    };

            // تنفيذ الأمر والحصول على رقم الطلب الجديد
            object result = databaseService.ExecuteScalar(query, parameters);

            if (result != null)
            {
                int newOrderId = Convert.ToInt32(result);
                SaveOrderItems(newOrderId);
                MessageBox.Show($"تم حفظ الطلب بنجاح برقم: {newOrderId}", "نجاح");
            }
        }

        private string GetCustomerAddress()
        {
            // في التطبيق الحقيقي، سيتم جلب عنوان الشحن من بيانات العميل
            return "عنوان الشحن الافتراضي";
        }

        private void SaveOrderItems(int orderId)
        {
            foreach (DataGridViewRow row in dgvOrderItems.Rows)
            {
                if (row.Cells["ProductID"].Value != null)
                {
                    int productId = Convert.ToInt32(row.Cells["ProductID"].Value);
                    int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                    decimal unitPrice = Convert.ToDecimal(row.Cells["UnitPrice"].Value);
                    decimal lineTotal = Convert.ToDecimal(row.Cells["Total"].Value);

                    string query = @"INSERT INTO Order_Item (Order_ID, Product_ID, quantity, unit_price, line_total, created_at)
                           VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice, @LineTotal, GETDATE())";

                    SqlParameter[] parameters = {
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@ProductID", productId),
                new SqlParameter("@Quantity", quantity),
                new SqlParameter("@UnitPrice", unitPrice),
                new SqlParameter("@LineTotal", lineTotal)
            };

                    databaseService.ExecuteNonQuery(query, parameters);
                }
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
