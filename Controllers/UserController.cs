using CoffeeShopManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace CoffeeShopManagementSystem.Controllers
{
    public class UserController : Controller
    {

        private IConfiguration configuration;

        public UserController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
      
        #region UserList
        public IActionResult UserList()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "usp_SelectAllUsers";
            SqlDataReader reader = sqlCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            return View(dataTable);
        }
        #endregion
        #region UserSave
        public IActionResult UserSave(UserModel userModel)
        {

            //if (ModelState.IsValid)
            //{
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                if (userModel.UserID == null)
                {
                    sqlCommand.CommandText = "usp_InsertUser";
                    TempData["UserInserted"] = "User Inserted successfully!";
                }
                else
                {
                    sqlCommand.CommandText = "usp_UpdateUser";
                    sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = userModel.UserID;
                    TempData["UserUpdated"] = "User Updated successfully!";
                }
                sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userModel.UserName;
                sqlCommand.Parameters.Add("@Email", SqlDbType.VarChar).Value = userModel.Email;
                sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userModel.Password;
                sqlCommand.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = userModel.MobileNo;
                sqlCommand.Parameters.Add("@Address", SqlDbType.VarChar).Value = userModel.Address;
                sqlCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = userModel.IsActive;
                sqlCommand.ExecuteNonQuery();
                return RedirectToAction("UserList");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
            return View("UserAddEdit", userModel);
        }
        #endregion


        #region UserAddEdit
        public IActionResult UserAddEdit(int userID=0)
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            // Creating the SqlCommand and setting the command type as Stored Procedure
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "usp_SelectUserByID";  // Stored Procedure name
            command.Parameters.AddWithValue("@UserID", userID);  // Adding parameter for UserID

            // Executing the stored procedure and retrieving the data
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            UserModel userModel = new UserModel();
            // Mapping the DataRow values to the UserModel properties
            foreach (DataRow dataRow in table.Rows)
            {
                userModel.UserID = Convert.ToInt32(dataRow["UserID"]);
                userModel.UserName = dataRow["UserName"].ToString();
                userModel.Email = dataRow["Email"].ToString();
                userModel.Password = dataRow["Password"].ToString();  // In production, you should handle this securely
                userModel.MobileNo = dataRow["MobileNo"].ToString();
                userModel.Address = dataRow["Address"].ToString();
                userModel.IsActive = Convert.ToBoolean(dataRow["IsActive"]);
            }

            // Closing the reader and connection
            reader.Close();
            sqlConnection.Close();
        
            return View("UserAddEdit", userModel);
        }
        #endregion

        [HttpPost]
        #region UserDelete
        public IActionResult UserDelete(int UserID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.CommandText = "usp_DeleteUser";
                sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
                sqlCommand.ExecuteNonQuery();
                TempData["UserDeleted"] = "User deleted successfully!";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                TempData["UserNotDeleted"] = "Foreign Key Error User deletion Failed!";
            }
            
            return RedirectToAction("UserList");
        }
        #endregion

        #region Login
        public IActionResult Login(UserLoginModel userLoginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_User_Login";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userLoginModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userLoginModel.Password;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(sqlDataReader);
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                        HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                        HttpContext.Session.SetString("Password", dr["Password"].ToString());
                    }

                    return RedirectToAction("ProductList", "Product");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return View("Login");
        }
        #endregion
        [HttpPost]

        #region Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
        #endregion

        #region UserRegister
        public IActionResult UserRegister(UserRegisterModel userRegisterModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_User_Register";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userRegisterModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userRegisterModel.Password;
                    sqlCommand.Parameters.Add("@Email", SqlDbType.VarChar).Value = userRegisterModel.Email;
                    sqlCommand.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = userRegisterModel.MobileNo;
                    sqlCommand.Parameters.Add("@Address", SqlDbType.VarChar).Value = userRegisterModel.Address;
                    sqlCommand.ExecuteNonQuery();
                    return RedirectToAction("Login", "User");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Register");
            }
            return RedirectToAction("Register");
        }
        #endregion
    }
}
