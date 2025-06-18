using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CSharpProject.Data;
using CSharpProject.Models;
using System.Threading.Tasks;

namespace CSharpProject.Pages.EmployeesFolder
{
    public class UpdateEmployeeModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public UpdateEmployeeModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee? Employee { get; set; }

        public SelectList Companies { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Employee = await _context.Employees.FindAsync(id);

            if (Employee == null)
            {
                return NotFound();
            }

            Companies = new SelectList(await _context.Companies.ToListAsync(), "Id", "Name", Employee.CompanyId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Companies = new SelectList(await _context.Companies.ToListAsync(), "Id", "Name");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Employee == null)
            {
                return BadRequest();
            }

            var employeeToUpdate = await _context.Employees.FindAsync(Employee.Id);

            if (employeeToUpdate == null)
            {
                return NotFound();
            }

            // Update fields
            employeeToUpdate.FirstName = Employee.FirstName;
            employeeToUpdate.LastName = Employee.LastName;
            employeeToUpdate.Email = Employee.Email;
            employeeToUpdate.PhoneNumber = Employee.PhoneNumber;
            employeeToUpdate.CompanyId = Employee.CompanyId;

            await _context.SaveChangesAsync();

            return RedirectToPage("/Employees");
        }
    }
}
