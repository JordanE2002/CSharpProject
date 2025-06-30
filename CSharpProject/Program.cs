using Microsoft.EntityFrameworkCore;
using CSharpProject.Data;
using CSharpProject.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();

    // Seed Alpha company and employees if not exists
    if (!context.Companies.Any(c => c.Name == "Alpha"))
    {
        var alpha = new Company
        {
            Name = "Alpha",
            Email = "contact@alpha.com",
            Website = "https://alpha.com",
            Logo = "/uploads/images/alpha.png"
        };
        context.Companies.Add(alpha);
        context.SaveChanges();

        context.Employees.AddRange(new[]
        {
            new Employee { FirstName = "Alice", LastName = "Johnson", Email = "alice@alpha.com", PhoneNumber = "555-0101", CompanyId = alpha.Id },
            new Employee { FirstName = "Bob", LastName = "Smith", Email = "bob@alpha.com", PhoneNumber = "555-0102", CompanyId = alpha.Id },
            new Employee { FirstName = "Charlie", LastName = "Williams", Email = "charlie@alpha.com", PhoneNumber = "555-0103", CompanyId = alpha.Id }
        });
        context.SaveChanges();
    }

    // Seed Beta company and employees if not exists
    if (!context.Companies.Any(c => c.Name == "Beta"))
    {
        var beta = new Company
        {
            Name = "Beta",
            Email = "contact@beta.com",
            Website = "https://beta.com",
            Logo = "/uploads/images/beta.png"
        };
        context.Companies.Add(beta);
        context.SaveChanges();

        context.Employees.AddRange(new[]
        {
            new Employee { FirstName = "David", LastName = "Brown", Email = "david@beta.com", PhoneNumber = "555-0201", CompanyId = beta.Id },
            new Employee { FirstName = "Eva", LastName = "Jones", Email = "eva@beta.com", PhoneNumber = "555-0202", CompanyId = beta.Id },
            new Employee { FirstName = "Fiona", LastName = "Miller", Email = "fiona@beta.com", PhoneNumber = "555-0203", CompanyId = beta.Id }
        });
        context.SaveChanges();
    }

    // Seed Charlie company and employees if not exists
    if (!context.Companies.Any(c => c.Name == "Charlie"))
    {
        var charlie = new Company
        {
            Name = "Charlie",
            Email = "contact@charlie.com",
            Website = "https://charlie.com",
            Logo = "/uploads/images/charlie.png"
        };
        context.Companies.Add(charlie);
        context.SaveChanges();

        context.Employees.AddRange(new[]
        {
            new Employee { FirstName = "George", LastName = "Davis", Email = "george@charlie.com", PhoneNumber = "555-0301", CompanyId = charlie.Id },
            new Employee { FirstName = "Hannah", LastName = "Wilson", Email = "hannah@charlie.com", PhoneNumber = "555-0302", CompanyId = charlie.Id },
            new Employee { FirstName = "Ian", LastName = "Moore", Email = "ian@charlie.com", PhoneNumber = "555-0303", CompanyId = charlie.Id }
        });
        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapGet("/", context =>
{
    context.Response.Redirect("/Home");
    return Task.CompletedTask;
});

app.MapRazorPages();
app.Run();
