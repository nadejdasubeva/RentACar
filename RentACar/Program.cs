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
            // Build the host first
            var host = CreateHostBuilder(args).Build();

            // Initialize admin user (sync-over-async pattern)
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Use GetAwaiter().GetResult() to bridge sync/async
                    InitializeAdmin.InitializeAdminAsync(services).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while initializing admin user");
                }
            }

            // Run the host
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((context, services) =>
                    {
                        // Add services to the container.
                        var connectionString = context.Configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

                        services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseSqlServer(connectionString));

                        services.AddDatabaseDeveloperPageExceptionFilter();

                        services.AddIdentity<User, IdentityRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddDefaultUI()
                            .AddDefaultTokenProviders();

                        // Add repositories and services
                        services.AddScoped<IPhotoService, PhotoService>();
                        services.AddScoped<IAutoRepository, AutoRepository>();
                        services.AddScoped<IRequestRepository, RequestRepository>();
                        services.AddScoped<IBookingPeriodRepository, BookingPeriodRepository>();
                        services.AddScoped<IUserRepository, UserRepository>();
                        services.AddScoped<IUserService, UserService>();

                        services.Configure<CloudinarySettings>(context.Configuration.GetSection("CloudinarySettings"));
                        services.AddControllersWithViews();
                        services.AddRazorPages();
                    })
                    .Configure(app =>
                    {
                        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

                        if (env.IsDevelopment())
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
                        app.UseAuthentication();
                        app.UseAuthorization();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllerRoute(
                                name: "default",
                                pattern: "{controller=Home}/{action=Index}/{id?}");
                            endpoints.MapRazorPages();
                        });
                    });
                });
}