using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CSharpProject.Data;
using EmployeeModel = CSharpProject.Models.Employee;  // alias to fix conflict
using System.Linq;

namespace CSharpProject.Pages.EmployeesFolder  // matches folder structure
{
    public class CreateEmployeeModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateEmployeeModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EmployeeModel Employee { get; set; } = new EmployeeModel();

        public SelectList Companies { get; set; }

        public void OnGet()
        {
            Companies = new SelectList(_context.Companies.ToList(), "Id", "Name");
        }

        public IActionResult OnPost()
        {
            Companies = new SelectList(_context.Companies.ToList(), "Id", "Name");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Employees.Add(Employee);
            _context.SaveChanges();
            return RedirectToPage("/Employees");
        }
    }
}