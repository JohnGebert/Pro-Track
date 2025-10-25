using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using ProTrack.Data;
using ProTrack.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Get the database connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configure Entity Framework with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure ASP.NET Core Identity with custom password complexity rules
builder.Services.AddDefaultIdentity<ApplicationUser>(options => 
{
    // Sign-in settings
    options.SignIn.RequireConfirmedAccount = false;
    
    // Password complexity requirements
    // These rules enforce strong passwords to protect user accounts
    options.Password.RequireDigit = true;                    // Must contain at least one digit (0-9)
    options.Password.RequireLowercase = true;                // Must contain at least one lowercase letter (a-z)
    options.Password.RequireNonAlphanumeric = false;         // Special characters (!@#$) are optional
    options.Password.RequireUppercase = true;                // Must contain at least one uppercase letter (A-Z)
    options.Password.RequiredLength = 6;                     // Minimum length of 6 characters
    options.Password.RequiredUniqueChars = 1;                // Must have at least 1 unique character
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Add MVC controllers and views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
