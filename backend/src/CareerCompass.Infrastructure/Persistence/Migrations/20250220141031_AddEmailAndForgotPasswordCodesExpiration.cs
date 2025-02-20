using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerCompass.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailAndForgotPasswordCodesExpiration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EmailConfirmationCodeExpireAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ForgotPasswordCodeExpireAt",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmationCodeExpireAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ForgotPasswordCodeExpireAt",
                table: "Users");
        }
    }
}
