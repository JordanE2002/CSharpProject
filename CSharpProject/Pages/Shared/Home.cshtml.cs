using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CSharpProject.Data;
using CSharpProject.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSharpProject.Pages
{
    public class HomeModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public HomeModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Company> LatestCompanies { get; set; } = new();
        public List<Employee> LatestEmployees { get; set; } = new();

        public async Task OnGetAsync()
        {
            LatestCompanies = await _context.Companies
                .OrderByDescending(c => c.Id)  // Or CreatedDate if you have it
                .Take(5)
                .AsNoTracking()
                .ToListAsync();

            LatestEmployees = await _context.Employees
                .Include(e => e.Company)
                .OrderByDescending(e => e.Id)  // Or CreatedDate if available
                .Take(5)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
