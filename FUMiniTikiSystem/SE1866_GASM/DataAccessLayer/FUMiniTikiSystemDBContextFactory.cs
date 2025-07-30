using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;

namespace DataAccessLayer
{
    public class FUMiniTikiSystemDBContextFactory : IDesignTimeDbContextFactory<FUMiniTikiSystemDBContext>
    {
        public FUMiniTikiSystemDBContext CreateDbContext(string[] args)
        {
            string basePath = Directory.GetCurrentDirectory();
            Console.WriteLine($"DEBUG: Current base path for appsettings.json: {basePath}");

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Lấy chuỗi kết nối TỪ TÊN ĐÚNG TRONG appsettings.json: "FUMiniTikiDB"
            var connectionString = config.GetConnectionString("FUMiniTikiDB");

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "Server=(local);Database=FUMiniTikiSystem;Trusted_Connection=True;TrustServerCertificate=True;";
                Console.WriteLine($"DEBUG: Connection string from appsettings.json was not found or was empty. Using fallback connection string: {connectionString}");
            }
            else
            {
                Console.WriteLine($"DEBUG: Using connection string from appsettings.json: {connectionString}");
            }

            var optionsBuilder = new DbContextOptionsBuilder<FUMiniTikiSystemDBContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new FUMiniTikiSystemDBContext(optionsBuilder.Options);
        }
    }
}