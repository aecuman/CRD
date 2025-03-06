using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StructureRepoGPT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StructureAttributeSelections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StructureId = table.Column<int>(type: "int", nullable: false),
                    AttributeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StructureAttributeSelections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StructureAttributeSelections_StructureAttributes_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "StructureAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StructureAttributeSelections_Structures_StructureId",
                        column: x => x.StructureId,
                        principalTable: "Structures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StructureOptionSelections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttributeSelectionId = table.Column<int>(type: "int", nullable: false),
                    OptionId = table.Column<int>(type: "int", nullable: false),
                    StructureAttributeSelectionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StructureOptionSelections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StructureOptionSelections_StructureAttributeSelections_StructureAttributeSelectionId",
                        column: x => x.StructureAttributeSelectionId,
                        principalTable: "StructureAttributeSelections",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StructureOptionSelections_StructureOptions_OptionId",
                        column: x => x.OptionId,
                        principalTable: "StructureOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StructureAttributeSelections_AttributeId",
                table: "StructureAttributeSelections",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_StructureAttributeSelections_StructureId",
                table: "StructureAttributeSelections",
                column: "StructureId");

            migrationBuilder.CreateIndex(
                name: "IX_StructureOptionSelections_OptionId",
                table: "StructureOptionSelections",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_StructureOptionSelections_StructureAttributeSelectionId",
                table: "StructureOptionSelections",
                column: "StructureAttributeSelectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StructureOptionSelections");

            migrationBuilder.DropTable(
                name: "StructureAttributeSelections");
        }
    }
}
