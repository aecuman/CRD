using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PlantCategoryInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PlantCategories",
                table: "PlantCategories");

            migrationBuilder.RenameTable(
                name: "PlantCategories",
                newName: "Category");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CategoryInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    TreeId = table.Column<int>(type: "int", nullable: true),
                    CropId = table.Column<int>(type: "int", nullable: true),
                    Info = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryInfo_Crops_CropId",
                        column: x => x.CropId,
                        principalTable: "Crops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryInfo_Trees_TreeId",
                        column: x => x.TreeId,
                        principalTable: "Trees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryInfo_CropId",
                table: "CategoryInfo",
                column: "CropId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryInfo_TreeId",
                table: "CategoryInfo",
                column: "TreeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "PlantCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlantCategories",
                table: "PlantCategories",
                column: "Id");
        }
    }
}
