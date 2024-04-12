using Brix.Models;
using Brix.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML.OnnxRuntime;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Access to ConfigureServices
        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        // Middleware configurations
        ConfigureMiddleware(app);

        // Seed data or perform other startup tasks
        await ConfigureStartupTasks(app);

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication().AddGoogle(googleOptions =>
        {
            googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
            googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
        });

        //Add services to the container.
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        services.AddDbContext<BrixIdentityDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddDbContext<BrixDatabaseContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<BrixIdentityDbContext>();
        services.AddControllersWithViews();

        services.AddScoped<ILegostoreRepository, EFLegostoreRepository>();

        services.AddRazorPages();
        services.AddDistributedMemoryCache();
        services.AddSession();
        services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddSingleton<InferenceSession>(
            new InferenceSession(".\\decision_tree_model-3.onnx")
        );

        // Configure Identity options
        services.Configure<IdentityOptions>(options =>
        {
            // Password settings and others...
        });
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
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
        app.UseSession();

        app.UseCookiePolicy();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();
    }

    private static async Task ConfigureStartupTasks(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        using (var scope = app.Services.CreateScope())
        {
            var userManager =
                scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string email = "aurorabrickwell@gmail.com";
            string password = "Masterbuildersunite!123";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser();
                user.UserName = email;
                user.Email = email;
                user.EmailConfirmed = true;

                await userManager.CreateAsync(user, password);

                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
    
}

//using Brix.Models;
//using Brix.Services;

////using Brix.Services;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.ML.OnnxRuntime;

//var builder = WebApplication.CreateBuilder(args);
//var services = builder.Services;
//var configuration = builder.Configuration;

//services.AddAuthentication().AddGoogle(googleOptions =>
//{
//    googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
//    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
//});

////Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//builder.Services.AddDbContext<BrixIdentityDbContext>(options =>
//    options.UseSqlServer(connectionString));
//builder.Services.AddDbContext<BrixDatabaseContext>(options =>
//    options.UseSqlServer(connectionString));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<BrixIdentityDbContext>();
//builder.Services.AddControllersWithViews();

//builder.Services.AddScoped<ILegoStoreRepository, EFLegostoreRepository>();

//builder.Services.AddRazorPages();
//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession();
//builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//builder.Services.Configure<CookiePolicyOptions>(options =>
//{
//    // This lambda determines whether user consent for non-essential 
//    // cookies is needed for a given request.
//    options.CheckConsentNeeded = context => true;

//    options.MinimumSameSitePolicy = SameSiteMode.None;
//    //options.ConsentCookieValue = "true";
//});

////services.AddSingleton<InferenceSession>(
////    new InferenceSession("C:\\Users\\autum\\Source\\Repos\\Brix\\Brix\\decision_tree_model-3.onnx")
////);

//services.AddSingleton<InferenceSession>(provider =>);
//public class Program
//{
//    public static async Task Main(string[] args)
//    {
//        var builder = WebApplication.CreateBuilder(args);
//        var services = builder.Services;
//        var configuration = builder.Configuration;

//        services.AddAuthentication().AddGoogle(googleOptions =>
//        {
//            googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
//            googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
//        });

//        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//        services.AddDbContext<BrixIdentityDbContext>(options =>
//            options.UseSqlServer(connectionString));
//        services.AddDbContext<BrixDatabaseContext>(options =>
//            options.UseSqlServer(connectionString));
//        services.AddDatabaseDeveloperPageExceptionFilter();

//        services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//            .AddRoles<IdentityRole>()
//            .AddEntityFrameworkStores<BrixIdentityDbContext>();

//        services.AddControllersWithViews();

//        services.AddScoped<ILegostoreRepository, EFLegostoreRepository>();

//        services.Configure<CookiePolicyOptions>(options =>
//        {
//            options.CheckConsentNeeded = context => true;
//            options.MinimumSameSitePolicy = SameSiteMode.None;
//        });

//        services.AddSingleton<InferenceSession>(
//            new InferenceSession(".\\decision_tree_model-3.onnx")
//        );

//        services.AddSingleton<InferenceSession>(provider =>
//        {
//            // Provide the path to the ONNX model file
//            string modelPath = ".\\decision_tree_model-3.onnx";
//            return new InferenceSession(modelPath);
//        });

//        //builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();

//        //password requirements
//        builder.Services.Configure<IdentityOptions>(options =>
//        {
//            // Password settings.
//            options.Password.RequireDigit = true;
//            options.Password.RequireLowercase = true;
//            options.Password.RequireNonAlphanumeric = true;
//            options.Password.RequireUppercase = true;
//            options.Password.RequiredLength = 8;
//            options.Password.RequiredUniqueChars = 3;

//            // Lockout settings.
//            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
//            options.Lockout.MaxFailedAccessAttempts = 5;
//            options.Lockout.AllowedForNewUsers = true;

//            // User settings.
//            options.User.AllowedUserNameCharacters =
//            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
//            options.User.RequireUniqueEmail = false;
//        });

//        var app = builder.Build();

//        if (app.Environment.IsDevelopment())
//        {
//            app.UseMigrationsEndPoint();
//        }
//        else
//        {
//            app.UseExceptionHandler("/Home/Error");
//            app.UseHsts();
//        }

//        app.UseHttpsRedirection();
//        app.UseStaticFiles();
//        app.UseCookiePolicy();
//        app.UseRouting();
//        app.UseAuthentication();
//        app.UseAuthorization();

//        app.MapControllerRoute(
//            name: "default",
//            pattern: "{controller=Home}/{action=Index}/{id?}");
//        app.MapRazorPages();

//        using (var scope = app.Services.CreateScope())
//        {
//            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//            var roles = new[] { "Admin", "User" };

//            foreach (var role in roles)
//            {
//                if (!await roleManager.RoleExistsAsync(role))
//                    await roleManager.CreateAsync(new IdentityRole(role));
//            }
//        }

//        using (var scope = app.Services.CreateScope())
//        {
//            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
//            string email = "aurora@brickwell.com";
//            string password = "Aurora@1234,";

//            if (await userManager.FindByEmailAsync(email) == null)
//            {
//                var user = new IdentityUser();
//                user.UserName = email;
//                user.Email = email;

//                await userManager.CreateAsync(user, password);
//                await userManager.AddToRoleAsync(user, "admin");
//            }
//        }

//        app.Run();
//    }
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseSession();

//app.UseCookiePolicy();

//app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapRazorPages();

//app.Run();
