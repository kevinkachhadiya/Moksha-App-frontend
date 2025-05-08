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
    public class PartyListViewModel
    {
        public List<Party> Party {get;set;}
        public string party_ { get; set; }
        public string SortColumn { get; set; } = "P_Name";
        public string sortDirection { get; set; } = "asc";
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }

    public class States
    {
        public int Id { get; set; }
        public string StateName { get; set; } = "";

    }
}
