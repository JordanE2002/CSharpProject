using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CSharpProject.Data;
using EmployeeModel = CSharpProject.Models.Employee;  // alias to fix conflict
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> OnPostAsync()
        {
            Companies = new SelectList(_context.Companies.ToList(), "Id", "Name");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ✅ Check for existing email (case-insensitive)
            var emailExists = await _context.Employees
                .AnyAsync(e => e.Email.ToLower() == Employee.Email.ToLower());

            if (emailExists)
            {
                ModelState.AddModelError("Employee.Email", "An employee with this email already exists.");
                return Page();
            }

            _context.Employees.Add(Employee);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Employees");
        }
    }
}
