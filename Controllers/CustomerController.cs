using CoffeeShopManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace CoffeeShopManagementSystem.Controllers
{
    public class CustomerController : Controller
    {

        private IConfiguration configuration;

        public CustomerController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }


        #region CustomerList
        public IActionResult CustomerList() 
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "usp_SelectAllCustomers";
            SqlDataReader reader = sqlCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            return View(dataTable);
        }
        #endregion


        #region CustomerAddEdit
        public IActionResult CustomerAddEdit(int customerID=0)
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
            List<ForCustomerUserDropDownModel> userList = new List<ForCustomerUserDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                ForCustomerUserDropDownModel userDropDownModel = new ForCustomerUserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(data["UserID"]);
                userDropDownModel.UserName = data["UserName"].ToString();
                userList.Add(userDropDownModel);
            }
            ViewBag.UserList = userList;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            // Creating the SqlCommand and setting the command type as Stored Procedure
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "usp_SelectCustomerByID";  // Stored Procedure name
            command.Parameters.AddWithValue("@CustomerID", customerID);  // Adding parameter for CustomerID

            // Executing the stored procedure and retrieving the data
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);

            // Creating an instance of CustomerModel
            CustomerModel customerModel = new CustomerModel();

            // Mapping the DataRow values to the CustomerModel properties
            foreach (DataRow dataRow in table.Rows)
            {
                customerModel.CustomerID = Convert.ToInt32(dataRow["CustomerID"]);
                customerModel.CustomerName = dataRow["CustomerName"].ToString();
                customerModel.HomeAddress = dataRow["HomeAddress"].ToString();
                customerModel.Email = dataRow["Email"].ToString();
                customerModel.MobileNo = dataRow["MobileNo"].ToString();
                customerModel.GST_NO = dataRow["GST_NO"] != DBNull.Value ? dataRow["GST_NO"].ToString() : null;
                customerModel.CityName = dataRow["CityName"].ToString();
                customerModel.Pincode = dataRow["Pincode"].ToString();
                customerModel.NetAmount = Convert.ToDouble(dataRow["NetAmount"]);
                customerModel.UserID = Convert.ToInt32(dataRow["UserID"]);
            }

            // Closing the connection
            connection.Close();
           
            // Returning the populated CustomerModel to the view
            return View("CustomerAddEdit", customerModel);
           // return View();
        }
        #endregion

        #region CustomerSave
        public IActionResult CustomerSave(CustomerModel customerModel)
        {
            if (customerModel.UserID <= 0)
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

                if (customerModel.CustomerID == null)
                {
                    sqlCommand.CommandText = "usp_InsertCustomer";
                }
                else
                {
                    sqlCommand.CommandText = "usp_UpdateCustomer";
                    sqlCommand.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerModel.CustomerID;
                    TempData["CustomerUpdated"] = "Customer Updated successfully!";
                }
                sqlCommand.Parameters.Add("@CustomerName", SqlDbType.VarChar).Value = customerModel.CustomerName;
                sqlCommand.Parameters.Add("@HomeAddress", SqlDbType.VarChar).Value = customerModel.HomeAddress;
                sqlCommand.Parameters.Add("@Email", SqlDbType.VarChar).Value = customerModel.Email;
                sqlCommand.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = customerModel.MobileNo;
                sqlCommand.Parameters.Add("@GST_NO", SqlDbType.VarChar).Value = customerModel.GST_NO;
                sqlCommand.Parameters.Add("@CityName", SqlDbType.VarChar).Value = customerModel.CityName;
                sqlCommand.Parameters.Add("@Pincode",SqlDbType.VarChar).Value=customerModel.Pincode;
                sqlCommand.Parameters.Add("@NetAmount",SqlDbType.Decimal).Value=customerModel.NetAmount;
                sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = customerModel.UserID;
                sqlCommand.ExecuteNonQuery();
                return RedirectToAction("CustomerList");
            }

            return View("CustomerAddEdit", customerModel);
        }
        #endregion
        [HttpPost]

        #region CustomerDelete
        public IActionResult CustomerDelete(int CustomerID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.CommandText = "usp_DeleteCustomer";
                sqlCommand.Parameters.Add("@CustomerID", SqlDbType.Int).Value = CustomerID;
                sqlCommand.ExecuteNonQuery();
                TempData["CustomerDeleted"] = "Customer deleted successfully!";
            }
            catch (Exception ex)
            {
             
                Console.WriteLine(ex.ToString());
                TempData["CustomerNotDeleted"] = "Foreign Key Error Customer deletion Failed!";

            }
            return RedirectToAction("CustomerList");
        }
        #endregion
    }
}
