using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodDonation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditBllodRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BloodRequests_Users_UserId",
                table: "BloodRequests");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BloodRequests",
                newName: "CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_BloodRequests_UserId",
                table: "BloodRequests",
                newName: "IX_BloodRequests_CreatedByUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Users",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicenseDocumentPath",
                table: "Hospitals",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Hotline",
                table: "Hospitals",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "HospitalId",
                table: "BloodRequests",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "BloodRequests",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "BloodRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedByHospitalId",
                table: "BloodRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientAge",
                table: "BloodRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientName",
                table: "BloodRequests",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "BloodRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OcrVerifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BloodRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    ConfidenceScore = table.Column<double>(type: "float", nullable: false),
                    RawExtractedText = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcrVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OcrVerifications_BloodRequests_BloodRequestId",
                        column: x => x.BloodRequestId,
                        principalTable: "BloodRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hospitals_Name_City",
                table: "Hospitals",
                columns: new[] { "Name", "City" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OcrVerifications_BloodRequestId",
                table: "OcrVerifications",
                column: "BloodRequestId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BloodRequests_Users_CreatedByUserId",
                table: "BloodRequests",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BloodRequests_Users_CreatedByUserId",
                table: "BloodRequests");

            migrationBuilder.DropTable(
                name: "OcrVerifications");

            migrationBuilder.DropIndex(
                name: "IX_Hospitals_Name_City",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "BloodRequests");

            migrationBuilder.DropColumn(
                name: "ApprovedByHospitalId",
                table: "BloodRequests");

            migrationBuilder.DropColumn(
                name: "PatientAge",
                table: "BloodRequests");

            migrationBuilder.DropColumn(
                name: "PatientName",
                table: "BloodRequests");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "BloodRequests");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "BloodRequests",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BloodRequests_CreatedByUserId",
                table: "BloodRequests",
                newName: "IX_BloodRequests_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicenseDocumentPath",
                table: "Hospitals",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Hotline",
                table: "Hospitals",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "HospitalId",
                table: "BloodRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "BloodRequests",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddForeignKey(
                name: "FK_BloodRequests_Users_UserId",
                table: "BloodRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
