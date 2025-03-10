using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Repositories;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace WebApplication1
{


    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "E-Commerce API",
                    Version = "v1",
                    Description = "An ASP.NET Core API for managing products and orders."
                });
            });

            builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(
                    "Server=DESKTOP-40BVR0R,1433;Database=ecomm;Integrated Security=True;TrustServerCertificate=True;",
                    options => options.EnableRetryOnFailure());
            });

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role; 
            });

            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();

            builder.Services.AddScoped<UserManager<AppUser>>();
            builder.Services.AddScoped<SignInManager<AppUser>>();

            builder.Services.AddScoped<IDataRepository<Product>, ProductRepository>();
            builder.Services.AddScoped<IDataRepository<Order>, OrderRepository>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce API v1");
                    c.RoutePrefix = "swagger"; 
                });
            }

            app.UseStaticFiles();
            app.UseRouting();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();

                await SeedRolesAndUsers(roleManager, userManager);
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapDefaultControllerRoute();

            app.MapGet("/", () => "Hello World!\n:)\n:) I AM NEW");

            app.Run();
        }

        public static async Task SeedRolesAndUsers(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            string adminEmail = "admin@example.com";
            string adminPassword = "Admin@123"; // Change this for production

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new AppUser { UserName = "AdminUser", Email = adminEmail };
                var result = await userManager.CreateAsync(newAdmin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }

    }
}
