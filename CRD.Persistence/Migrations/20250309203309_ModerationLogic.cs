using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModerationLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryInfo_Crops_CropId",
                table: "CategoryInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryInfo_Trees_TreeId",
                table: "CategoryInfo");

           /**/ migrationBuilder.DropForeignKey(
                name: "FK_CRDFiles_Crops_CropId",
                table: "CRDFiles");
           
            migrationBuilder.DropForeignKey(
                name: "FK_CRDFiles_Trees_RefId",
                table: "CRDFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Translations_Crops_CropId",
                table: "Translations");

            migrationBuilder.DropForeignKey(
                name: "FK_Translations_Trees_TreeId",
                table: "Translations");

 /*           migrationBuilder.DropTable(
                name: "Crops");
 */
            migrationBuilder.DropIndex(
                name: "IX_Translations_CropId",
                table: "Translations");

            migrationBuilder.DropIndex(
                name: "IX_CRDFiles_CropId",
                table: "CRDFiles");

            migrationBuilder.DropIndex(
                name: "IX_CategoryInfo_CropId",
                table: "CategoryInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trees",
                table: "Trees");

            migrationBuilder.DropColumn(
                name: "CropId",
                table: "CRDFiles");

            migrationBuilder.RenameTable(
                name: "Trees",
                newName: "Plant");

            migrationBuilder.RenameColumn(
                name: "UploadsIds",
                table: "DistrictRates",
                newName: "UploadIds");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "DistrictRates",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Othernames",
                table: "Plant",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Aez",
                table: "Plant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CropType",
                table: "Plant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Plant",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Plant",
                table: "Plant",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PlantRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantId = table.Column<int>(type: "int", nullable: false),
                    PlantType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DistrictRateId = table.Column<int>(type: "int", nullable: false),
                    GrowthStageId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CategoryInfoId = table.Column<int>(type: "int", nullable: false),
                    CategoryInfoOption = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Assumptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscretionInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsReviewDeferred = table.Column<bool>(type: "bit", nullable: false),
                    DeferredReviewReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeferredReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantRates_CategoryInfo_CategoryInfoId",
                        column: x => x.CategoryInfoId,
                        principalTable: "CategoryInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlantRates_DistrictRates_DistrictRateId",
                        column: x => x.DistrictRateId,
                        principalTable: "DistrictRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlantRates_Plant_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompensationRateModerations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantRateId = table.Column<int>(type: "int", nullable: false),
                    OldRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NewRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ModerationNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Moderator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModerationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDeferred = table.Column<bool>(type: "bit", nullable: false),
                    DeferredReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompensationRateModerations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompensationRateModerations_PlantRates_PlantRateId",
                        column: x => x.PlantRateId,
                        principalTable: "PlantRates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryInfo_CategoryId",
                table: "CategoryInfo",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CompensationRateModerations_PlantRateId",
                table: "CompensationRateModerations",
                column: "PlantRateId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantRates_CategoryInfoId",
                table: "PlantRates",
                column: "CategoryInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantRates_DistrictRateId",
                table: "PlantRates",
                column: "DistrictRateId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantRates_PlantId",
                table: "PlantRates",
                column: "PlantId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryInfo_Category_CategoryId",
                table: "CategoryInfo",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryInfo_Plant_TreeId",
                table: "CategoryInfo",
                column: "TreeId",
                principalTable: "Plant",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFiles_Plant_RefId",
                table: "CRDFiles",
                column: "RefId",
                principalTable: "Plant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Translations_Plant_TreeId",
                table: "Translations",
                column: "TreeId",
                principalTable: "Plant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryInfo_Category_CategoryId",
                table: "CategoryInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryInfo_Plant_TreeId",
                table: "CategoryInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_CRDFiles_Plant_RefId",
                table: "CRDFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Translations_Plant_TreeId",
                table: "Translations");

            migrationBuilder.DropTable(
                name: "CompensationRateModerations");

            migrationBuilder.DropTable(
                name: "PlantRates");

            migrationBuilder.DropIndex(
                name: "IX_CategoryInfo_CategoryId",
                table: "CategoryInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Plant",
                table: "Plant");

            migrationBuilder.DropColumn(
                name: "Aez",
                table: "Plant");

            migrationBuilder.DropColumn(
                name: "CropType",
                table: "Plant");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Plant");

            migrationBuilder.RenameTable(
                name: "Plant",
                newName: "Trees");

            migrationBuilder.RenameColumn(
                name: "UploadIds",
                table: "DistrictRates",
                newName: "UploadsIds");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "DistrictRates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CropId",
                table: "CRDFiles",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Othernames",
                table: "Trees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trees",
                table: "Trees",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Crops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aez = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BotanicalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categories = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommonName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CropType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrowthStages = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Info = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlantType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crops", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Translations_CropId",
                table: "Translations",
                column: "CropId");

            migrationBuilder.CreateIndex(
                name: "IX_CRDFiles_CropId",
                table: "CRDFiles",
                column: "CropId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryInfo_CropId",
                table: "CategoryInfo",
                column: "CropId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryInfo_Crops_CropId",
                table: "CategoryInfo",
                column: "CropId",
                principalTable: "Crops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryInfo_Trees_TreeId",
                table: "CategoryInfo",
                column: "TreeId",
                principalTable: "Trees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFiles_Crops_CropId",
                table: "CRDFiles",
                column: "CropId",
                principalTable: "Crops",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFiles_Trees_RefId",
                table: "CRDFiles",
                column: "RefId",
                principalTable: "Trees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
