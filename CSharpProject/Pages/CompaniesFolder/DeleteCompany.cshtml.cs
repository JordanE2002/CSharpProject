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

        public bool HasEmployees { get; set; }

        public IActionResult OnGet(int id)
        {
            Company = _context.Companies.FirstOrDefault(c => c.Id == id);
            if (Company == null)
            {
                return RedirectToPage("/Companies");
            }

            // Check if company has active (non-archived) employees
            HasEmployees = _context.Employees.Any(e => e.CompanyId == Company.Id && !e.IsDeleted);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var company = await _context.Companies.FindAsync(Company.Id);
            if (company == null)
            {
                return RedirectToPage("/Companies");
            }

            // Block delete if still has active employees
            var hasEmployees = _context.Employees.Any(e => e.CompanyId == company.Id && !e.IsDeleted);
            if (hasEmployees)
            {
                ViewData["ErrorMessage"] = "Cannot delete this company while it has active employees. Please reassign or remove them first.";
                Company = company;
                HasEmployees = true;
                return Page();
            }

            // Soft delete
            company.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToPage("/Companies");
        }
    }
}
