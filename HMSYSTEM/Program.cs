using HMSYSTEM.Data;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddDbContext<Db>(options =>
    options.UseSqlServer(Db.ConnectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();

builder.Services.AddSession();


builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
    });

builder.Services.AddAuthorization();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


var culture = new CultureInfo("en-GB"); 
CultureInfo.DefaultThreadCurrentCulture = culture; 
CultureInfo.DefaultThreadCurrentUICulture = culture;

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();         
app.UseSession();       

app.UseAuthentication();    
app.UseAuthorization();     


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();  
app.Run();
