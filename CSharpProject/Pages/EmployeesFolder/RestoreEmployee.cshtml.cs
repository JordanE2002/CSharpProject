using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CSharpProject.Data;
using CSharpProject.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSharpProject.Pages.EmployeesFolder
{
    public class RestoreEmployeeModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RestoreEmployeeModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Employee> ArchivedEmployees { get; set; } = new List<Employee>();

        public async Task OnGetAsync()
        {
            // Get all soft-deleted employees + their company info
            ArchivedEmployees = await _context.Employees
                                              .Include(e => e.Company)
                                              .Where(e => e.IsDeleted)
                                              .ToListAsync();
        }

        public async Task<IActionResult> OnPostRestoreAsync(int id)
        {
            var employee = await _context.Employees
                                         .Include(e => e.Company)
                                         .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                TempData["RestoreMessage"] = "Employee not found.";
                return RedirectToPage();
            }

            // If company is missing (hard deleted), permanently delete employee
            if (employee.Company == null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                TempData["RestoreMessage"] = $"Employee {employee.FirstName} {employee.LastName} permanently deleted because their company no longer exists.";
                return RedirectToPage();
            }

            // If company exists but is soft-deleted, block restore
            if (employee.Company.IsDeleted)
            {
                TempData["RestoreMessage"] = $"Cannot restore {employee.FirstName} {employee.LastName} because their company ({employee.Company.Name}) is archived.";
                return RedirectToPage();
            }

            // ✅ Restore employee
            employee.IsDeleted = false;
            await _context.SaveChangesAsync();

            TempData["RestoreMessage"] = $"Employee {employee.FirstName} {employee.LastName} has been restored.";
            return RedirectToPage();
        }
    }
}
