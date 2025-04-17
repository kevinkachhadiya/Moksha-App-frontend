using Microsoft.AspNetCore.Mvc;
using Moksha_App.Models;


namespace Moksha_App.Controllers
{
    public class DashBoardController : Controller
    {
        public IActionResult Dash_Board()
        {

            var data = new StockModel
            {
                // Core Stock Properties
                TotalBags = 150,
                Weight = 1250.75m,
                AvailableStock = 980.25m,

                // Material initialization (FIXED: Added 'new Material()')
                Material = new Material
                {
                    Id = 1,
                    ColorName = "Royal Blue",
                    BasePrice = 10,
                    IsActive = true
                },

                // Buying History
                BuyingHistory = new List<BuyingRecord>
    {
        new BuyingRecord
        {
            Date = DateTime.Now.AddDays(-2),
            Material = new Material
            {
                ColorName = "Royal Blue",
                BasePrice = 10,
                Id = 1,
                IsActive = true
            },
            Weight = 200.5m,
            Cost = 1200.00m
        },
        new BuyingRecord
        {
            Date = DateTime.Now.AddDays(-7),
            Material = new Material
            {
                ColorName = "Royal Blue",
                BasePrice = 10,
                Id = 1,
                IsActive = true
            },
            Weight = 150.0m,
            Cost = 900.00m
        },
        new BuyingRecord
        {
            Date = DateTime.Now.AddDays(-15),
            Material = new Material
            {
                ColorName = "Royal Blue",
                BasePrice = 10,
                Id = 1,
                IsActive = true
            },
            Weight = 300.0m,
            Cost = 1800.00m
        },
        new BuyingRecord
        {
            Date = DateTime.Now.AddDays(-30),
            Material = new Material
            {
                ColorName = "Royal Blue",
                BasePrice = 10,
                Id = 1,
                IsActive = true
            },
            Weight = 250.0m,
            Cost = 1500.00m
        },
        new BuyingRecord
        {
            Date = DateTime.Now.AddDays(-45),
            Material = new Material
            {
                ColorName = "Royal Blue",
                BasePrice = 10,
                Id = 1,
                IsActive = true
            },
            Weight = 350.75m,
            Cost = 2104.50m
        }
    },

                // Monthly Trends
                MonthlyTrends = new List<MonthlyTrend>
    {
        new MonthlyTrend { Month = "Jan 2023", TotalWeight = 450.0m, TotalCost = 2700.00m },
        new MonthlyTrend { Month = "Feb 2023", TotalWeight = 600.5m, TotalCost = 3603.00m },
        new MonthlyTrend { Month = "Mar 2023", TotalWeight = 550.25m, TotalCost = 3301.50m },
        new MonthlyTrend { Month = "Apr 2023", TotalWeight = 700.0m, TotalCost = 4200.00m },
        new MonthlyTrend { Month = "May 2023", TotalWeight = 800.75m, TotalCost = 4804.50m },
        new MonthlyTrend { Month = "Jun 2023", TotalWeight = 650.5m, TotalCost = 3903.00m }
    }
            };

            return View(data);
        }
    }
}
