using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DistrictWorkflowAddRateId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DistrictRateId",
                table: "DistrictWorkflows",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DistrictWorkflows_DistrictRateId",
                table: "DistrictWorkflows",
                column: "DistrictRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictWorkflows_DistrictRates_DistrictRateId",
                table: "DistrictWorkflows",
                column: "DistrictRateId",
                principalTable: "DistrictRates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DistrictWorkflows_DistrictRates_DistrictRateId",
                table: "DistrictWorkflows");

            migrationBuilder.DropIndex(
                name: "IX_DistrictWorkflows_DistrictRateId",
                table: "DistrictWorkflows");

            migrationBuilder.DropColumn(
                name: "DistrictRateId",
                table: "DistrictWorkflows");
        }
    }
}
