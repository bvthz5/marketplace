using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace MarketPlace.DataAccess.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AdminRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var name = "Admin";
            var email = "invmarketplace4u@gmail.com";
            var password = BCrypt.Net.BCrypt.HashPassword("Admin@123");
            var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            migrationBuilder.Sql($"INSERT INTO Admins (Name, Email, Password, CreatedDate, UpdatedDate) VALUES('{name}','{email}','{password}','{date}','{date}')");
        }
    }
}
