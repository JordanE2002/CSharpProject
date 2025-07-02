using Microsoft.EntityFrameworkCore;
using CSharpProject.Data;
using CSharpProject.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))); // SQLite

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated(); //  Create DB & tables

    // First company
    if (!context.Companies.Any(c => c.Name == "Test Company"))
    {
        var company1 = new Company
        {
            Name = "Test Company",
            Email = "test@example.com",
            Website = "https://test.com",
            Logo = "logo1.png"
        };
        context.Companies.Add(company1);
        context.SaveChanges();

        var employee1 = new Employee
        {
            FirstName = "Alice",
            LastName = "Johnson",
            Email = "alice@test.com",
            PhoneNumber = "123-456-7890",
            CompanyId = company1.Id
        };
        context.Employees.Add(employee1);
        context.SaveChanges();
    }

    // 2nd company for testing
    if (!context.Companies.Any(c => c.Name == "Second Test Company"))
    {
        var company2 = new Company
        {
            Name = "Second Test Company",
            Email = "second@test.com",
            Website = "https://secondtest.com",
            Logo = "logo2.png"
        };
        context.Companies.Add(company2);
        context.SaveChanges();

        var employee2 = new Employee
        {
            FirstName = "Bob",
            LastName = "Smith",
            Email = "bob@secondtest.com",
            PhoneNumber = "987-654-3210",
            CompanyId = company2.Id
        };
        context.Employees.Add(employee2);
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
