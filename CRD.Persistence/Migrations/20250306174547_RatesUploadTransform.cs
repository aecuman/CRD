using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RatesUploadTransform : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DistrictRates_Districts_DistrictId1",
                table: "DistrictRates");

            migrationBuilder.DropIndex(
                name: "IX_DistrictRates_DistrictId1",
                table: "DistrictRates");

            migrationBuilder.DropColumn(
                name: "DistrictId1",
                table: "DistrictRates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DistrictId1",
                table: "DistrictRates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DistrictRates_DistrictId1",
                table: "DistrictRates",
                column: "DistrictId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictRates_Districts_DistrictId1",
                table: "DistrictRates",
                column: "DistrictId1",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
