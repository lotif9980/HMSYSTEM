using HMSYSTEM.Repository;
using Microsoft.EntityFrameworkCore;
using HMSYSTEM.Data;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter()); // সবার জন্য authorize বাধ্যতামূলক
});

builder.Services.AddDbContext<Db>(options =>
    options.UseSqlServer(Db.ConnectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();

builder.Services.AddSession();

// 👉 Authentication & Authorization
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login"; // Login page path
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// 👉 Correct middleware order starts here
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();           // ✅ Routing must come first
app.UseSession();           // ✅ Session before auth

app.UseAuthentication();    // ✅ Authentication before Authorization
app.UseAuthorization();     // ✅ Only once, and after authentication

// 👉 Controller Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();     // Keep if you are using static asset mapping

app.Run();
