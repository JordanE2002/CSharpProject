using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CSharpProject.Data;
using CSharpProject.Models;
using System.Threading.Tasks;

namespace CSharpProject.Pages.EmployeesFolder
{
    public class DeleteEmployeePermanentModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteEmployeePermanentModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Employee = await _context.Employees
                                     .Include(e => e.Company)
                                     .FirstOrDefaultAsync(e => e.Id == id);

            if (Employee == null)
            {
                return RedirectToPage("/EmployeesFolder/RestoreEmployee");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var employee = await _context.Employees.FindAsync(Employee.Id);

            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/EmployeesFolder/RestoreEmployee");
        }
    }
}
