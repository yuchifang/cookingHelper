using System.Text.Json;
using CookingHelper.Data;
using CookingHelper.DatabaseService;
using CookingHelper.LineDto;
using CookingHelper.LineDtoService;
using CookingHelper.Middleware;
using CookingHelper.Model;
using CookingHelper.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace CookingHelper.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders().AddConsole();
            builder.Services.AddMemoryCache();
            builder.Services.AddHttpClient();
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            if (builder.Environment.IsDevelopment())
            {
                var ConnectString = builder.Configuration.GetConnectionString("MySQLConnectString");
                builder.Services.AddDbContext<UserListDbContext>(Options =>
                {
                    Options.UseMySql(ConnectString, ServerVersion.AutoDetect(ConnectString));
                });
            }
            else
            {
                var ConnectString = builder.Configuration.GetConnectionString(
                    "AZURE_SQL_CONNECTIONSTRING"
                );
                builder.Services.AddDbContext<UserListDbContext>(Options =>
                {
                    Options.UseSqlServer(
                        ConnectString,
                        providerOptions => providerOptions.EnableRetryOnFailure()
                    );
                });
            }

            builder.Services.AddHostedService<ApiLogBackgroundService>();
            builder.Services.AddSingleton<ApiLogService, ApiLogService>();
            builder.Services.AddScoped<LineBotService, LineBotService>();
            builder.Services.AddScoped<RichMenuService, RichMenuService>();
            builder.Services.AddScoped<ShoppingListDatabaseService, ShoppingListDatabaseService>();
            builder.Services.AddScoped<ShoppingListService, ShoppingListService>();
            builder.Services.AddScoped<StorageManagementService, StorageManagementService>();
            builder.Services.AddScoped<
                StorageManagementAdditionService,
                StorageManagementAdditionService
            >();
            builder.Services.AddScoped<
                StorageManagementDatabaseService,
                StorageManagementDatabaseService
            >();
            builder.Services.AddScoped<
                StorageManagementSearchService,
                StorageManagementSearchService
            >();
            builder.Services.AddScoped<RecipeListService, RecipeListService>();
            builder.Services.AddScoped<RecipeListDatabaseService, RecipeListDatabaseService>();
            builder.Services.AddScoped<RecipeListAdditionService, RecipeListAdditionService>();
            builder.Services.AddScoped<RecipeListSearchService, RecipeListSearchService>();
            builder.Services.AddScoped<GenerateDataService, GenerateDataService>();

            // Add Identity
            builder
                .Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<UserListDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            // Configure Identity options
            builder.Services.Configure<IdentityOptions>(IdentityConfigureOptions);

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                // 設定密碼重設令牌的有效期限
                options.TokenLifespan = TimeSpan.FromSeconds(180); // 預設為 1 天
            });

            // Configure Cookie settings
            builder.Services.ConfigureApplicationCookie(CookieAuthenticationOptions);

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Lax; // 全局 SameSite 策略
            });

            var app = builder.Build();

            app.UseWhen(
                context => context.Request.Path.StartsWithSegments("/api/LineBot/Webhook"),
                appBuilder =>
                {
                    appBuilder.UseMiddleware<ApiLoggingMiddleware>();
                }
            );

            app.UseDefaultFiles();
            app.UseStaticFiles(
                new StaticFileOptions
                {
                    OnPrepareResponse = context =>
                    {
                        var headers = context.Context.Response.Headers;

                        if (context.File.Name.EndsWith(".js") || context.File.Name.EndsWith(".css"))
                        {
                            headers["Cache-Control"] = "public,max-age=31536000";
                        }
                    }
                }
            );

            app.UseStaticFiles(
                new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(
                            builder.Environment.ContentRootPath,
                            "UploadFile",
                            "RecipeImage"
                        )
                    ),
                    RequestPath = "/UploadFile/RecipeImage",
                    OnPrepareResponse = context =>
                    {
                        var headers = context.Context.Response.Headers;
                        headers["Cache-Control"] = "public,max-age=31536000";
                    }
                }
            );

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }

        public static void IdentityConfigureOptions(IdentityOptions options)
        {
            // Password settings
            options.Password.RequireDigit = false;

            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;

            // Lockout settings
            // options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            // options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
        }

        public static void CookieAuthenticationOptions(CookieAuthenticationOptions options)
        {
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(14); // Example: 2 weeks
            options.SlidingExpiration = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        }
    }
}
