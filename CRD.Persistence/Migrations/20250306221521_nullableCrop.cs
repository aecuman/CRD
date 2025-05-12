using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class nullableCrop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CRDFiles_DistrictRates_RefId",
                table: "CRDFiles");

            migrationBuilder.AlterColumn<string>(
                name: "Info",
                table: "Trees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Info",
                table: "Crops",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CropType",
                table: "Crops",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Aez",
                table: "Crops",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "DistrictRateId",
                table: "CRDFiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CRDFiles_DistrictRateId",
                table: "CRDFiles",
                column: "DistrictRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFiles_DistrictRates_DistrictRateId",
                table: "CRDFiles",
                column: "DistrictRateId",
                principalTable: "DistrictRates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CRDFiles_DistrictRates_DistrictRateId",
                table: "CRDFiles");

            migrationBuilder.DropIndex(
                name: "IX_CRDFiles_DistrictRateId",
                table: "CRDFiles");

            migrationBuilder.DropColumn(
                name: "DistrictRateId",
                table: "CRDFiles");

            migrationBuilder.AlterColumn<string>(
                name: "Info",
                table: "Trees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Info",
                table: "Crops",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CropType",
                table: "Crops",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Aez",
                table: "Crops",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFiles_DistrictRates_RefId",
                table: "CRDFiles",
                column: "RefId",
                principalTable: "DistrictRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
