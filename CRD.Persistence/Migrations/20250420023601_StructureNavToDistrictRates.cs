using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StructureNavToDistrictRates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DistrictRateId1",
                table: "StructureRates",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StructureRates_DistrictRateId1",
                table: "StructureRates",
                column: "DistrictRateId1");

            migrationBuilder.AddForeignKey(
                name: "FK_StructureRates_DistrictRates_DistrictRateId1",
                table: "StructureRates",
                column: "DistrictRateId1",
                principalTable: "DistrictRates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StructureRates_DistrictRates_DistrictRateId1",
                table: "StructureRates");

            migrationBuilder.DropIndex(
                name: "IX_StructureRates_DistrictRateId1",
                table: "StructureRates");

            migrationBuilder.DropColumn(
                name: "DistrictRateId1",
                table: "StructureRates");
        }
    }
}
