using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerCompass.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddForgotPasswordCodeToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ForgotPasswordCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForgotPasswordCode",
                table: "Users");
        }
    }
}
