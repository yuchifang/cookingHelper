using CookingHelper.Data;
using CookingHelper.DatabaseService;
using CookingHelper.LineDtoService;
using Microsoft.EntityFrameworkCore;

namespace CookingHelper.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddMemoryCache();
            builder.Services.AddHttpClient();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
            builder.Services.AddScoped<RecipeListAddition, RecipeListAddition>();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

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
