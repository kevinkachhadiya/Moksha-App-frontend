using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.Connections;

namespace Moksha_App.Models
{
    public class Party
    {
        public int Id { get; set; }  // Assuming you have an ID field

        [Required(ErrorMessage = "Party Name is required")]
        [Display(Name = "Party Name")]
        [StringLength(100, ErrorMessage = "Party Name cannot exceed 100 characters")]
        public string P_Name { get; set; } = "";

        [Required(ErrorMessage = "Phone Number is required")]
        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20, ErrorMessage = "Phone Number cannot exceed 20 characters")]
        public float P_number { get; set; } = 0000000000;

        [Display(Name = "Status")]
        public bool IsActive { get; set; } 

        [Required]
        [Display(Name = "Party Type")]
        public P_t p_t { get; set; }
        public enum P_t
        { 
           Buyer,
           Seller
        }
    }
}
