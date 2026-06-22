using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodDonation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationAgentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "Notifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Notifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Status",
                table: "Notifications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_CreatedAt",
                table: "Notifications",
                columns: new[] { "UserId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notifications_Status",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId_CreatedAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Notifications");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");
        }
    }
}
