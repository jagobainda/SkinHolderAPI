using Microsoft.EntityFrameworkCore;
using SkinHolderAPI.DataService.Contexts;

namespace SkinHolderAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<SkinHolderDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 42))));

            builder.Services.AddDbContext<SkinHolderLogDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("LogConnection"),
                    new MySqlServerVersion(new Version(8, 0, 42))));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
