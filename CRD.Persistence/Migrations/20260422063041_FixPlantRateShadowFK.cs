using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixPlantRateShadowFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlantRates_Plant_PlantId1",
                table: "PlantRates");

            migrationBuilder.DropIndex(
                name: "IX_PlantRates_PlantId1",
                table: "PlantRates");

            migrationBuilder.DropColumn(
                name: "PlantId1",
                table: "PlantRates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlantId1",
                table: "PlantRates",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlantRates_PlantId1",
                table: "PlantRates",
                column: "PlantId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PlantRates_Plant_PlantId1",
                table: "PlantRates",
                column: "PlantId1",
                principalTable: "Plant",
                principalColumn: "Id");
        }
    }
}
