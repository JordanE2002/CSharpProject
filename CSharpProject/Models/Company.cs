using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CSharpProject.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [StrictEmail]
        public string Email { get; set; }

        [ValidateNever]
        public string Logo { get; set; }

        [Url]
        public string Website { get; set; }

        [ValidateNever]

        // ✅ Soft delete flag
        public bool IsDeleted { get; set; } = false;
        public IList<Employee> Employees { get; set; } = new List<Employee>();

    }
}
