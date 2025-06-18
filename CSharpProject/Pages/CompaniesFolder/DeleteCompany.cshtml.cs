using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CSharpProject.Data;
using CSharpProject.Models;
using System.Threading.Tasks;
using System.Linq;

namespace CSharpProject.Pages.CompaniesFolder
{
    public class DeleteCompanyModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteCompanyModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Company Company { get; set; }

        public IActionResult OnGet(int id)
        {
            Company = _context.Companies.FirstOrDefault(c => c.Id == id);
            if (Company == null)
            {
                return RedirectToPage("/Companies");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Find the company
            var company = await _context.Companies.FindAsync(Company.Id);
            if (company == null)
            {
                return RedirectToPage("/Companies");
            }

            // Get and delete all employees linked to this company
            var employees = _context.Employees.Where(e => e.CompanyId == company.Id);
            _context.Employees.RemoveRange(employees);

            // Delete the company
            _context.Companies.Remove(company);

            // Commit changes to the database
            await _context.SaveChangesAsync();

            return RedirectToPage("/Companies");
        }
    }
}
