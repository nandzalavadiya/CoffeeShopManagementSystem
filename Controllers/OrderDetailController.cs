using CoffeeShopManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace CoffeeShopManagementSystem.Controllers
{
    
    public class OrderDetailController : Controller
    {
        private IConfiguration configuration;

        public OrderDetailController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #region OrderDetailList
        public IActionResult OrderDetailList()
        {

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "usp_SelectAllOrderDetail";
            SqlDataReader reader = sqlCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            return View(dataTable);
        }
        #endregion

        #region OrderDetailSave
        public IActionResult OrderDetailSave(OrderDetailModel orderDetailModel)
        {
            if (orderDetailModel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }
            if (orderDetailModel.OrderID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }
            if (orderDetailModel.ProductID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                if (orderDetailModel.OrderDetailID == null)
                {
                    sqlCommand.CommandText = "usp_InsertOrderDetail";
                    TempData["OrderDetailsInsert"] = "OrderDetails Inserted successfully!";
                }
                else
                {
                    sqlCommand.CommandText = "usp_UpdateOrderDetail";
                    sqlCommand.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = orderDetailModel.OrderDetailID;
                    TempData["OrderDetailsUpdated"] = "OrderDetails Updated successfully!";
                }
                sqlCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = orderDetailModel.OrderID;
                sqlCommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = orderDetailModel.ProductID;
                sqlCommand.Parameters.Add("@Quantity", SqlDbType.Int).Value = orderDetailModel.Quantity;
                sqlCommand.Parameters.Add("@Amount", SqlDbType.Decimal).Value = orderDetailModel.Amount;
                sqlCommand.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = orderDetailModel.TotalAmount;
                sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = orderDetailModel.UserID;
                sqlCommand.ExecuteNonQuery();
                return RedirectToAction("OrderDetailList");
            }

            return View("OrderDetailAddEdit",orderDetailModel);
        }
        #endregion

        #region OrderDetailAddEdit
        public IActionResult OrderDetailAddEdit(int orderDetailID=0)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_User_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<ForOrderDetailUserDropDownModel> userList = new List<ForOrderDetailUserDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                ForOrderDetailUserDropDownModel userDropDownModel = new ForOrderDetailUserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(data["UserID"]);
                userDropDownModel.UserName = data["UserName"].ToString();
                userList.Add(userDropDownModel);
            }
            ViewBag.UserList = userList;

            SqlConnection connection2 = new SqlConnection(connectionString);
            connection2.Open();
            SqlCommand command2 = connection2.CreateCommand();
            command2.CommandType = System.Data.CommandType.StoredProcedure;
            command2.CommandText = "PR_Product_DropDown";
            SqlDataReader reader2 = command2.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            List<ForOrderDetailProductDropDownModel> productList = new List<ForOrderDetailProductDropDownModel>();
            foreach (DataRow data in dataTable2.Rows)
            {
                ForOrderDetailProductDropDownModel productDropDownModel = new ForOrderDetailProductDropDownModel();
                productDropDownModel.ProductID = Convert.ToInt32(data["ProductID"]);
                productDropDownModel.ProductName = data["ProductName"].ToString();
                productList.Add(productDropDownModel);
            }
            ViewBag.ProductList = productList;

          //  string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection3 = new SqlConnection(connectionString);
            connection3.Open();
            SqlCommand command3 = connection3.CreateCommand();
            command3.CommandType = System.Data.CommandType.StoredProcedure;
            command3.CommandText = "PR_Order_DropDown";
            SqlDataReader reader3 = command3.ExecuteReader();
            DataTable dataTable3 = new DataTable();
            dataTable3.Load(reader3);
            List<ForOrderDropDownModel> orderList = new List<ForOrderDropDownModel>();
            foreach (DataRow data in dataTable3.Rows)
            {
                ForOrderDropDownModel orderDropDownModel = new ForOrderDropDownModel();
                orderDropDownModel.OrderId = Convert.ToInt32(data["OrderId"]);
                orderList.Add(orderDropDownModel);
            }
            ViewBag.OrderList = orderList;

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "usp_SelectOrderDetailByID";  // Stored Procedure name for OrderDetail
            command.Parameters.AddWithValue("@OrderDetailID", orderDetailID);  // Adding parameter for OrderDetailID

            // Executing the stored procedure and retrieving the data
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);

            // Creating an instance of OrderDetailModel
            OrderDetailModel orderDetailModel = new OrderDetailModel();

            // Mapping the DataRow values to the OrderDetailModel properties
            foreach (DataRow dataRow in table.Rows)
            {
                orderDetailModel.OrderDetailID = dataRow["OrderDetailID"] != DBNull.Value ? (int?)Convert.ToInt32(dataRow["OrderDetailID"]) : null;
                orderDetailModel.OrderID = Convert.ToInt32(dataRow["OrderID"]);
                orderDetailModel.ProductID = Convert.ToInt32(dataRow["ProductID"]);
                orderDetailModel.Quantity = Convert.ToInt32(dataRow["Quantity"]);
                orderDetailModel.Amount = Convert.ToDouble(dataRow["Amount"]);
                orderDetailModel.TotalAmount = Convert.ToDouble(dataRow["TotalAmount"]);
                orderDetailModel.UserID = Convert.ToInt32(dataRow["UserID"]);
            }

            // Closing the connection
            connection.Close();

          
            return View("OrderDetailAddEdit", orderDetailModel);
           // return View();
        }
        #endregion

        [HttpPost]
        #region OrderDetailDelete
        public IActionResult OrderDetailDelete(int OrderDetailID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.CommandText = "usp_DeleteOrderDetail";
                sqlCommand.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = OrderDetailID;
                sqlCommand.ExecuteNonQuery();
                TempData["OrderDetailsDeleted"] = "OrderDetails deleted successfully!";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                TempData["OrderDetailNotDeleted"] = "Foreign Key Error OrderDetail deletion Failed!";
            }
            return RedirectToAction("OrderDetailList");
        }
        #endregion
    }
}
