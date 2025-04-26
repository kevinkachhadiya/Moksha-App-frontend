using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.Connections;

namespace Moksha_App.Models
{
    public class Party
    {
        public int Id { get; set; }

        public string P_Name { get; set; } = "";

        public string P_number { get; set; } = string.Empty;

        public bool IsActive { get; set; } 

        public P_t p_t { get; set; }
        public enum P_t
        {
            Supplier,
            Customer
        }
        public string P_Address { get; set; }="";
    }
}
