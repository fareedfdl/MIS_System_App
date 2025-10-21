
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.Inventory
{
    public partial class ProductForm : Form
    {
        private DatabaseService databaseService;
        private int? productId;
        private TextBox txtName, txtDescription, txtCost, txtSelling, txtReorder, txtQuantity;
        private ComboBox cmbUnit;
        private CheckBox chkActive;
        private Button btnSave, btnCancel;

        public ProductForm(int? prodId = null)
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            productId = prodId;
            
            if (productId.HasValue)
            {
                LoadProductData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = productId.HasValue ? "تعديل بيانات المنتج" : "إضافة منتج جديد";
            this.Size = new Size(500, 500);
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

            // اسم المنتج
            Label lblName = new Label();
            lblName.Text = "اسم المنتج:";
            lblName.Location = new Point(350, yPos);
            lblName.Size = new Size(labelWidth, 25);

            txtName = new TextBox();
            txtName.Location = new Point(40, yPos);
            txtName.Size = new Size(controlWidth, 25);
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

            // سعر التكلفة
            Label lblCost = new Label();
            lblCost.Text = "سعر التكلفة:";
            lblCost.Location = new Point(350, yPos);
            lblCost.Size = new Size(labelWidth, 25);

            txtCost = new TextBox();
            txtCost.Location = new Point(40, yPos);
            txtCost.Size = new Size(controlWidth, 25);
            yPos += 40;

            // سعر البيع
            Label lblSelling = new Label();
            lblSelling.Text = "سعر البيع:";
            lblSelling.Location = new Point(350, yPos);
            lblSelling.Size = new Size(labelWidth, 25);

            txtSelling = new TextBox();
            txtSelling.Location = new Point(40, yPos);
            txtSelling.Size = new Size(controlWidth, 25);
            yPos += 40;

            // الكمية المتاحة
            Label lblQuantity = new Label();
            lblQuantity.Text = "الكمية المتاحة:";
            lblQuantity.Location = new Point(350, yPos);
            lblQuantity.Size = new Size(labelWidth, 25);

            txtQuantity = new TextBox();
            txtQuantity.Location = new Point(40, yPos);
            txtQuantity.Size = new Size(controlWidth, 25);
            yPos += 40;

            // وحدة القياس
            Label lblUnit = new Label();
            lblUnit.Text = "وحدة القياس:";
            lblUnit.Location = new Point(350, yPos);
            lblUnit.Size = new Size(labelWidth, 25);

            cmbUnit = new ComboBox();
            cmbUnit.Location = new Point(40, yPos);
            cmbUnit.Size = new Size(controlWidth, 25);
            cmbUnit.Items.AddRange(new string[] { "قطعة", "كيلو", "لتر", "علبة", "كرتون" });
            cmbUnit.SelectedIndex = 0;
            yPos += 40;

            // حد إعادة الطلب
            Label lblReorder = new Label();
            lblReorder.Text = "حد إعادة الطلب:";
            lblReorder.Location = new Point(350, yPos);
            lblReorder.Size = new Size(labelWidth, 25);

            txtReorder = new TextBox();
            txtReorder.Location = new Point(40, yPos);
            txtReorder.Size = new Size(controlWidth, 25);
            txtReorder.Text = "10";
            yPos += 40;

            // الحالة
            chkActive = new CheckBox();
            chkActive.Text = "المنتج نشط";
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
                lblDescription, txtDescription,
                lblCost, txtCost,
                lblSelling, txtSelling,
                lblQuantity, txtQuantity,
                lblUnit, cmbUnit,
                lblReorder, txtReorder,
                chkActive,
                btnSave, btnCancel
            });
        }

        private void LoadProductData()
        {
            string query = @"SELECT product_name, product_description, cost_price, selling_price, 
                           available_quantity, unit_of_measure, reorder_level, is_active
                           FROM Product WHERE Product_ID = @ProductID";
            
            SqlParameter[] parameters = {
                new SqlParameter("@ProductID", productId.Value)
            };

            DataTable result = databaseService.ExecuteQuery(query, parameters);
            
            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                txtName.Text = row["product_name"].ToString();
                txtDescription.Text = row["product_description"].ToString();
                txtCost.Text = row["cost_price"].ToString();
                txtSelling.Text = row["selling_price"].ToString();
                txtQuantity.Text = row["available_quantity"].ToString();
                txtReorder.Text = row["reorder_level"].ToString();
                cmbUnit.Text = row["unit_of_measure"].ToString();
                chkActive.Checked = Convert.ToBoolean(row["is_active"]);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    if (productId.HasValue)
                    {
                        UpdateProduct();
                    }
                    else
                    {
                        InsertProduct();
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
                MessageBox.Show("يرجى إدخال اسم المنتج", "تحذير");
                txtName.Focus();
                return false;
            }

            if (!decimal.TryParse(txtCost.Text, out decimal cost) || cost < 0)
            {
                MessageBox.Show("يرجى إدخال سعر تكلفة صحيح", "تحذير");
                txtCost.Focus();
                return false;
            }

            if (!decimal.TryParse(txtSelling.Text, out decimal selling) || selling < 0)
            {
                MessageBox.Show("يرجى إدخال سعر بيع صحيح", "تحذير");
                txtSelling.Focus();
                return false;
            }

            if (selling < cost)
            {
                MessageBox.Show("سعر البيع يجب أن يكون أكبر من سعر التكلفة", "تحذير");
                txtSelling.Focus();
                return false;
            }

            return true;
        }

        private void InsertProduct()
        {
            string query = @"INSERT INTO Product (product_name, product_description, cost_price, selling_price,
                           available_quantity, unit_of_measure, reorder_level, is_active, Org_ID, created_at)
                           VALUES (@Name, @Description, @Cost, @Selling, @Quantity, @Unit, @Reorder, @Active, 1, GETDATE())";

            SqlParameter[] parameters = {
                new SqlParameter("@Name", txtName.Text.Trim()),
                new SqlParameter("@Description", txtDescription.Text.Trim()),
                new SqlParameter("@Cost", decimal.Parse(txtCost.Text)),
                new SqlParameter("@Selling", decimal.Parse(txtSelling.Text)),
                new SqlParameter("@Quantity", int.Parse(txtQuantity.Text)),
                new SqlParameter("@Unit", cmbUnit.Text),
                new SqlParameter("@Reorder", int.Parse(txtReorder.Text)),
                new SqlParameter("@Active", chkActive.Checked)
            };

            databaseService.ExecuteNonQuery(query, parameters);
        }

        private void UpdateProduct()
        {
            string query = @"UPDATE Product SET 
                           product_name = @Name,
                           product_description = @Description,
                           cost_price = @Cost,
                           selling_price = @Selling,
                           available_quantity = @Quantity,
                           unit_of_measure = @Unit,
                           reorder_level = @Reorder,
                           is_active = @Active
                           WHERE Product_ID = @ProductID";

            SqlParameter[] parameters = {
                new SqlParameter("@Name", txtName.Text.Trim()),
                new SqlParameter("@Description", txtDescription.Text.Trim()),
                new SqlParameter("@Cost", decimal.Parse(txtCost.Text)),
                new SqlParameter("@Selling", decimal.Parse(txtSelling.Text)),
                new SqlParameter("@Quantity", int.Parse(txtQuantity.Text)),
                new SqlParameter("@Unit", cmbUnit.Text),
                new SqlParameter("@Reorder", int.Parse(txtReorder.Text)),
                new SqlParameter("@Active", chkActive.Checked),
                new SqlParameter("@ProductID", productId.Value)
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
