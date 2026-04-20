using DataAccess.Context;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(
    options => {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireDigit=true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequiredUniqueChars = 5;

        options.Lockout.MaxFailedAccessAttempts = 3;
    }
).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BlogDbContext>();


builder.Services.AddScoped<BlogRepository>();

builder.Services.AddControllersWithViews();

// Register Google external login without overriding Identity's default schemes
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });



builder.Services.AddRazorPages();

var log = new LoggerConfiguration().
    MinimumLevel.Override("Microsoft.AspNetCore.Authentication.Google", Serilog.Events.LogEventLevel.Error)
    .WriteTo
    .File("logs/log-.txt", rollingInterval: RollingInterval.Day).CreateLogger();

builder.Logging.AddSerilog(log);



var app = builder.Build();
 


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//ReExecute vs Redirect:
//ReExecute happens on the server side  (the url in the browser stays the same)
//Redirect happens on the client side

app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();



