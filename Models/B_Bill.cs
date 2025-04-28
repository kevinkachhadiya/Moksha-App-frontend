using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static Moksha_App.Models.B_Bill;

namespace Moksha_App.Models
{
    public class B_Bill
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Auto-increment primary key
        public int B_Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string BillNo { get; set; }

        [Required]
        [MaxLength(100)]
        public string BuyerName { get; set; }

        public List<B_BillItem> Items { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalBillPrice { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public PaymentMethodType PaymentMethod { get; set; }

        public bool IsPaid { get; set; }

        public Boolean IsActive { get; set; }

        public B_Bill()
        {
            CreatedAt = DateTime.UtcNow;
        }
        public enum PaymentMethodType
        {
            Cash,
            CreditCard,
            BankTransfer
        }
        public string P_number { get; set; } = string.Empty;
    }

    public class B_BillItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Auto-increment primary key
        public int Id { get; set; }

        [Required]
        public int MaterialId { get; set; }

        [Required]
        public decimal Quantity { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18,2)")]  // Decimal type for price with precision
        public decimal Price { get; set; }  // Price at which the material is purchased

        // Navigation property to Material
        public Material Material { get; set; }

        // Calculating Total Price for the item
        public decimal TotalPrice => Price * Quantity;

        // Fetch the ColorName from Material for each item
        public string ColorName => Material?.ColorName;

    }

    public class Create_B_Bill_Dto
    {
        public string BuyerName { get; set; } = "";
        public bool IsPaid { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public List<B_BillItemDto> Items { get; set; }

        public string P_number { get; set; } = string.Empty;
    }

    public class B_BillItemDto
        {
        public int MaterialId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class Edit_B_Bill_Dto
    {
        public int id { get; set; }
        public string? BuyerName { get; set; }

        public string P_number { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public List<B_BillItemDto>? Items { get; set; }

    }

    public class BillListViewModel
    {
        public IEnumerable<B_Bill> Bills { get; set; }
        public string SearchTerm { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
}

