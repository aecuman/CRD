using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModerationLogicUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryInfo_Plant_TreeId",
                table: "CategoryInfo");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryInfo_Plant_TreeId",
                table: "CategoryInfo",
                column: "TreeId",
                principalTable: "Plant",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryInfo_Plant_TreeId",
                table: "CategoryInfo");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryInfo_Plant_TreeId",
                table: "CategoryInfo",
                column: "TreeId",
                principalTable: "Plant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
