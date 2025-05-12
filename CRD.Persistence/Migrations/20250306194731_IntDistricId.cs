using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IntDistricId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CRDFiles_DistrictRates_DistrictRateId",
                table: "CRDFiles");

            migrationBuilder.DropIndex(
                name: "IX_CRDFiles_DistrictRateId",
                table: "CRDFiles");

            migrationBuilder.DropColumn(
                name: "DistrictRateId",
                table: "CRDFiles");

            migrationBuilder.AlterColumn<int>(
                name: "DistrictId",
                table: "DistrictRates",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_DistrictRates_DistrictId",
                table: "DistrictRates",
                column: "DistrictId");

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFiles_DistrictRates_RefId",
                table: "CRDFiles",
                column: "RefId",
                principalTable: "DistrictRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictRates_Districts_DistrictId",
                table: "DistrictRates",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CRDFiles_DistrictRates_RefId",
                table: "CRDFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrictRates_Districts_DistrictId",
                table: "DistrictRates");

            migrationBuilder.DropIndex(
                name: "IX_DistrictRates_DistrictId",
                table: "DistrictRates");

            migrationBuilder.AlterColumn<string>(
                name: "DistrictId",
                table: "DistrictRates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DistrictRateId",
                table: "CRDFiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CRDFiles_DistrictRateId",
                table: "CRDFiles",
                column: "DistrictRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFiles_DistrictRates_DistrictRateId",
                table: "CRDFiles",
                column: "DistrictRateId",
                principalTable: "DistrictRates",
                principalColumn: "Id");
        }
    }
}
