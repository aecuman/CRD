using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class WorkflowMGTGPT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StructureAttributeSelections_Structures_StructureId",
                table: "StructureAttributeSelections");

            /*migrationBuilder.DropForeignKey(
                name: "FK_StructureOptionSelections_StructureAttributeSelections_StructureAttributeSelectionId",
                table: "StructureOptionSelections");
            */
            migrationBuilder.DropIndex(
                name: "IX_StructureOptionSelections_StructureAttributeSelectionId",
                table: "StructureOptionSelections");
            /*
            migrationBuilder.DropColumn(
                name: "StructureAttributeSelectionId",
                table: "StructureOptionSelections");
            */
            migrationBuilder.AddColumn<int>(
                name: "DistrictRateId",
                table: "CRDFile",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkflowStepId",
                table: "AspNetRoles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DistrictRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistrictId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistrictRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workflow",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflow", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DistrictWorkflow",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    WorkflowId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistrictWorkflow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistrictWorkflow_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DistrictWorkflow_Workflow_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    AssignedToRoles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    WorkflowId = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreviousStepId = table.Column<int>(type: "int", nullable: true),
                    NextStepId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowSteps_DistrictWorkflow_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "DistrictWorkflow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowSteps_Workflow_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflow",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowSubSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    AssignedToRoles = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkflowStepId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowSubSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowSubSteps_WorkflowSteps_WorkflowStepId",
                        column: x => x.WorkflowStepId,
                        principalTable: "WorkflowSteps",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowStepId = table.Column<int>(type: "int", nullable: true),
                    WorkflowSubStepId = table.Column<int>(type: "int", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_WorkflowSteps_WorkflowStepId",
                        column: x => x.WorkflowStepId,
                        principalTable: "WorkflowSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_WorkflowSubSteps_WorkflowSubStepId",
                        column: x => x.WorkflowSubStepId,
                        principalTable: "WorkflowSubSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkflowStepId = table.Column<int>(type: "int", nullable: true),
                    WorkflowSubStepId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_WorkflowSteps_WorkflowStepId",
                        column: x => x.WorkflowStepId,
                        principalTable: "WorkflowSteps",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Documents_WorkflowSubSteps_WorkflowSubStepId",
                        column: x => x.WorkflowSubStepId,
                        principalTable: "WorkflowSubSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StructureOptionSelections_AttributeSelectionId",
                table: "StructureOptionSelections",
                column: "AttributeSelectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CRDFile_DistrictRateId",
                table: "CRDFile",
                column: "DistrictRateId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_WorkflowStepId",
                table: "AspNetRoles",
                column: "WorkflowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_WorkflowStepId",
                table: "Comments",
                column: "WorkflowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_WorkflowSubStepId",
                table: "Comments",
                column: "WorkflowSubStepId");

            migrationBuilder.CreateIndex(
                name: "IX_DistrictWorkflow_DistrictId",
                table: "DistrictWorkflow",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_DistrictWorkflow_WorkflowId",
                table: "DistrictWorkflow",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_WorkflowStepId",
                table: "Documents",
                column: "WorkflowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_WorkflowSubStepId",
                table: "Documents",
                column: "WorkflowSubStepId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_DistrictId",
                table: "WorkflowSteps",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_WorkflowId",
                table: "WorkflowSteps",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSubSteps_WorkflowStepId",
                table: "WorkflowSubSteps",
                column: "WorkflowStepId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_WorkflowSteps_WorkflowStepId",
                table: "AspNetRoles",
                column: "WorkflowStepId",
                principalTable: "WorkflowSteps",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CRDFile_DistrictRates_DistrictRateId",
                table: "CRDFile",
                column: "DistrictRateId",
                principalTable: "DistrictRates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StructureAttributeSelections_Structures_StructureId",
                table: "StructureAttributeSelections",
                column: "StructureId",
                principalTable: "Structures",
                principalColumn: "Id");

           /**/ migrationBuilder.AddForeignKey(
                name: "FK_StructureOptionSelections_StructureAttributeSelections_AttributeSelectionId",
                table: "StructureOptionSelections",
                column: "AttributeSelectionId",
                principalTable: "StructureAttributeSelections",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_WorkflowSteps_WorkflowStepId",
                table: "AspNetRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_CRDFile_DistrictRates_DistrictRateId",
                table: "CRDFile");

            migrationBuilder.DropForeignKey(
                name: "FK_StructureAttributeSelections_Structures_StructureId",
                table: "StructureAttributeSelections");

           /**/ migrationBuilder.DropForeignKey(
                name: "FK_StructureOptionSelections_StructureAttributeSelections_AttributeSelectionId",
                table: "StructureOptionSelections");
           
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "DistrictRates");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "WorkflowSubSteps");

            migrationBuilder.DropTable(
                name: "WorkflowSteps");

            migrationBuilder.DropTable(
                name: "DistrictWorkflow");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Workflow");

            migrationBuilder.DropIndex(
                name: "IX_StructureOptionSelections_AttributeSelectionId",
                table: "StructureOptionSelections");

            migrationBuilder.DropIndex(
                name: "IX_CRDFile_DistrictRateId",
                table: "CRDFile");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_WorkflowStepId",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "DistrictRateId",
                table: "CRDFile");

            migrationBuilder.DropColumn(
                name: "WorkflowStepId",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<int>(
                name: "StructureAttributeSelectionId",
                table: "StructureOptionSelections",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StructureOptionSelections_StructureAttributeSelectionId",
                table: "StructureOptionSelections",
                column: "StructureAttributeSelectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StructureAttributeSelections_Structures_StructureId",
                table: "StructureAttributeSelections",
                column: "StructureId",
                principalTable: "Structures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

           /* migrationBuilder.AddForeignKey(
                name: "FK_StructureOptionSelections_StructureAttributeSelections_StructureAttributeSelectionId",
                table: "StructureOptionSelections",
                column: "StructureAttributeSelectionId",
                principalTable: "StructureAttributeSelections",
                principalColumn: "Id");*/
        }
    }
}
