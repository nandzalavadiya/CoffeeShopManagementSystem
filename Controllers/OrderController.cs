using CoffeeShopManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace CoffeeShopManagementSystem.Controllers
{
    public class OrderController : Controller
    {

        private IConfiguration configuration;

        public OrderController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region OrderList
        public IActionResult OrderList()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "usp_SelectAllOrder";
            SqlDataReader reader = sqlCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            return View(dataTable);
        }
        #endregion


        #region OrderSave
        public IActionResult OrderSave(OrderModel orderModel)
        {
            if (orderModel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }
            if (orderModel.CustomerID <= 0)
            {
                ModelState.AddModelError("CustomerID", "A valid Customer is required.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                if (orderModel.OrderID == null)
                {
                    sqlCommand.CommandText = "usp_InsertOrder";
                    TempData["OrderInserted"] = "Order Insert successfully!";
                }
                else
                {
                    sqlCommand.CommandText = "usp_UpdateOrder";
                    sqlCommand.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderModel.OrderID;
                    TempData["OrderUpdated"] = "Order Updated successfully!";
                }
                sqlCommand.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = orderModel.OrderDate;
                sqlCommand.Parameters.Add("@CustomerID", SqlDbType.Int).Value = orderModel.CustomerID;
                sqlCommand.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = orderModel.PaymentMode;
                sqlCommand.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = orderModel.TotalAmount;
                sqlCommand.Parameters.Add("@ShippingAddress", SqlDbType.VarChar).Value = orderModel.ShippingAddress;
                sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = orderModel.UserID;
                sqlCommand.ExecuteNonQuery();
                return RedirectToAction("OrderList");
            }

            return View("OrderAddEdit", orderModel);
        }
        #endregion

        #region OrderAddEdit

        public IActionResult OrderAddEdit(int orderID=0)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_Customer_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<ForOrderCustomerDropDownModel> customerList = new List<ForOrderCustomerDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                ForOrderCustomerDropDownModel customerDropDownModel = new ForOrderCustomerDropDownModel();
                customerDropDownModel.CustomerID = Convert.ToInt32(data["CustomerID"]);
                customerDropDownModel.CustomerName = data["CustomerName"].ToString();
                customerList.Add(customerDropDownModel);
            }
            ViewBag.CustomerList = customerList;


            SqlConnection connection2 = new SqlConnection(connectionString);
            connection2.Open();
            SqlCommand command2 = connection2.CreateCommand();
            command2.CommandType = System.Data.CommandType.StoredProcedure;
            command2.CommandText = "PR_User_DropDown";
            SqlDataReader reader2 = command2.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            List<ForOrderUserDropDownModel> userList = new List<ForOrderUserDropDownModel>();
            foreach (DataRow data in dataTable2.Rows)
            {
                ForOrderUserDropDownModel userDropDownModel = new ForOrderUserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(data["UserID"]);
                userDropDownModel.UserName = data["UserName"].ToString();
                userList.Add(userDropDownModel);
            }
            ViewBag.UserList = userList;

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand("usp_SelectOrderByID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@OrderID", orderID);

            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);

            OrderModel orderModel = new OrderModel();

            // Step 4: Map DataRow values to OrderModel properties
            foreach (DataRow dataRow in table.Rows)
            {
                orderModel.OrderID = dataRow["OrderID"] != DBNull.Value ? (int?)Convert.ToInt32(dataRow["OrderID"]) : null;
                orderModel.OrderDate = Convert.ToDateTime(dataRow["OrderDate"]);
                orderModel.CustomerID = Convert.ToInt32(dataRow["CustomerID"]);
                orderModel.PaymentMode = dataRow["PaymentMode"].ToString();
                orderModel.TotalAmount = Convert.ToDouble(dataRow["TotalAmount"]);
                orderModel.ShippingAddress = dataRow["ShippingAddress"].ToString();
                orderModel.UserID = Convert.ToInt32(dataRow["UserID"]);
            }

            connection.Close();

         
            return View("OrderAddEdit", orderModel);

           // return View();
        }
        #endregion

        [HttpPost]
        #region OrderDelete
        public IActionResult OrderDelete(int OrderID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.CommandText = "usp_DeleteOrder";
                sqlCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
                sqlCommand.ExecuteNonQuery();
                TempData["OrderDeleted"] = "Order deleted successfully!";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                TempData["OrderNotDeleted"] = "Foreign Key Error Order deletion Failed!";
            }
            return RedirectToAction("OrderList");
        }
        #endregion
    }
}
