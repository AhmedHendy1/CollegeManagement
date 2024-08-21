using CollegeManagement.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Redirect to login page if not authenticated
        options.LogoutPath = "/Account/Logout"; // Redirect to logout path
        options.Cookie.Name = "CollegeManagement"; // Set the name of the authentication cookie
        options.Cookie.HttpOnly = true; // Make cookie inaccessible to JavaScript
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Use HTTPS for secure cookies
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // Set expiration time for cookies
        options.SlidingExpiration = true; // Renew the authentication cookie on each request
        options.Events.OnRedirectToLogin = context =>
        {
            // Handle redirection
            context.Response.Redirect("/Account/Login");
            return Task.CompletedTask;
        };
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("Employee"));
    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
});

// Add DbContext
builder.Services.AddDbContext<StudentManagementSystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Ensure authentication is enabled
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
