using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodDonation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitsNeededToBloodRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BloodRequests_Users_PatientId",
                table: "BloodRequests");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "BloodRequests",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BloodRequests_PatientId",
                table: "BloodRequests",
                newName: "IX_BloodRequests_UserId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsKnown",
                table: "Hospitals",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Hospitals",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Hospitals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Hospitals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Hospitals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "HospitalId",
                table: "BloodRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnitsNeeded",
                table: "BloodRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Hospitals_UserId",
                table: "Hospitals",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BloodRequests_Users_UserId",
                table: "BloodRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Hospitals_Users_UserId",
                table: "Hospitals",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BloodRequests_Users_UserId",
                table: "BloodRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Hospitals_Users_UserId",
                table: "Hospitals");

            migrationBuilder.DropIndex(
                name: "IX_Hospitals_UserId",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "UnitsNeeded",
                table: "BloodRequests");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BloodRequests",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_BloodRequests_UserId",
                table: "BloodRequests",
                newName: "IX_BloodRequests_PatientId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsKnown",
                table: "Hospitals",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Hospitals",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<Guid>(
                name: "HospitalId",
                table: "BloodRequests",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_BloodRequests_Users_PatientId",
                table: "BloodRequests",
                column: "PatientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
