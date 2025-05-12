using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RatesUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CRDFile_Crops_CropId",
                table: "CRDFile");

            migrationBuilder.DropForeignKey(
                name: "FK_CRDFile_DistrictRates_DistrictRateId",
                table: "CRDFile");

            migrationBuilder.DropForeignKey(
                name: "FK_CRDFile_Trees_RefId",
                table: "CRDFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CRDFile",
                table: "CRDFile");

            migrationBuilder.RenameTable(
                name: "CRDFile",
                newName: "CRDFiles");

            migrationBuilder.RenameIndex(
                name: "IX_CRDFile_RefId",
                table: "CRDFiles",
                newName: "IX_CRDFiles_RefId");

            migrationBuilder.RenameIndex(
                name: "IX_CRDFile_DistrictRateId",
                table: "CRDFiles",
                newName: "IX_CRDFiles_DistrictRateId");

            migrationBuilder.RenameIndex(
                name: "IX_CRDFile_CropId",
                table: "CRDFiles",
                newName: "IX_CRDFiles_CropId");

            migrationBuilder.AddColumn<int>(
                name: "DistrictId1",
                table: "DistrictRates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UploadsIds",
                table: "DistrictRates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CRDFiles",
                table: "CRDFiles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DistrictRates_DistrictId1",
                table: "DistrictRates",
                column: "DistrictId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFiles_Crops_CropId",
                table: "CRDFiles",
                column: "CropId",
                principalTable: "Crops",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFiles_DistrictRates_DistrictRateId",
                table: "CRDFiles",
                column: "DistrictRateId",
                principalTable: "DistrictRates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFiles_Trees_RefId",
                table: "CRDFiles",
                column: "RefId",
                principalTable: "Trees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictRates_Districts_DistrictId1",
                table: "DistrictRates",
                column: "DistrictId1",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CRDFiles_Crops_CropId",
                table: "CRDFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CRDFiles_DistrictRates_DistrictRateId",
                table: "CRDFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CRDFiles_Trees_RefId",
                table: "CRDFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrictRates_Districts_DistrictId1",
                table: "DistrictRates");

            migrationBuilder.DropIndex(
                name: "IX_DistrictRates_DistrictId1",
                table: "DistrictRates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CRDFiles",
                table: "CRDFiles");

            migrationBuilder.DropColumn(
                name: "DistrictId1",
                table: "DistrictRates");

            migrationBuilder.DropColumn(
                name: "UploadsIds",
                table: "DistrictRates");

            migrationBuilder.RenameTable(
                name: "CRDFiles",
                newName: "CRDFile");

            migrationBuilder.RenameIndex(
                name: "IX_CRDFiles_RefId",
                table: "CRDFile",
                newName: "IX_CRDFile_RefId");

            migrationBuilder.RenameIndex(
                name: "IX_CRDFiles_DistrictRateId",
                table: "CRDFile",
                newName: "IX_CRDFile_DistrictRateId");

            migrationBuilder.RenameIndex(
                name: "IX_CRDFiles_CropId",
                table: "CRDFile",
                newName: "IX_CRDFile_CropId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CRDFile",
                table: "CRDFile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFile_Crops_CropId",
                table: "CRDFile",
                column: "CropId",
                principalTable: "Crops",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFile_DistrictRates_DistrictRateId",
                table: "CRDFile",
                column: "DistrictRateId",
                principalTable: "DistrictRates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFile_Trees_RefId",
                table: "CRDFile",
                column: "RefId",
                principalTable: "Trees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
