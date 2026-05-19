using Bookify.Web.Core.Mapping;
using Bookify.Web.Helpers;
using Hangfire;
using HashidsNet;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Reflection;
using UoN.ExpressiveAnnotations.NetCore.DependencyInjection;
using ViewToHTML.Extensions;
using WhatsAppCloudApi.Extensions;

namespace Bookify.Web.Extensions
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddBookifyServices(this IServiceCollection services, WebApplicationBuilder builder)
        {

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.Zero);
            //services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;

                options.User.RequireUniqueEmail = true;
            });

            services.AddDataProtection().SetApplicationName(nameof(Bookify));

            services.AddSingleton<IHashids>(_ => new Hashids("find1ngn3m0", minHashLength: 11));
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IEmailBodyBuilder, EmailBodyBuilder>();

            services.AddControllersWithViews();

            services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));


            services.Configure<CloudinarySettings>(builder.Configuration.GetSection(nameof(CloudinarySettings)));
            services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));

            services.AddWhatsAppApiClient(builder.Configuration);

            services.AddExpressiveAnnotations();

            services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
            services.AddHangfireServer();
            services.Configure<AuthorizationOptions>(options => options.AddPolicy("AdminsOnly", policy =>
            {

                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRoles.Admin);

            }));
            services.AddViewToHTML();

            services.AddMvc(option =>
            option.Filters.Add(new AutoValidateAntiforgeryTokenAttribute())
            );

            return services;
        }
    }
}
