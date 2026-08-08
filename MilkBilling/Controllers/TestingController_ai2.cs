using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using System;
using System.Data;

namespace MilkBilling.Controllers
{
    public class TestingController_ai2 : Controller
    {
        private string connectionString =
            "Server=localhost;Database=MilkBilling;User Id=admin;Password=Admin@123;";

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetCustomer(string customerName)
        {
            var sql = "SELECT * FROM Customers WHERE Name = '" + customerName + "'";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(sql, con);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return Ok(new
                    {
                        Id = reader["Id"],
                        Name = reader["Name"],
                        Phone = reader["Phone"],
                        Address = reader["Address"]
                    });
                }
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "Admin@123")
            {
                return Ok("Login successful");
            }

            return Unauthorized();
        }

        [HttpPost]
        public IActionResult DeleteCustomer(int id)
        {
            var sql = "DELETE FROM Customers WHERE Id = " + id;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
            }

            return Ok("Customer deleted");
        }

        [HttpPost]
        public IActionResult UpdateCustomer(int id, string name, string phone)
        {
            var sql = "UPDATE Customers SET Name='" + name +
                      "', Phone='" + phone +
                      "' WHERE Id=" + id;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
            }

            return Ok("Customer updated");
        }

        [HttpGet]
        public IActionResult GetCustomerDetails(int id)
        {
            var sql = "SELECT * FROM Customers WHERE Id=" + id;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(sql, con);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return Ok(new
                    {
                        Id = reader["Id"],
                        Name = reader["Name"],
                        Phone = reader["Phone"],
                        Password = reader["Password"]
                    });
                }
            }

            return NotFound();
        }
    }
}