using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StructureCategoryDescriptionOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StructureCategoryOptions",
                table: "StructureCategories");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "StructureDescriptionNames",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StructureDescriptionNames_CategoryId",
                table: "StructureDescriptionNames",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_StructureDescriptionNames_StructureCategories_CategoryId",
                table: "StructureDescriptionNames",
                column: "CategoryId",
                principalTable: "StructureCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StructureDescriptionNames_StructureCategories_CategoryId",
                table: "StructureDescriptionNames");

            migrationBuilder.DropIndex(
                name: "IX_StructureDescriptionNames_CategoryId",
                table: "StructureDescriptionNames");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "StructureDescriptionNames");

            migrationBuilder.AddColumn<string>(
                name: "StructureCategoryOptions",
                table: "StructureCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
