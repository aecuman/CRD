using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StructureGPT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StructureDescription");

            migrationBuilder.DropTable(
                name: "StructureDescriptionOptions");

            migrationBuilder.DropTable(
                name: "StructureDescriptionNames");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationRoleId",
                table: "AspNetUserRoles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "AspNetUserRoles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StructureAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StructureAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StructureAttributes_StructureCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "StructureCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StructureOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttributeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StructureOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StructureOptions_StructureAttributes_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "StructureAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Structures_CategoryId",
                table: "Structures",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Structures_StructureTypeId",
                table: "Structures",
                column: "StructureTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_ApplicationRoleId",
                table: "AspNetUserRoles",
                column: "ApplicationRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_ApplicationUserId",
                table: "AspNetUserRoles",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StructureAttributes_CategoryId",
                table: "StructureAttributes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StructureOptions_AttributeId",
                table: "StructureOptions",
                column: "AttributeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_ApplicationRoleId",
                table: "AspNetUserRoles",
                column: "ApplicationRoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_ApplicationUserId",
                table: "AspNetUserRoles",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Structures_StructureCategories_CategoryId",
                table: "Structures",
                column: "CategoryId",
                principalTable: "StructureCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Structures_StructureTypes_StructureTypeId",
                table: "Structures",
                column: "StructureTypeId",
                principalTable: "StructureTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_ApplicationRoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_ApplicationUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Structures_StructureCategories_CategoryId",
                table: "Structures");

            migrationBuilder.DropForeignKey(
                name: "FK_Structures_StructureTypes_StructureTypeId",
                table: "Structures");

            migrationBuilder.DropTable(
                name: "StructureOptions");

            migrationBuilder.DropTable(
                name: "StructureAttributes");

            migrationBuilder.DropIndex(
                name: "IX_Structures_CategoryId",
                table: "Structures");

            migrationBuilder.DropIndex(
                name: "IX_Structures_StructureTypeId",
                table: "Structures");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_ApplicationRoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_ApplicationUserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "ApplicationRoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AspNetUserRoles");

            migrationBuilder.CreateTable(
                name: "StructureDescription",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DescriptionOptionValue = table.Column<int>(type: "int", nullable: false),
                    StructureDescriptionNameId = table.Column<int>(type: "int", nullable: false),
                    StructureId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StructureDescription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StructureDescription_Structures_StructureId",
                        column: x => x.StructureId,
                        principalTable: "Structures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StructureDescriptionNames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StructureDescriptionNames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StructureDescriptionNames_StructureCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "StructureCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StructureDescriptionOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StructureDescriptionNameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StructureDescriptionOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StructureDescriptionOptions_StructureDescriptionNames_StructureDescriptionNameId",
                        column: x => x.StructureDescriptionNameId,
                        principalTable: "StructureDescriptionNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StructureDescription_StructureId",
                table: "StructureDescription",
                column: "StructureId");

            migrationBuilder.CreateIndex(
                name: "IX_StructureDescriptionNames_CategoryId",
                table: "StructureDescriptionNames",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StructureDescriptionOptions_StructureDescriptionNameId",
                table: "StructureDescriptionOptions",
                column: "StructureDescriptionNameId");
        }
    }
}
