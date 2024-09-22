using CoffeeShopManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace CoffeeShopManagementSystem.Controllers
{
    public class BillsController : Controller
    {

        private IConfiguration configuration;

        public BillsController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }


        #region BillsList

        public IActionResult BillsList()
        {

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "usp_SelectAllBill";
            SqlDataReader reader = sqlCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            return View(dataTable);
        }

        #endregion

        #region BillsAddEdit
        public IActionResult BillsAddEdit(int billID = 0)
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
            List<ForBillsUserDropDownModel> userList = new List<ForBillsUserDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                ForBillsUserDropDownModel userDropDownModel = new ForBillsUserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(data["UserID"]);
                userDropDownModel.UserName = data["UserName"].ToString();
                userList.Add(userDropDownModel);
            }
            ViewBag.UserList = userList;

            
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

            SqlCommand command = new SqlCommand("usp_SelectBillByID", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            BillsModel billModel = new BillsModel();
            command.Parameters.AddWithValue("@BillID", billID);

            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);

            // Step 3: Map DataRow values to BillsModel properties
            foreach (DataRow dataRow in table.Rows)
            {
                billModel.BillID = dataRow["BillID"] != DBNull.Value ? (int?)Convert.ToInt32(dataRow["BillID"]) : null;
                billModel.BillNumber = dataRow["BillNumber"].ToString();
                billModel.BillDate = Convert.ToDateTime(dataRow["BillDate"]);
                billModel.OrderID = Convert.ToInt32(dataRow["OrderID"]);
                billModel.TotalAmount = Convert.ToDouble(dataRow["TotalAmount"]);
                billModel.Discount = Convert.ToDouble(dataRow["Discount"]);
                billModel.NetAmount = Convert.ToDouble(dataRow["NetAmount"]);
                billModel.UserID = Convert.ToInt32(dataRow["UserID"]);
            }

            connection.Close();
           
            return View("BillsAddEdit", billModel);
        }
        #endregion

        [HttpPost]
        #region BillsSave
        public IActionResult BillsSave(BillsModel billsModel)
        {
            if (billsModel.OrderID <= 0)
            {
                ModelState.AddModelError("OrderID", "A valid Order is required.");
            }
            if (billsModel.UserID <= 0)
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

                if (billsModel.BillID == null)
                {
                    sqlCommand.CommandText = "usp_InsertBill";
                }
                else
                {
                    sqlCommand.CommandText = "usp_UpdateBill";
                    sqlCommand.Parameters.Add("@BillID", SqlDbType.Int).Value = billsModel.BillID;
                    TempData["BillsUpdated"] = "Bill Updated successfully!";
                }
                sqlCommand.Parameters.Add("@BillNumber", SqlDbType.VarChar).Value = billsModel.BillNumber;
                sqlCommand.Parameters.Add("@BillDate", SqlDbType.DateTime).Value = billsModel.BillDate;
                sqlCommand.Parameters.Add("@OrderID", SqlDbType.Int).Value = billsModel.OrderID;
                sqlCommand.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = billsModel.TotalAmount;
                sqlCommand.Parameters.Add("@Discount", SqlDbType.Decimal).Value = billsModel.Discount;
                sqlCommand.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = billsModel.NetAmount;
                sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = billsModel.UserID;
                sqlCommand.ExecuteNonQuery();
                return RedirectToAction("BillsList");
            }

            return View("BillsAddEdit", billsModel);


        }
        #endregion

        [HttpPost]
        #region BiilsDelete
        public IActionResult BiilsDelete(int BillID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.CommandText = "usp_DeleteBill";
                sqlCommand.Parameters.Add("@BillId", SqlDbType.Int).Value = BillID;
                sqlCommand.ExecuteNonQuery();
                TempData["BillsDeleted"] = "Bill deleted successfully!";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                TempData["BillsNotDeleted"] = "Foreign Key Error Bill deletion Failed!";

            }
            return RedirectToAction("BillsList");
        }
        #endregion
    }
}
