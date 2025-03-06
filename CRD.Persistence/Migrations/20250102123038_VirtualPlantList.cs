using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VirtualPlantList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CRDFile_Crops_RefId",
                table: "CRDFile");

            migrationBuilder.AddColumn<int>(
                name: "CropId",
                table: "CRDFile",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CRDFile_CropId",
                table: "CRDFile",
                column: "CropId");

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFile_Crops_CropId",
                table: "CRDFile",
                column: "CropId",
                principalTable: "Crops",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CRDFile_Crops_CropId",
                table: "CRDFile");

            migrationBuilder.DropIndex(
                name: "IX_CRDFile_CropId",
                table: "CRDFile");

            migrationBuilder.DropColumn(
                name: "CropId",
                table: "CRDFile");

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFile_Crops_RefId",
                table: "CRDFile",
                column: "RefId",
                principalTable: "Crops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
