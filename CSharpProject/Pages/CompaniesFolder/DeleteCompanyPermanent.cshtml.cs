using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CSharpProject.Data;
using CSharpProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CSharpProject.Pages.CompaniesFolder
{
    public class DeleteCompanyPermanentModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteCompanyPermanentModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Company Company { get; set; }

        public bool HasEmployees { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Company = await _context.Companies
                                    .Include(c => c.Employees)
                                    .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted);

            if (Company == null)
                return RedirectToPage("/CompaniesFolder/RestoreCompany");

            HasEmployees = Company.Employees.Any();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var company = await _context.Companies
                                        .Include(c => c.Employees)
                                        .FirstOrDefaultAsync(c => c.Id == Company.Id);

            if (company == null)
                return RedirectToPage("/CompaniesFolder/RestoreCompany");

            if (company.Employees?.Any() == true)
                _context.Employees.RemoveRange(company.Employees);

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return RedirectToPage("/CompaniesFolder/RestoreCompany");
        }
    }
}
