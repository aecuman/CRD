using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DistrictRateComps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DistrictWorkflows_DistrictRates_DistrictRateId",
                table: "DistrictWorkflows");

            migrationBuilder.AddColumn<string>(
                name: "ComparableDistrictRatesIds",
                table: "DistrictRates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DistrictRateId",
                table: "DistrictRates",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DistrictRates_DistrictRateId",
                table: "DistrictRates",
                column: "DistrictRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictRates_DistrictRates_DistrictRateId",
                table: "DistrictRates",
                column: "DistrictRateId",
                principalTable: "DistrictRates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictWorkflows_DistrictRates_DistrictRateId",
                table: "DistrictWorkflows",
                column: "DistrictRateId",
                principalTable: "DistrictRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DistrictRates_DistrictRates_DistrictRateId",
                table: "DistrictRates");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrictWorkflows_DistrictRates_DistrictRateId",
                table: "DistrictWorkflows");

            migrationBuilder.DropIndex(
                name: "IX_DistrictRates_DistrictRateId",
                table: "DistrictRates");

            migrationBuilder.DropColumn(
                name: "ComparableDistrictRatesIds",
                table: "DistrictRates");

            migrationBuilder.DropColumn(
                name: "DistrictRateId",
                table: "DistrictRates");

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictWorkflows_DistrictRates_DistrictRateId",
                table: "DistrictWorkflows",
                column: "DistrictRateId",
                principalTable: "DistrictRates",
                principalColumn: "Id");
        }
    }
}
