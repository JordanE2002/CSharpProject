using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CSharpProject.Data;
using CSharpProject.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace CSharpProject.Pages
{
    public class EmployeesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 6; // number of employees per page

        public EmployeesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Employee> Employees { get; set; } = new List<Employee>();

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }   // <-- NEW

        public int TotalPages { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<Employee> query = _context.Employees
                                                 .Include(e => e.Company)
                                                 .Where(e => !e.IsDeleted); // <-- Only active employees

            // ✅ Filtering (case-insensitive, matches first OR last OR full name)
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                string lowerSearch = SearchTerm.ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(lowerSearch) ||
                    e.LastName.ToLower().Contains(lowerSearch) ||
                    (e.FirstName + " " + e.LastName).ToLower().Contains(lowerSearch));
            }

            // Sorting
            switch (SortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(e => e.FirstName);
                    break;
                default:
                    query = query.OrderBy(e => e.FirstName);
                    break;
            }

            // Count total items for pagination (after filtering)
            int totalEmployees = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalEmployees / (double)PageSize);

            if (PageNumber < 1) PageNumber = 1;
            if (PageNumber > TotalPages && TotalPages > 0) PageNumber = TotalPages;

            // Pagination
            Employees = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
