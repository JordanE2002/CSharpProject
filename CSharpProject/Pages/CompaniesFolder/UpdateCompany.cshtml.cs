using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CSharpProject.Data;
using CSharpProject.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Drawing;
using System.Text;

namespace CSharpProject.Pages.CompaniesFolder
{
    public class UpdateCompanyModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public UpdateCompanyModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Company Company { get; set; }

        [BindProperty]
        public IFormFile LogoFile { get; set; }

        public IActionResult OnGet(int id)
        {
            Company = _context.Companies.FirstOrDefault(c => c.Id == id);
            if (Company == null)
            {
                return RedirectToPage("/Companies");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var companyInDb = _context.Companies.FirstOrDefault(c => c.Id == Company.Id);
            if (companyInDb == null)
                return RedirectToPage("/Companies");

            companyInDb.Name = Company.Name;
            companyInDb.Email = Company.Email;
            companyInDb.Website = Company.Website;

            if (LogoFile != null && LogoFile.Length > 0)
            {
                try
                {
                    using var image = Image.FromStream(LogoFile.OpenReadStream());
                    if (image.Width < 100 || image.Height < 100)
                    {
                        ModelState.AddModelError("LogoFile", "Logo must be at least 100x100 pixels.");
                        return Page();
                    }

                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads", "Images");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Path.GetFileNameWithoutExtension(LogoFile.FileName)
                        + "_"
                        + Path.GetRandomFileName().Replace(".", "")
                        + Path.GetExtension(LogoFile.FileName);

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await LogoFile.CopyToAsync(fileStream);
                    }

                    companyInDb.Logo = $"/Uploads/Images/{uniqueFileName}";
                }
                catch
                {
                    ModelState.AddModelError("LogoFile", "Invalid image file.");
                    return Page();
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/Companies");
        }
    }
}
