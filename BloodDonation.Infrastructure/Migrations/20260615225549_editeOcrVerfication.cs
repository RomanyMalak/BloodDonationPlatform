using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodDonation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editeOcrVerfication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExtractedBloodType",
                table: "OcrVerifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtractedUnits",
                table: "OcrVerifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtractedUrgency",
                table: "OcrVerifications",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtractedBloodType",
                table: "OcrVerifications");

            migrationBuilder.DropColumn(
                name: "ExtractedUnits",
                table: "OcrVerifications");

            migrationBuilder.DropColumn(
                name: "ExtractedUrgency",
                table: "OcrVerifications");
        }
    }
}
