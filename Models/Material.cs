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
    }

  
    
}
