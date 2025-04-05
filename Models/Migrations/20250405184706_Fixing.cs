using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentACar.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fixing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingPeriod_Autos_AutoId",
                table: "BookingPeriod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingPeriod",
                table: "BookingPeriod");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "BookingPeriod",
                newName: "BookingPeriods");

            migrationBuilder.RenameIndex(
                name: "IX_BookingPeriod_AutoId",
                table: "BookingPeriods",
                newName: "IX_BookingPeriods_AutoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingPeriods",
                table: "BookingPeriods",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingPeriods_Autos_AutoId",
                table: "BookingPeriods",
                column: "AutoId",
                principalTable: "Autos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingPeriods_Autos_AutoId",
                table: "BookingPeriods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingPeriods",
                table: "BookingPeriods");

            migrationBuilder.RenameTable(
                name: "BookingPeriods",
                newName: "BookingPeriod");

            migrationBuilder.RenameIndex(
                name: "IX_BookingPeriods_AutoId",
                table: "BookingPeriod",
                newName: "IX_BookingPeriod_AutoId");

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingPeriod",
                table: "BookingPeriod",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingPeriod_Autos_AutoId",
                table: "BookingPeriod",
                column: "AutoId",
                principalTable: "Autos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
