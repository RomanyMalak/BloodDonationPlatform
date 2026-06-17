using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodDonation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Last : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.Sql(@"
                            IF COL_LENGTH('Hospitals', 'RejectionReason') IS NOT NULL
                            BEGIN
                                ALTER TABLE [Hospitals] DROP COLUMN [RejectionReason];
                            END
                            ");

          migrationBuilder.Sql(@"
                            IF COL_LENGTH('Hospitals', 'ReviewedAt') IS NOT NULL
                            BEGIN
                                ALTER TABLE [Hospitals] DROP COLUMN [ReviewedAt];
                            END
                            ");

           migrationBuilder.Sql(@"
                            IF COL_LENGTH('Hospitals', 'Status') IS NOT NULL
                            BEGIN
                                ALTER TABLE [Hospitals] DROP COLUMN [Status];
                            END
                            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Hospitals",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "Hospitals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Hospitals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
