using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CSharpProject.Data;
using CSharpProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CSharpProject.Pages.CompaniesFolder
{
    public class ViewEmployeesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ViewEmployeesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Employee> Employees { get; set; } = new List<Employee>();

        [BindProperty(SupportsGet = true)]
        public int CompanyId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (CompanyId == 0)
            {
                return NotFound();
            }

            Employees = await _context.Employees
                .Where(e => e.CompanyId == CompanyId)
                .ToListAsync();

            return Page();
        }
    }
}
