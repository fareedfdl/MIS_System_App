using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using MIS_System_App.Services;

namespace MIS_System_App.Forms.Inventory
{
    public partial class WarehouseForm : Form
    {
        private DatabaseService databaseService;
        private int? warehouseId;
        private TextBox txtName, txtLocation, txtCapacity;
        private Button btnSave, btnCancel;

        public WarehouseForm(int? whId = null)
        {
            InitializeComponent();
            databaseService = new DatabaseService();
            warehouseId = whId;
            
            if (warehouseId.HasValue)
            {
                LoadWarehouseData();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text = warehouseId.HasValue ? "تعديل بيانات المستودع" : "إضافة مستودع جديد";
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

            // اسم المستودع
            Label lblName = new Label();
            lblName.Text = "اسم المستودع:";
            lblName.Location = new Point(350, yPos);
            lblName.Size = new Size(labelWidth, 25);

            txtName = new TextBox();
            txtName.Location = new Point(40, yPos);
            txtName.Size = new Size(controlWidth, 25);
            yPos += 40;

            // الموقع الجغرافي
            Label lblLocation = new Label();
            lblLocation.Text = "الموقع الجغرافي:";
            lblLocation.Location = new Point(350, yPos);
            lblLocation.Size = new Size(labelWidth, 25);

            txtLocation = new TextBox();
            txtLocation.Location = new Point(40, yPos);
            txtLocation.Size = new Size(controlWidth, 25);
            yPos += 40;

            // السعة
            Label lblCapacity = new Label();
            lblCapacity.Text = "السعة:";
            lblCapacity.Location = new Point(350, yPos);
            lblCapacity.Size = new Size(labelWidth, 25);

            txtCapacity = new TextBox();
            txtCapacity.Location = new Point(40, yPos);
            txtCapacity.Size = new Size(controlWidth, 25);
            txtCapacity.Text = "0";
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
                lblLocation, txtLocation,
                lblCapacity, txtCapacity,
                btnSave, btnCancel
            });
        }

        private void LoadWarehouseData()
        {
            string query = @"SELECT warehouse_name, geographic_location, capacity 
                           FROM Warehouse WHERE Warehouse_ID = @WarehouseID";
            
            SqlParameter[] parameters = {
                new SqlParameter("@WarehouseID", warehouseId.Value)
            };

            DataTable result = databaseService.ExecuteQuery(query, parameters);
            
            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                txtName.Text = row["warehouse_name"].ToString();
                txtLocation.Text = row["geographic_location"].ToString();
                txtCapacity.Text = row["capacity"].ToString();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                try
                {
                    if (warehouseId.HasValue)
                    {
                        UpdateWarehouse();
                    }
                    else
                    {
                        InsertWarehouse();
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
                MessageBox.Show("يرجى إدخال اسم المستودع", "تحذير");
                txtName.Focus();
                return false;
            }

            if (!decimal.TryParse(txtCapacity.Text, out decimal capacity) || capacity < 0)
            {
                MessageBox.Show("يرجى إدخال سعة صحيحة", "تحذير");
                txtCapacity.Focus();
                return false;
            }

            return true;
        }

        private void InsertWarehouse()
        {
            string query = @"INSERT INTO Warehouse (warehouse_name, geographic_location, capacity, Org_ID, created_at)
                           VALUES (@Name, @Location, @Capacity, 1, GETDATE())";

            SqlParameter[] parameters = {
                new SqlParameter("@Name", txtName.Text.Trim()),
                new SqlParameter("@Location", txtLocation.Text.Trim()),
                new SqlParameter("@Capacity", decimal.Parse(txtCapacity.Text))
            };

            databaseService.ExecuteNonQuery(query, parameters);
        }

        private void UpdateWarehouse()
        {
            string query = @"UPDATE Warehouse SET 
                           warehouse_name = @Name,
                           geographic_location = @Location,
                           capacity = @Capacity
                           WHERE Warehouse_ID = @WarehouseID";

            SqlParameter[] parameters = {
                new SqlParameter("@Name", txtName.Text.Trim()),
                new SqlParameter("@Location", txtLocation.Text.Trim()),
                new SqlParameter("@Capacity", decimal.Parse(txtCapacity.Text)),
                new SqlParameter("@WarehouseID", warehouseId.Value)
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