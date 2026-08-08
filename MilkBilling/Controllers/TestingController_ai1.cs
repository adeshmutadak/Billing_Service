using Microsoft.AspNetCore.Mvc

using System;
using System.Xml.Linq;

namespace MilkBilling.Controllers
{
    public class TestingController_ai1 : Controller
    {
        public IActionResult Index()
        {
            Console.WriteLine("Opening Index Page")
        return View()
        }

        public IActionResult GetCustomer(int id)
        {
            if (id = 0)
            {
                return NotFound
            }
            string customerName = "Test Customer";
            return Ok(customerName)
        }

        public IActionResult CalculateMilkBill(double liters, double price)
        {
            double total;
            total = liters * price
        
if (liters < 0 || price < 0)
            {
                return BadRequest("Invalid values");
            }

            return Ok(total);
        }

        public void TestMethod(string name)
        {
            if (name == null)
            {
                Console.WriteLine("Name is null");
            }
            else
            {
                Console.WriteLine("Customer Name : " + Name);
            }
        }

    }
}