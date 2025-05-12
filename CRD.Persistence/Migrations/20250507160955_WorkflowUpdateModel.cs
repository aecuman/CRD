using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class WorkflowUpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoles_WorkflowSteps_WorkflowStepId",
                table: "AspNetRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_WorkflowSteps_WorkflowStepId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_WorkflowSubSteps_WorkflowSubStepId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_WorkflowSteps_WorkflowStepId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_WorkflowSubSteps_WorkflowSubStepId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowSteps_DistrictWorkflow_DistrictId",
                table: "WorkflowSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowSteps_Workflows_WorkflowId",
                table: "WorkflowSteps");

            migrationBuilder.DropIndex(
                name: "IX_WorkflowSteps_DistrictId",
                table: "WorkflowSteps");

            migrationBuilder.DropIndex(
                name: "IX_Documents_WorkflowSubStepId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoles_WorkflowStepId",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "WorkflowSubSteps");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "WorkflowSteps");

            migrationBuilder.DropColumn(
                name: "WorkflowSubStepId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "WorkflowStepId",
                table: "AspNetRoles");

            migrationBuilder.RenameColumn(
                name: "WorkflowStepId",
                table: "Documents",
                newName: "DistrictWorkflowSubStepId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_WorkflowStepId",
                table: "Documents",
                newName: "IX_Documents_DistrictWorkflowSubStepId");

            migrationBuilder.RenameColumn(
                name: "WorkflowSubStepId",
                table: "Comments",
                newName: "DistrictWorkflowSubStepId");

            migrationBuilder.RenameColumn(
                name: "WorkflowStepId",
                table: "Comments",
                newName: "DistrictWorkflowStepId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_WorkflowSubStepId",
                table: "Comments",
                newName: "IX_Comments_DistrictWorkflowSubStepId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_WorkflowStepId",
                table: "Comments",
                newName: "IX_Comments_DistrictWorkflowStepId");

            migrationBuilder.AddColumn<int>(
                name: "DistrictWorkflowStepId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                table: "DistrictWorkflow",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                table: "DistrictWorkflow",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "DistrictWorkflowStep",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowStepId = table.Column<int>(type: "int", nullable: false),
                    DistrictWorkflowId = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistrictWorkflowStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistrictWorkflowStep_DistrictWorkflow_DistrictWorkflowId",
                        column: x => x.DistrictWorkflowId,
                        principalTable: "DistrictWorkflow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DistrictWorkflowStep_WorkflowSteps_WorkflowStepId",
                        column: x => x.WorkflowStepId,
                        principalTable: "WorkflowSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DistrictWorkflowSubStep",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowSubStepId = table.Column<int>(type: "int", nullable: false),
                    DistrictWorkflowStepId = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistrictWorkflowSubStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistrictWorkflowSubStep_DistrictWorkflowStep_DistrictWorkflowStepId",
                        column: x => x.DistrictWorkflowStepId,
                        principalTable: "DistrictWorkflowStep",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DistrictWorkflowSubStep_WorkflowSubSteps_WorkflowSubStepId",
                        column: x => x.WorkflowSubStepId,
                        principalTable: "WorkflowSubSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DistrictWorkflowStepId",
                table: "Documents",
                column: "DistrictWorkflowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_DistrictWorkflowStep_DistrictWorkflowId",
                table: "DistrictWorkflowStep",
                column: "DistrictWorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_DistrictWorkflowStep_WorkflowStepId",
                table: "DistrictWorkflowStep",
                column: "WorkflowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_DistrictWorkflowSubStep_DistrictWorkflowStepId",
                table: "DistrictWorkflowSubStep",
                column: "DistrictWorkflowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_DistrictWorkflowSubStep_WorkflowSubStepId",
                table: "DistrictWorkflowSubStep",
                column: "WorkflowSubStepId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_DistrictWorkflowStep_DistrictWorkflowStepId",
                table: "Comments",
                column: "DistrictWorkflowStepId",
                principalTable: "DistrictWorkflowStep",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_DistrictWorkflowSubStep_DistrictWorkflowSubStepId",
                table: "Comments",
                column: "DistrictWorkflowSubStepId",
                principalTable: "DistrictWorkflowSubStep",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DistrictWorkflowStep_DistrictWorkflowStepId",
                table: "Documents",
                column: "DistrictWorkflowStepId",
                principalTable: "DistrictWorkflowStep",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DistrictWorkflowSubStep_DistrictWorkflowSubStepId",
                table: "Documents",
                column: "DistrictWorkflowSubStepId",
                principalTable: "DistrictWorkflowSubStep",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowSteps_Workflows_WorkflowId",
                table: "WorkflowSteps",
                column: "WorkflowId",
                principalTable: "Workflows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_DistrictWorkflowStep_DistrictWorkflowStepId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_DistrictWorkflowSubStep_DistrictWorkflowSubStepId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DistrictWorkflowStep_DistrictWorkflowStepId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DistrictWorkflowSubStep_DistrictWorkflowSubStepId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowSteps_Workflows_WorkflowId",
                table: "WorkflowSteps");

            migrationBuilder.DropTable(
                name: "DistrictWorkflowSubStep");

            migrationBuilder.DropTable(
                name: "DistrictWorkflowStep");

            migrationBuilder.DropIndex(
                name: "IX_Documents_DistrictWorkflowStepId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DistrictWorkflowStepId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "DistrictWorkflow");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "DistrictWorkflow");

            migrationBuilder.RenameColumn(
                name: "DistrictWorkflowSubStepId",
                table: "Documents",
                newName: "WorkflowStepId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_DistrictWorkflowSubStepId",
                table: "Documents",
                newName: "IX_Documents_WorkflowStepId");

            migrationBuilder.RenameColumn(
                name: "DistrictWorkflowSubStepId",
                table: "Comments",
                newName: "WorkflowSubStepId");

            migrationBuilder.RenameColumn(
                name: "DistrictWorkflowStepId",
                table: "Comments",
                newName: "WorkflowStepId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_DistrictWorkflowSubStepId",
                table: "Comments",
                newName: "IX_Comments_WorkflowSubStepId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_DistrictWorkflowStepId",
                table: "Comments",
                newName: "IX_Comments_WorkflowStepId");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "WorkflowSubSteps",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "WorkflowSteps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "WorkflowSteps",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                table: "WorkflowSteps",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                table: "WorkflowSteps",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "WorkflowSubStepId",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkflowStepId",
                table: "AspNetRoles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_DistrictId",
                table: "WorkflowSteps",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_WorkflowSubStepId",
                table: "Documents",
                column: "WorkflowSubStepId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoles_WorkflowStepId",
                table: "AspNetRoles",
                column: "WorkflowStepId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoles_WorkflowSteps_WorkflowStepId",
                table: "AspNetRoles",
                column: "WorkflowStepId",
                principalTable: "WorkflowSteps",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_WorkflowSteps_WorkflowStepId",
                table: "Comments",
                column: "WorkflowStepId",
                principalTable: "WorkflowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_WorkflowSubSteps_WorkflowSubStepId",
                table: "Comments",
                column: "WorkflowSubStepId",
                principalTable: "WorkflowSubSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_WorkflowSteps_WorkflowStepId",
                table: "Documents",
                column: "WorkflowStepId",
                principalTable: "WorkflowSteps",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_WorkflowSubSteps_WorkflowSubStepId",
                table: "Documents",
                column: "WorkflowSubStepId",
                principalTable: "WorkflowSubSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowSteps_DistrictWorkflow_DistrictId",
                table: "WorkflowSteps",
                column: "DistrictId",
                principalTable: "DistrictWorkflow",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowSteps_Workflows_WorkflowId",
                table: "WorkflowSteps",
                column: "WorkflowId",
                principalTable: "Workflows",
                principalColumn: "Id");
        }
    }
}
