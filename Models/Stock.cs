using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Moksha_App.Models
{
    public class Stock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Auto-increment primary key
        public int StockId { get; set; }  // Primary Key for stock record

        [Required]
        public int MaterialId { get; set; }  // Foreign Key to Material

        [Required]
        public int TotalBags { get; set; }  // Total number of bags in stock

        [Required]
        [Column(TypeName = "decimal(18,2)")]  // Weight per bag
        public decimal Weight { get; set; }

        // Calculated property for total weight
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalWeight => TotalBags * Weight;  // Total weight = TotalBags * Weight per bag

        // Available stock after purchases or sales
        [Column(TypeName = "decimal(18,2)")]
        public decimal AvailableStock { get; set; }  // Track how much stock is available
        public bool isActive { get; set; }

        public decimal ExtraWeight { get; set; }
    }
    public class Stock_
    {

        public int StockId { get; set; }

        public int MaterialId { get; set; }

        public string ColorName { get; set; }

        public int TotalBags { get; set; }

        public decimal Weight { get; set; }

        public decimal TotalWeight => TotalBags * Weight;

        public decimal AvailableStock { get; set; }

        public bool isActive { get; set; }
    }

    public class stockListViewModel
    {
        public IEnumerable<Stock_> Stock { get; set; }
        public string SearchTerm { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
    public class StockModel
    {

        public int TotalBags { get; set; }
        public decimal Weight { get; set; }
        public decimal AvailableStock { get; set; }
        public Material Material { get; set; }

        // New properties for charts/table
        public List<BuyingRecord> BuyingHistory { get; set; }
        public List<MonthlyTrend> MonthlyTrends { get; set; }
    }

    public class BuyingRecord
    {
        public DateTime Date { get; set; }
        public Material Material { get; set; }
        public decimal Weight { get; set; }
        public decimal Cost { get; set; }
    }

    public class MonthlyTrend
    {
        public string Month { get; set; } // e.g., "Jan 2023"
        public decimal TotalWeight { get; set; }
        public decimal TotalCost { get; set; }
    }
}