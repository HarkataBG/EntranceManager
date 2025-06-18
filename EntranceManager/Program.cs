using AspNetCoreDemo.Helpers;
using EntranceManager.Data;
using EntranceManager.Repositories;
using EntranceManager.Repositories.Contracts;
using EntranceManager.Services;
using EntranceManager.Services.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

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

            var key = builder.Configuration.GetSection("JwtSettings")["Key"];

            if (key == null)
                key = "123456";

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddCookie(options =>
             {
                 options.LoginPath = "/AuthMvc/Login";
                 options.LogoutPath = "/AuthMvc/Logout";
                 options.Events.OnRedirectToLogin = context =>
                 {
                     if (context.Request.Path.StartsWithSegments("/api") &&
                         context.Response.StatusCode == 200)
                     {
                         context.Response.StatusCode = 401;
                         return Task.CompletedTask;
                     }

                     context.Response.Redirect(context.RedirectUri);
                     return Task.CompletedTask;
                 };
             })
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = "EntranceManager",
                     ValidAudience = "EntranceUsers",
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                 };
             });

            builder.Services.AddAuthorization();

            //Repositories
            builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
            builder.Services.AddScoped<IEntranceRepository, EntranceRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IFeeRepository, FeeRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

            //Services
            builder.Services.AddScoped<IApartmentService, ApartmentService>();
            builder.Services.AddScoped<IEntranceService, EntranceService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUsersService, UsersService>();
            builder.Services.AddScoped<IFeesService, FeesService>();
            builder.Services.AddScoped<IPaymentsService, PaymentsService>();

            //Helpers
            builder.Services.AddScoped<ModelMapper>();

            builder.Services.AddHttpClient();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            var supportedCultures = new[] { new CultureInfo("en-US") };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                var token = context.Request.Cookies["auth_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers.Append("Authorization", "Bearer " + token);
                }

                await next();
            });

            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 401 &&
                    !context.Request.Path.StartsWithSegments("/api") &&
                    !context.Response.HasStarted)
                {
                    context.Response.Redirect("/AuthMvc/Login");
                }
            });

            app.UseAuthentication();

            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


                app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
