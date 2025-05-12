using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class GroupedItemKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupedPlantItems",
                table: "GroupedPlantItems");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "GroupedPlantItems",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupedPlantItems",
                table: "GroupedPlantItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupedPlantItems_GroupedPlantId",
                table: "GroupedPlantItems",
                column: "GroupedPlantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupedPlantItems",
                table: "GroupedPlantItems");

            migrationBuilder.DropIndex(
                name: "IX_GroupedPlantItems_GroupedPlantId",
                table: "GroupedPlantItems");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GroupedPlantItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupedPlantItems",
                table: "GroupedPlantItems",
                column: "GroupedPlantId");
        }
    }
}
