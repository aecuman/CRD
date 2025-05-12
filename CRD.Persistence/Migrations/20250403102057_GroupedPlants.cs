using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class GroupedPlants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupedPlants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupedPlants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupedPlantItems",
                columns: table => new
                {
                    GroupedPlantId = table.Column<int>(type: "int", nullable: false),
                    CropId = table.Column<int>(type: "int", nullable: true),
                    TreeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupedPlantItems", x => x.GroupedPlantId);
                    table.ForeignKey(
                        name: "FK_GroupedPlantItems_GroupedPlants_GroupedPlantId",
                        column: x => x.GroupedPlantId,
                        principalTable: "GroupedPlants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupedPlantItems_Plant_CropId",
                        column: x => x.CropId,
                        principalTable: "Plant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupedPlantItems_Plant_TreeId",
                        column: x => x.TreeId,
                        principalTable: "Plant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupedPlantItems_CropId",
                table: "GroupedPlantItems",
                column: "CropId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupedPlantItems_TreeId",
                table: "GroupedPlantItems",
                column: "TreeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupedPlantItems");

            migrationBuilder.DropTable(
                name: "GroupedPlants");
        }
    }
}
