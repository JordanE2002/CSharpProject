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

        private const int PageSize = 3; // number of employees per page

        public EmployeesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Employee> Employees { get; set; } = new List<Employee>();

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int TotalPages { get; set; }

        public async Task OnGetAsync()
        {
            // ✅ Include the related Company in the query
            IQueryable<Employee> query = _context.Employees
                                                 .Include(e => e.Company);

            // Sorting logic
            switch (SortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(e => e.FirstName);
                    break;
                default:
                    query = query.OrderBy(e => e.FirstName);
                    break;
            }

            // Count total items for pagination
            int totalEmployees = await query.CountAsync();

            // Calculate total pages
            TotalPages = (int)Math.Ceiling(totalEmployees / (double)PageSize);

            // Ensure page number is in range
            if (PageNumber < 1) PageNumber = 1;
            if (PageNumber > TotalPages) PageNumber = TotalPages;

            // Pagination: skip and take
            Employees = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
