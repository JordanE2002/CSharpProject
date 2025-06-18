using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CSharpProject.Data;
using CSharpProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;

namespace CSharpProject.Pages
{
    public class CompaniesModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        private const int PageSize = 6; // items per page

        public CompaniesModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Company> Companies { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int TotalPages { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<Company> companiesQuery = _context.Companies;

            switch (SortOrder)
            {
                case "name_desc":
                    companiesQuery = companiesQuery.OrderByDescending(c => c.Name);
                    break;
                default:
                    companiesQuery = companiesQuery.OrderBy(c => c.Name);
                    break;
            }

            int totalCompanies = await companiesQuery.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCompanies / (double)PageSize);

            if (PageNumber < 1) PageNumber = 1;
            if (PageNumber > TotalPages) PageNumber = TotalPages;

            Companies = await companiesQuery
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
