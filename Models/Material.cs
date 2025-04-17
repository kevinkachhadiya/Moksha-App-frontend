using Moksha_App.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moksha_App.Models
{
    // Material class
    public class Material
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // Auto-increment in SQL Server
        public int Id { get; set; }  // Primary Key

        [Required(ErrorMessage = "Color Name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Color Name must be between 3 and 50 characters.")]
        public string ColorName { get; set; }  // Color of the material

        [Required(ErrorMessage = "Base Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Base Price must be a positive number.")]
        public decimal BasePrice { get; set; }  // Base price of the material
        public bool IsActive { get; set; }

    }
    public class MaterialsListViewModel
    {
        public IEnumerable<Material> Materials { get; set; }
        public string SearchTerm { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }

}

