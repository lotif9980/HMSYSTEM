using HMSYSTEM.Data;
using HMSYSTEM.Repository;
using Microsoft.EntityFrameworkCore;
using HMSYSTEM.Data;
using HMSYSTEM.Repository;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<Db>(options => options.UseSqlServer(Db.ConnectionString));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();



// 🔹 Add distributed memory cache (required for session)
builder.Services.AddDistributedMemoryCache();

// 🔹 Add session service
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // session expire after 30 minutes
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";


        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);


        options.Cookie.IsEssential = true;
        options.Cookie.HttpOnly = true;
        options.SlidingExpiration = true;
        options.Cookie.MaxAge = null;
    });



builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


// ------------------ Culture Config ------------------
var cultureInfo = new CultureInfo("en-GB"); // dd/MM/yyyy
cultureInfo.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
cultureInfo.DateTimeFormat.LongTimePattern = "HH:mm:ss";

// Apply globally
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
// ----------------------------------------------------

app.UseHttpsRedirection();
app.UseStaticFiles();



app.UseRouting();

// 🔹 Use session middleware before authorization
app.UseSession();



app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
