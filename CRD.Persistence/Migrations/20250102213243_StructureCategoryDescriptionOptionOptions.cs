using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StructureCategoryDescriptionOptionOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_StructureDescriptionOptions_StructureDescriptionNameId",
                table: "StructureDescriptionOptions",
                column: "StructureDescriptionNameId");

            migrationBuilder.AddForeignKey(
                name: "FK_StructureDescriptionOptions_StructureDescriptionNames_StructureDescriptionNameId",
                table: "StructureDescriptionOptions",
                column: "StructureDescriptionNameId",
                principalTable: "StructureDescriptionNames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StructureDescriptionOptions_StructureDescriptionNames_StructureDescriptionNameId",
                table: "StructureDescriptionOptions");

            migrationBuilder.DropIndex(
                name: "IX_StructureDescriptionOptions_StructureDescriptionNameId",
                table: "StructureDescriptionOptions");
        }
    }
}
