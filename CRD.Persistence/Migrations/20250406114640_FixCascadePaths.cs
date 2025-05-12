using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadePaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StructureAttributeSelections_Structures_StructureId",
                table: "StructureAttributeSelections");

            /**/migrationBuilder.DropForeignKey(
                name: "FK_StructureOptionSelections_StructureAttributeSelections_AttributeSelectionId",
                table: "StructureOptionSelections");

            migrationBuilder.AddForeignKey(
                name: "FK_StructureAttributeSelections_Structures_StructureId",
                table: "StructureAttributeSelections",
                column: "StructureId",
                principalTable: "Structures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StructureOptionSelections_StructureAttributeSelections_AttributeSelectionId",
                table: "StructureOptionSelections",
                column: "AttributeSelectionId",
                principalTable: "StructureAttributeSelections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StructureAttributeSelections_Structures_StructureId",
                table: "StructureAttributeSelections");

            migrationBuilder.DropForeignKey(
                name: "FK_StructureOptionSelections_StructureAttributeSelections_AttributeSelectionId",
                table: "StructureOptionSelections");

            migrationBuilder.AddForeignKey(
                name: "FK_StructureAttributeSelections_Structures_StructureId",
                table: "StructureAttributeSelections",
                column: "StructureId",
                principalTable: "Structures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StructureOptionSelections_StructureAttributeSelections_AttributeSelectionId",
                table: "StructureOptionSelections",
                column: "AttributeSelectionId",
                principalTable: "StructureAttributeSelections",
                principalColumn: "Id");
        }
    }
}
