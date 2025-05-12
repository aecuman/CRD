using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DescretionInfoToModeration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompensationRateModerations_StructureRates_StructureRateId",
                table: "CompensationRateModerations");

            migrationBuilder.AlterColumn<int>(
                name: "PlantRateId",
                table: "CompensationRateModerations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "DiscretionInfo",
                table: "CompensationRateModerations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewDiscretionInfo",
                table: "CompensationRateModerations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CompensationRateModerations_StructureRates_StructureRateId",
                table: "CompensationRateModerations",
                column: "StructureRateId",
                principalTable: "StructureRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompensationRateModerations_StructureRates_StructureRateId",
                table: "CompensationRateModerations");

            migrationBuilder.DropColumn(
                name: "DiscretionInfo",
                table: "CompensationRateModerations");

            migrationBuilder.DropColumn(
                name: "NewDiscretionInfo",
                table: "CompensationRateModerations");

            migrationBuilder.AlterColumn<int>(
                name: "PlantRateId",
                table: "CompensationRateModerations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CompensationRateModerations_StructureRates_StructureRateId",
                table: "CompensationRateModerations",
                column: "StructureRateId",
                principalTable: "StructureRates",
                principalColumn: "Id");
        }
    }
}
