using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class StructureRates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StructureRateId",
                table: "CompensationRateModerations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StructureRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistrictRateId = table.Column<int>(type: "int", nullable: false),
                    StructureId = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Assumptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscretionInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsReviewDeferred = table.Column<bool>(type: "bit", nullable: false),
                    DeferredReviewReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeferredReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StructureRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StructureRates_DistrictRates_DistrictRateId",
                        column: x => x.DistrictRateId,
                        principalTable: "DistrictRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StructureRates_Structures_StructureId",
                        column: x => x.StructureId,
                        principalTable: "Structures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompensationRateModerations_StructureRateId",
                table: "CompensationRateModerations",
                column: "StructureRateId");

            migrationBuilder.CreateIndex(
                name: "IX_StructureRates_DistrictRateId",
                table: "StructureRates",
                column: "DistrictRateId");

            migrationBuilder.CreateIndex(
                name: "IX_StructureRates_StructureId",
                table: "StructureRates",
                column: "StructureId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompensationRateModerations_StructureRates_StructureRateId",
                table: "CompensationRateModerations",
                column: "StructureRateId",
                principalTable: "StructureRates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompensationRateModerations_StructureRates_StructureRateId",
                table: "CompensationRateModerations");

            migrationBuilder.DropTable(
                name: "StructureRates");

            migrationBuilder.DropIndex(
                name: "IX_CompensationRateModerations_StructureRateId",
                table: "CompensationRateModerations");

            migrationBuilder.DropColumn(
                name: "StructureRateId",
                table: "CompensationRateModerations");
        }
    }
}
