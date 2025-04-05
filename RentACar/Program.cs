using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RentACar.Data;
using RentACar.Data.Models; // Make sure this contains your User model
using RentACar.Repositories;
using RentACar.Repositories.Interfaces;
using RentACar.Services;
using RentACar.Services.Helpers;
using RentACar.Services.Interfaces;

namespace RentACar;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultUI()
        .AddDefaultTokenProviders();

        // Add repositories with proper dependencies
        builder.Services.AddScoped<IPhotoService, PhotoService>();
        builder.Services.AddScoped<IAutoRepository, AutoRepository>();
        builder.Services.AddScoped<IRequestRepository, RequestRepository>();
        builder.Services.AddScoped<IBookingPeriodRepository, BookingPeriodRepository>();

        // Make sure UserRepository is properly registered
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        // Authentication must come before Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Simplified endpoint configuration
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapRazorPages();

        app.Run();
    }
}