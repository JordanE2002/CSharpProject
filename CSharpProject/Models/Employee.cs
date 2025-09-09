using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CSharpProject.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [StrictEmail]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please select a company.")]
        public int CompanyId { get; set; }

 

        [ValidateNever]
        public Company Company { get; set; }


        public bool IsDeleted { get; set; } = false;



    }
}
