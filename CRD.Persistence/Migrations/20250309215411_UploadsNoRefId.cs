using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UploadsNoRefId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CRDFiles_Plant_RefId",
                table: "CRDFiles");

            migrationBuilder.DropIndex(
                name: "IX_CRDFiles_RefId",
                table: "CRDFiles");

            migrationBuilder.DropColumn(
                name: "RefId",
                table: "CRDFiles");

            migrationBuilder.AddColumn<int>(
                name: "PlantId",
                table: "CRDFiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CRDFiles_PlantId",
                table: "CRDFiles",
                column: "PlantId");

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFiles_Plant_PlantId",
                table: "CRDFiles",
                column: "PlantId",
                principalTable: "Plant",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CRDFiles_Plant_PlantId",
                table: "CRDFiles");

            migrationBuilder.DropIndex(
                name: "IX_CRDFiles_PlantId",
                table: "CRDFiles");

            migrationBuilder.DropColumn(
                name: "PlantId",
                table: "CRDFiles");

            migrationBuilder.AddColumn<int>(
                name: "RefId",
                table: "CRDFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CRDFiles_RefId",
                table: "CRDFiles",
                column: "RefId");

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFiles_Plant_RefId",
                table: "CRDFiles",
                column: "RefId",
                principalTable: "Plant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
