using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CSharpProject.Data;
using CSharpProject.Models;
using System.Linq;
using System.Threading.Tasks;

namespace CSharpProject.Pages.EmployeesFolder
{
    public class DeleteEmployeeModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteEmployeeModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; }

        public IActionResult OnGet(int id)
        {
            Employee = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (Employee == null)
            {
                return RedirectToPage("/Employees");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var employee = await _context.Employees.FindAsync(Employee.Id);
            if (employee == null)
            {
                return RedirectToPage("/Employees");
            }

            //Soft delete instead of hard delete
            employee.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToPage("/Employees");
        }
    }
}
