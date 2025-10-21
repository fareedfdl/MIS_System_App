using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MIS_System_App.Services
{
    public class InventoryService
    {
        private DatabaseService databaseService;

        public InventoryService()
        {
            databaseService = new DatabaseService();
        }

        // تحديث كمية المنتج في المخزون
        public bool UpdateProductQuantity(int productId, int newQuantity)
        {
            string query = "UPDATE Product SET available_quantity = @Quantity WHERE Product_ID = @ProductID";
            
            SqlParameter[] parameters = {
                new SqlParameter("@Quantity", newQuantity),
                new SqlParameter("@ProductID", productId)
            };

            int result = databaseService.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // الحصول على منتجات تحتاج إعادة طلب
        public DataTable GetProductsNeedReorder()
        {
            string query = @"
                SELECT 
                    product_name as 'اسم المنتج',
                    available_quantity as 'الكمية المتاحة',
                    reorder_level as 'حد إعادة الطلب',
                    unit_of_measure as 'الوحدة'
                FROM Product 
                WHERE is_active = 1 AND available_quantity <= reorder_level
                ORDER BY available_quantity ASC";

            return databaseService.ExecuteQuery(query);
        }

        // تسجيل حركة المخزون
        public bool RecordInventoryMovement(int productId, int warehouseId, int quantity, string movementType)
        {
            string query = @"
                IF EXISTS (SELECT 1 FROM Inventory WHERE Product_ID = @ProductID AND Warehouse_ID = @WarehouseID)
                BEGIN
                    UPDATE Inventory SET quantity = quantity + @Quantity, last_updated = GETDATE()
                    WHERE Product_ID = @ProductID AND Warehouse_ID = @WarehouseID
                END
                ELSE
                BEGIN
                    INSERT INTO Inventory (Product_ID, Warehouse_ID, quantity, last_updated)
                    VALUES (@ProductID, @WarehouseID, @Quantity, GETDATE())
                END";

            SqlParameter[] parameters = {
                new SqlParameter("@ProductID", productId),
                new SqlParameter("@WarehouseID", warehouseId),
                new SqlParameter("@Quantity", movementType == "IN" ? quantity : -quantity)
            };

            int result = databaseService.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // الحصول على تقرير المخزون بالمستودعات
        public DataTable GetInventoryReport()
        {
            string query = @"
                SELECT 
                    p.product_name as 'اسم المنتج',
                    w.warehouse_name as 'اسم المستودع',
                    i.quantity as 'الكمية',
                    p.unit_of_measure as 'الوحدة',
                    i.last_updated as 'آخر تحديث'
                FROM Inventory i
                INNER JOIN Product p ON i.Product_ID = p.Product_ID
                INNER JOIN Warehouse w ON i.Warehouse_ID = w.Warehouse_ID
                WHERE p.is_active = 1
                ORDER BY p.product_name, w.warehouse_name";

            return databaseService.ExecuteQuery(query);
        }
    }
}