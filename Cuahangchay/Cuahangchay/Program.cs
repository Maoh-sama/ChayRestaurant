using Cuahangchay.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.CodeAnalysis.Elfie.Model.Structures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Thêm d?ch v? xác th?c Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login"; // ???ng d?n ??n trang ??ng nh?p
    });

// Thêm d?ch v? phân quy?n
builder.Services.AddAuthorization();

// C?u hình k?t n?i database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
});

app.UseRouting();

// ??t UseAuthentication và UseAuthorization ? ?ây
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();