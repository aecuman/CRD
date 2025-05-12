using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class GlobalGroupedPlantRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupedPlantId",
                table: "PlantRates",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlantRates_GroupedPlantId",
                table: "PlantRates",
                column: "GroupedPlantId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlantRates_GroupedPlants_GroupedPlantId",
                table: "PlantRates",
                column: "GroupedPlantId",
                principalTable: "GroupedPlants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlantRates_GroupedPlants_GroupedPlantId",
                table: "PlantRates");

            migrationBuilder.DropIndex(
                name: "IX_PlantRates_GroupedPlantId",
                table: "PlantRates");

            migrationBuilder.DropColumn(
                name: "GroupedPlantId",
                table: "PlantRates");
        }
    }
}
