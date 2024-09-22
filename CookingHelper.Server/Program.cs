using CookingHelper.Data;
using CookingHelper.DatabaseService;
using CookingHelper.LineDtoService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace CookingHelper.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // builder.Services.AddApplicationInsightsTelemetry();
            builder.Logging.ClearProviders().AddConsole();
            builder.Services.AddMemoryCache();
            builder.Services.AddHttpClient();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            if (builder.Environment.IsDevelopment())
            {
                // var ConnectString = builder.Configuration.GetConnectionString("MySQLConnectString");
                // builder.Services.AddDbContext<UserListDbContext>(Options =>
                // {
                //     Options.UseMySql(ConnectString, ServerVersion.AutoDetect(ConnectString));
                // });
                builder
                    .Configuration.AddEnvironmentVariables()
                    .AddJsonFile("appsettings.Production.json");
                var ConnectString = builder.Configuration.GetConnectionString(
                    "AZURE_SQL_CONNECTIONSTRING"
                );
                builder.Services.AddDbContext<UserListDbContext>(Options =>
                {
                    Options.UseSqlServer(ConnectString);
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

            var app = builder.Build();

            app.UseDefaultFiles();
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
                    RequestPath = "/UploadFile/RecipeImage"
                }
            );

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
