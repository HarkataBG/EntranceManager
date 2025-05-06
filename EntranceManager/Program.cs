using AspNetCoreDemo.Helpers;
using EntranceManager.Data;
using EntranceManager.Repositories;
using EntranceManager.Services;
using Microsoft.EntityFrameworkCore;

namespace EntranceManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();

            builder.Services.AddControllersWithViews();

            //Repositories
            builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
            builder.Services.AddScoped<IEntranceRepository, EntranceRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            //Services
            builder.Services.AddScoped<IApartmentService, ApartmentService>();
            builder.Services.AddScoped<IEntranceService, EntranceService>();

            //Helpers
            builder.Services.AddScoped<ModelMapper>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
