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

            HasEmployees = _context.Employees.Any(e => e.CompanyId == Company.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var company = await _context.Companies.FindAsync(Company.Id);
            if (company == null)
            {
                return RedirectToPage("/Companies");
            }

            var hasEmployees = _context.Employees.Any(e => e.CompanyId == company.Id);
            if (hasEmployees)
            {
                ModelState.AddModelError(string.Empty, "Cannot delete this company while it has assigned employees. Please remove or reassign them first.");
                Company = company; // Rebind to show on form again
                HasEmployees = true;
                return Page();
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Companies");
        }
    }
}
