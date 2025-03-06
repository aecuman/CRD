using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PlantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Translations_Crops_LanguageId",
                table: "Translations");

            migrationBuilder.DropForeignKey(
                name: "FK_Translations_Trees_LanguageId",
                table: "Translations");

            migrationBuilder.DropIndex(
                name: "IX_Translations_LanguageId",
                table: "Translations");

            migrationBuilder.AddColumn<int>(
                name: "CropId",
                table: "Translations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TreeId",
                table: "Translations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Translations_CropId",
                table: "Translations",
                column: "CropId");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_TreeId",
                table: "Translations",
                column: "TreeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Translations_Crops_CropId",
                table: "Translations",
                column: "CropId",
                principalTable: "Crops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Translations_Trees_TreeId",
                table: "Translations",
                column: "TreeId",
                principalTable: "Trees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Translations_Crops_CropId",
                table: "Translations");

            migrationBuilder.DropForeignKey(
                name: "FK_Translations_Trees_TreeId",
                table: "Translations");

            migrationBuilder.DropIndex(
                name: "IX_Translations_CropId",
                table: "Translations");

            migrationBuilder.DropIndex(
                name: "IX_Translations_TreeId",
                table: "Translations");

            migrationBuilder.DropColumn(
                name: "CropId",
                table: "Translations");

            migrationBuilder.DropColumn(
                name: "TreeId",
                table: "Translations");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_LanguageId",
                table: "Translations",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Translations_Crops_LanguageId",
                table: "Translations",
                column: "LanguageId",
                principalTable: "Crops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Translations_Trees_LanguageId",
                table: "Translations",
                column: "LanguageId",
                principalTable: "Trees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
