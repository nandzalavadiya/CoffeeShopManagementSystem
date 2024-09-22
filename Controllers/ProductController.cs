using CoffeeShopManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace CoffeeShopManagementSystem.Controllers
{
    [CheckAccess]
    public class ProductController : Controller
    {
        private IConfiguration configuration;

        public ProductController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }


        #region ProductList
        public IActionResult ProductList()
        {

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "usp_SelectAllProduct";
            SqlDataReader reader = sqlCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);

            return View(dataTable);
        }
        #endregion

        #region ProductSave

        public IActionResult ProductSave(ProductModel productModel)
        {
            if (productModel.UserID <= 0)
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

                if (productModel.ProductID == null) 
                {
                    sqlCommand.CommandText = "usp_InsertProduct";
                    TempData["ProductInsert"] = "Product Inserted successfully!";
                }
                else
                {
                    sqlCommand.CommandText = "usp_UpdateProduct";
                    sqlCommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = productModel.ProductID;
                    TempData["ProductUpdated"] = "Product Updated successfully!";
                }
                sqlCommand.Parameters.Add("@ProductName", SqlDbType.VarChar).Value = productModel.ProductName;
                sqlCommand.Parameters.Add("@ProductCode", SqlDbType.VarChar).Value = productModel.ProductCode;
                sqlCommand.Parameters.Add("@ProductPrice", SqlDbType.Decimal).Value = productModel.ProductPrice;
                sqlCommand.Parameters.Add("@Description", SqlDbType.VarChar).Value = productModel.Description;
                sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = productModel.UserID;
                sqlCommand.ExecuteNonQuery();
                return RedirectToAction("ProductList");
            }
            return View("ProductAddEdit", productModel);
        }
        #endregion


        #region ProductAddEdit
        public IActionResult ProductAddEdit(int productID=0)
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
            List<ForProductUserDropDownModel> userList = new List<ForProductUserDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                ForProductUserDropDownModel userDropDownModel = new ForProductUserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(data["UserID"]);
                userDropDownModel.UserName = data["UserName"].ToString();
                userList.Add(userDropDownModel);
            }
            ViewBag.UserList = userList;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "usp_SelectProductByID";  
            command.Parameters.AddWithValue("@ProductID", productID); 

            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);

            ProductModel productModel = new ProductModel();

            foreach (DataRow dataRow in table.Rows)
            {
                productModel.ProductID = dataRow["ProductID"] != DBNull.Value ? (int?)Convert.ToInt32(dataRow["ProductID"]) : null;
                productModel.ProductName = dataRow["ProductName"].ToString();
                productModel.ProductPrice = Convert.ToDouble(dataRow["ProductPrice"]);
                productModel.ProductCode = dataRow["ProductCode"].ToString();
                productModel.Description = dataRow["Description"].ToString();
                productModel.UserID = Convert.ToInt32(dataRow["UserID"]);
            }

            connection.Close();

           
            return View("ProductAddEdit", productModel);

        }
        #endregion
        [HttpPost]

        #region ProductDelete 
        public IActionResult ProductDelete(int ProductID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.CommandText = "usp_DeleteProduct";
                sqlCommand.Parameters.Add("@ProductID", SqlDbType.Int).Value = ProductID;
                sqlCommand.ExecuteNonQuery();
                TempData["ProductDeleted"] = "Product deleted successfully!";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                TempData["ProductNotDeleted"] = "Foreign Key Error Product deletion Failed!";

            }
            return RedirectToAction("ProductList");
        }
        #endregion
    }
}



