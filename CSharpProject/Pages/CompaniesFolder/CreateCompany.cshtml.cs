using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CSharpProject.Data;
using CSharpProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using System.Text;

namespace CSharpProject.Pages.CompaniesFolder
{
    public class CreateCompanyModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateCompanyModel(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public Company Company { get; set; } = new Company();

        [BindProperty]
        [Required(ErrorMessage = "Please submit a logo")]
        public IFormFile LogoFile { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var errors = new StringBuilder();
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        errors.AppendLine($"{state.Key}: {error.ErrorMessage}");
                    }
                }

                System.Diagnostics.Debug.WriteLine("ModelState is invalid:");
                System.Diagnostics.Debug.WriteLine(errors.ToString());

                ModelState.AddModelError(string.Empty, "Validation failed: Please correct the errors below.");
                return Page();
            }

            if (LogoFile == null || LogoFile.Length == 0)
            {
                ModelState.AddModelError("LogoFile", "Please submit a logo");
                return Page();
            }

            // ✅ Email uniqueness check
            var existingCompany = await _context.Companies
                .FirstOrDefaultAsync(c => c.Email.ToLower() == Company.Email.ToLower());

            if (existingCompany != null)
            {
                ModelState.AddModelError("Company.Email", "A company with this email already exists.");
                return Page();
            }

            try
            {
                // Validate image dimensions
                using var image = Image.FromStream(LogoFile.OpenReadStream());
                if (image.Width < 100 || image.Height < 100)
                {
                    ModelState.AddModelError("LogoFile", "Logo must be at least 100x100 pixels.");
                    return Page();
                }

                // Save image
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", "Images");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Path.GetFileNameWithoutExtension(LogoFile.FileName)
                    + "_" + Path.GetRandomFileName().Replace(".", "")
                    + Path.GetExtension(LogoFile.FileName);

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await LogoFile.CopyToAsync(fileStream);
                }

                Company.Logo = $"/Uploads/Images/{uniqueFileName}";

                _context.Companies.Add(Company);
                await _context.SaveChangesAsync();

                return RedirectToPage("/Companies");
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception during OnPostAsync:");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);

                ModelState.AddModelError(string.Empty, "An error occurred while saving. Please try again.");
                return Page();
            }
        }
    }
}
