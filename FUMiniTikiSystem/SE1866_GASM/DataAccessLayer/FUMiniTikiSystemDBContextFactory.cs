using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System; // Thêm dòng này để sử dụng Console

namespace DataAccessLayer
{
    public class FUMiniTikiSystemDBContextFactory : IDesignTimeDbContextFactory<FUMiniTikiSystemDBContext>
    {
        public FUMiniTikiSystemDBContext CreateDbContext(string[] args)
        {
            // Lấy đường dẫn thư mục hiện tại khi lệnh migration chạy
            string basePath = Directory.GetCurrentDirectory();
            Console.WriteLine($"DEBUG: Current base path for appsettings.json: {basePath}"); // DEBUG

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Lấy chuỗi kết nối từ appsettings.json
            var connectionString = config.GetConnectionString("DefaultConnection");

            // Kiểm tra nếu chuỗi kết nối là null hoặc rỗng, sau đó sử dụng chuỗi dự phòng
            if (string.IsNullOrEmpty(connectionString))
            {
                // Đây là chuỗi dự phòng, nếu appsettings.json không tìm thấy hoặc key không đúng
                connectionString = "Server=(local);Database=FUMiniTikiSystem;Trusted_Connection=True;TrustServerCertificate=True;";
                Console.WriteLine($"DEBUG: Connection string from appsettings.json was not found or was empty. Using fallback connection string: {connectionString}"); // DEBUG
            }
            else
            {
                Console.WriteLine($"DEBUG: Using connection string from appsettings.json: {connectionString}"); // DEBUG
            }

            var optionsBuilder = new DbContextOptionsBuilder<FUMiniTikiSystemDBContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new FUMiniTikiSystemDBContext(optionsBuilder.Options);
        }
    }
}