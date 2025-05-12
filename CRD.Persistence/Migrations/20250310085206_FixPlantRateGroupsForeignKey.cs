using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixPlantRateGroupsForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlantRates_CategoryInfo_CategoryInfoId",
                table: "PlantRates");

            migrationBuilder.DropForeignKey(
                name: "FK_PlantRates_Plant_PlantId",
                table: "PlantRates");

            migrationBuilder.AlterColumn<string>(
                name: "PlantType",
                table: "PlantRates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "PlantId",
                table: "PlantRates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryInfoOption",
                table: "PlantRates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryInfoId",
                table: "PlantRates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "PlantRates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "PlantRates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlantId1",
                table: "PlantRates",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlantRateGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantRateId = table.Column<int>(type: "int", nullable: false),
                    PlantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantRateGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantRateGroups_PlantRates_PlantRateId",
                        column: x => x.PlantRateId,
                        principalTable: "PlantRates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlantRateGroups_Plant_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plant",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlantRates_PlantId1",
                table: "PlantRates",
                column: "PlantId1");

            migrationBuilder.CreateIndex(
                name: "IX_PlantRateGroups_PlantId",
                table: "PlantRateGroups",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantRateGroups_PlantRateId",
                table: "PlantRateGroups",
                column: "PlantRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlantRates_CategoryInfo_CategoryInfoId",
                table: "PlantRates",
                column: "CategoryInfoId",
                principalTable: "CategoryInfo",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlantRates_Plant_PlantId",
                table: "PlantRates",
                column: "PlantId",
                principalTable: "Plant",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlantRates_Plant_PlantId1",
                table: "PlantRates",
                column: "PlantId1",
                principalTable: "Plant",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlantRates_CategoryInfo_CategoryInfoId",
                table: "PlantRates");

            migrationBuilder.DropForeignKey(
                name: "FK_PlantRates_Plant_PlantId",
                table: "PlantRates");

            migrationBuilder.DropForeignKey(
                name: "FK_PlantRates_Plant_PlantId1",
                table: "PlantRates");

            migrationBuilder.DropTable(
                name: "PlantRateGroups");

            migrationBuilder.DropIndex(
                name: "IX_PlantRates_PlantId1",
                table: "PlantRates");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "PlantRates");

            migrationBuilder.DropColumn(
                name: "PlantId1",
                table: "PlantRates");

            migrationBuilder.AlterColumn<string>(
                name: "PlantType",
                table: "PlantRates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlantId",
                table: "PlantRates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryInfoOption",
                table: "PlantRates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryInfoId",
                table: "PlantRates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "PlantRates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlantRates_CategoryInfo_CategoryInfoId",
                table: "PlantRates",
                column: "CategoryInfoId",
                principalTable: "CategoryInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlantRates_Plant_PlantId",
                table: "PlantRates",
                column: "PlantId",
                principalTable: "Plant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
