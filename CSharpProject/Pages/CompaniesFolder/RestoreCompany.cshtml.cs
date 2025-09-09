using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CSharpProject.Data;
using CSharpProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CSharpProject.Pages.CompaniesFolder
{
    public class RestoreCompanyModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RestoreCompanyModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Company> Companies { get; set; } = new List<Company>();

        public async Task OnGetAsync()
        {
            Companies = await _context.Companies
                                      .Where(c => c.IsDeleted)
                                      .ToListAsync();
        }

        public async Task<IActionResult> OnPostRestoreAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                company.IsDeleted = false;
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeletePermanentAsync(int id)
        {
            var company = await _context.Companies
                                        .Include(c => c.Employees) // include related employees
                                        .FirstOrDefaultAsync(c => c.Id == id);

            if (company != null)
            {
                // also remove employees belonging to this company
                if (company.Employees != null && company.Employees.Any())
                {
                    _context.Employees.RemoveRange(company.Employees);
                }

                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
