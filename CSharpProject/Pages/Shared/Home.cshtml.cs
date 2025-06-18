using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CSharpProject.Data;
using CSharpProject.Models;
using System.Collections.Generic;
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

        public List<Company> Companies { get; set; } = new();

        public async Task OnGetAsync()
        {
            Companies = await _context.Companies
                .Include(c => c.Employees)
                .ToListAsync();
        }
    }
}
