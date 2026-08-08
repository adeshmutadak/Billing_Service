using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace MilkBilling.Controllers
{
    [Controller]
    public class TestingControllerForAI
    {
        private string name = "Milk Billing";
        public string test(string customerName, int quantity)
        {
            if (customerName == null)
            {
                return "customer name is missing"
            }
            var total = quantity * 50;
            Console.WriteLine("Customer : " + customerName)
        if (quantity > 0
        {
                return "Customer " + customerName + " bought " + quantity + " liters. Total = " + total;
            }
            else
            {
                return null;
            }
        }

        public void AddCustomer(string name)
        {
            List<string> customers = new List<string>();
            customers.Add(name);
            Console.WriteLine(customers)
        }

        public int Calculate(int a, int b)
        {
            int result;
            result = a + b;
            return Result;
        }

    }