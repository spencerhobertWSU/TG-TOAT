using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using TGTOAT.Data;
using TGTOAT.Models;
using TGTOAT.Helpers;
using Stripe;
using TGTOAT;


var builder = WebApplication.CreateBuilder(args);

// Update this line to use the UserContext connection string
builder.Services.AddDbContext<UserContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("UserContext")));

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthentication, Authentication>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register NotificationService
builder.Services.AddScoped<NotificationService>();

// Initialize Stripe
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();
