
using Bookify.Infrastructure;
using Bookify.Web.Seeds;
using Bookify.Web.Tasks;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Serilog;
using Serilog.Context;

namespace Bookify.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddWebServices(builder);
            //Add SeriLog
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
            builder.Host.UseSerilog();

            var app = builder.Build();
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "Deny");

                await next();
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            

            //app.UseStatusCodePages(async statusCodeContext =>
            //{
            //	// using static System.Net.Mime.MediaTypeNames;
            //	statusCodeContext.HttpContext.Response.ContentType = System.Net.Mime.MediaTypeNames.Text.Plain;

            //	await statusCodeContext.HttpContext.Response.WriteAsync(
            //		$"Status Code Page: {statusCodeContext.HttpContext.Response.StatusCode}");
            //});

            //app.UseStatusCodePagesWithRedirects("/Home/Error/{0}");

          

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //app.UseCookiePolicy(new CookiePolicyOptions
            //{

            //    Secure = CookieSecurePolicy.Always
            //});

           

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var roleManger = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManger = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await DefaultRoles.SeedAsync(roleManger);
            await DefaultUsers.SeedAdminUserAsync(userManger);

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {


                DashboardTitle = "Bookify Dashboard",
                //IsReadOnlyFunc = (DashboardContext context) => true,
                Authorization = new IDashboardAuthorizationFilter[]
                {
                    new HangfireAuthorizationFilter("AdminsOnly")
                }
            });
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var webHostEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            var whatsAppClient = scope.ServiceProvider.GetRequiredService<IWhatsAppClient>();
            var emailBodyBuilder = scope.ServiceProvider.GetRequiredService<IEmailBodyBuilder>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            var hangfireTasks = new HangfireTasks(dbContext, webHostEnvironment, whatsAppClient,
                emailBodyBuilder, emailSender);

            //RecurringJob.AddOrUpdate(() => hangfireTasks.PrepareExpirationAlert(), "0 14 * * *");
            RecurringJob.AddOrUpdate(
     "subscription-expiration-alert",
     () => hangfireTasks.PrepareExpirationAlert(),
     "0 14 * * *",
     new RecurringJobOptions
     {
         TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")
     });

            RecurringJob.AddOrUpdate(
     "rentals-expiration-alert",
     () => hangfireTasks.RentalsExpirationAlert(),
     "0 14 * * *",
     new RecurringJobOptions
     {
         TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")
     });

            app.Use(async (context, next) =>
            {
                LogContext.PushProperty("UserId", context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                LogContext.PushProperty("UserName", context.User.FindFirst(ClaimTypes.Name)?.Value);
                await next();
            });
            app.UseSerilogRequestLogging();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
