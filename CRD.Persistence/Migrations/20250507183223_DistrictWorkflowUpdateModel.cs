using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRD.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DistrictWorkflowUpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_DistrictWorkflowStep_DistrictWorkflowStepId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_DistrictWorkflowSubStep_DistrictWorkflowSubStepId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrictWorkflow_Districts_DistrictId",
                table: "DistrictWorkflow");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrictWorkflow_Workflows_WorkflowId",
                table: "DistrictWorkflow");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrictWorkflowStep_DistrictWorkflow_DistrictWorkflowId",
                table: "DistrictWorkflowStep");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrictWorkflowStep_WorkflowSteps_WorkflowStepId",
                table: "DistrictWorkflowStep");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrictWorkflowSubStep_DistrictWorkflowStep_DistrictWorkflowStepId",
                table: "DistrictWorkflowSubStep");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrictWorkflowSubStep_WorkflowSubSteps_WorkflowSubStepId",
                table: "DistrictWorkflowSubStep");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DistrictWorkflowStep_DistrictWorkflowStepId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DistrictWorkflowSubStep_DistrictWorkflowSubStepId",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DistrictWorkflowSubStep",
                table: "DistrictWorkflowSubStep");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DistrictWorkflowStep",
                table: "DistrictWorkflowStep");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DistrictWorkflow",
                table: "DistrictWorkflow");

            migrationBuilder.RenameTable(
                name: "DistrictWorkflowSubStep",
                newName: "DistrictWorkflowSubSteps");

            migrationBuilder.RenameTable(
                name: "DistrictWorkflowStep",
                newName: "DistrictWorkflowSteps");

            migrationBuilder.RenameTable(
                name: "DistrictWorkflow",
                newName: "DistrictWorkflows");

            migrationBuilder.RenameIndex(
                name: "IX_DistrictWorkflowSubStep_WorkflowSubStepId",
                table: "DistrictWorkflowSubSteps",
                newName: "IX_DistrictWorkflowSubSteps_WorkflowSubStepId");

            migrationBuilder.RenameIndex(
                name: "IX_DistrictWorkflowSubStep_DistrictWorkflowStepId",
                table: "DistrictWorkflowSubSteps",
                newName: "IX_DistrictWorkflowSubSteps_DistrictWorkflowStepId");

            migrationBuilder.RenameIndex(
                name: "IX_DistrictWorkflowStep_WorkflowStepId",
                table: "DistrictWorkflowSteps",
                newName: "IX_DistrictWorkflowSteps_WorkflowStepId");

            migrationBuilder.RenameIndex(
                name: "IX_DistrictWorkflowStep_DistrictWorkflowId",
                table: "DistrictWorkflowSteps",
                newName: "IX_DistrictWorkflowSteps_DistrictWorkflowId");

            migrationBuilder.RenameIndex(
                name: "IX_DistrictWorkflow_WorkflowId",
                table: "DistrictWorkflows",
                newName: "IX_DistrictWorkflows_WorkflowId");

            migrationBuilder.RenameIndex(
                name: "IX_DistrictWorkflow_DistrictId",
                table: "DistrictWorkflows",
                newName: "IX_DistrictWorkflows_DistrictId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DistrictWorkflowSubSteps",
                table: "DistrictWorkflowSubSteps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DistrictWorkflowSteps",
                table: "DistrictWorkflowSteps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DistrictWorkflows",
                table: "DistrictWorkflows",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_DistrictWorkflowSteps_DistrictWorkflowStepId",
                table: "Comments",
                column: "DistrictWorkflowStepId",
                principalTable: "DistrictWorkflowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_DistrictWorkflowSubSteps_DistrictWorkflowSubStepId",
                table: "Comments",
                column: "DistrictWorkflowSubStepId",
                principalTable: "DistrictWorkflowSubSteps",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictWorkflows_Districts_DistrictId",
                table: "DistrictWorkflows",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictWorkflows_Workflows_WorkflowId",
                table: "DistrictWorkflows",
                column: "WorkflowId",
                principalTable: "Workflows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictWorkflowSteps_DistrictWorkflows_DistrictWorkflowId",
                table: "DistrictWorkflowSteps",
                column: "DistrictWorkflowId",
                principalTable: "DistrictWorkflows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictWorkflowSteps_WorkflowSteps_WorkflowStepId",
                table: "DistrictWorkflowSteps",
                column: "WorkflowStepId",
                principalTable: "WorkflowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictWorkflowSubSteps_DistrictWorkflowSteps_DistrictWorkflowStepId",
                table: "DistrictWorkflowSubSteps",
                column: "DistrictWorkflowStepId",
                principalTable: "DistrictWorkflowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictWorkflowSubSteps_WorkflowSubSteps_WorkflowSubStepId",
                table: "DistrictWorkflowSubSteps",
                column: "WorkflowSubStepId",
                principalTable: "WorkflowSubSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DistrictWorkflowSteps_DistrictWorkflowStepId",
                table: "Documents",
                column: "DistrictWorkflowStepId",
                principalTable: "DistrictWorkflowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DistrictWorkflowSubSteps_DistrictWorkflowSubStepId",
                table: "Documents",
                column: "DistrictWorkflowSubStepId",
                principalTable: "DistrictWorkflowSubSteps",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_DistrictWorkflowSteps_DistrictWorkflowStepId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_DistrictWorkflowSubSteps_DistrictWorkflowSubStepId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrictWorkflows_Districts_DistrictId",
                table: "DistrictWorkflows");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrictWorkflows_Workflows_WorkflowId",
                table: "DistrictWorkflows");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrictWorkflowSteps_DistrictWorkflows_DistrictWorkflowId",
                table: "DistrictWorkflowSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrictWorkflowSteps_WorkflowSteps_WorkflowStepId",
                table: "DistrictWorkflowSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrictWorkflowSubSteps_DistrictWorkflowSteps_DistrictWorkflowStepId",
                table: "DistrictWorkflowSubSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_DistrictWorkflowSubSteps_WorkflowSubSteps_WorkflowSubStepId",
                table: "DistrictWorkflowSubSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DistrictWorkflowSteps_DistrictWorkflowStepId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DistrictWorkflowSubSteps_DistrictWorkflowSubStepId",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DistrictWorkflowSubSteps",
                table: "DistrictWorkflowSubSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DistrictWorkflowSteps",
                table: "DistrictWorkflowSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DistrictWorkflows",
                table: "DistrictWorkflows");

            migrationBuilder.RenameTable(
                name: "DistrictWorkflowSubSteps",
                newName: "DistrictWorkflowSubStep");

            migrationBuilder.RenameTable(
                name: "DistrictWorkflowSteps",
                newName: "DistrictWorkflowStep");

            migrationBuilder.RenameTable(
                name: "DistrictWorkflows",
                newName: "DistrictWorkflow");

            migrationBuilder.RenameIndex(
                name: "IX_DistrictWorkflowSubSteps_WorkflowSubStepId",
                table: "DistrictWorkflowSubStep",
                newName: "IX_DistrictWorkflowSubStep_WorkflowSubStepId");

            migrationBuilder.RenameIndex(
                name: "IX_DistrictWorkflowSubSteps_DistrictWorkflowStepId",
                table: "DistrictWorkflowSubStep",
                newName: "IX_DistrictWorkflowSubStep_DistrictWorkflowStepId");

            migrationBuilder.RenameIndex(
                name: "IX_DistrictWorkflowSteps_WorkflowStepId",
                table: "DistrictWorkflowStep",
                newName: "IX_DistrictWorkflowStep_WorkflowStepId");

            migrationBuilder.RenameIndex(
                name: "IX_DistrictWorkflowSteps_DistrictWorkflowId",
                table: "DistrictWorkflowStep",
                newName: "IX_DistrictWorkflowStep_DistrictWorkflowId");

            migrationBuilder.RenameIndex(
                name: "IX_DistrictWorkflows_WorkflowId",
                table: "DistrictWorkflow",
                newName: "IX_DistrictWorkflow_WorkflowId");

            migrationBuilder.RenameIndex(
                name: "IX_DistrictWorkflows_DistrictId",
                table: "DistrictWorkflow",
                newName: "IX_DistrictWorkflow_DistrictId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DistrictWorkflowSubStep",
                table: "DistrictWorkflowSubStep",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DistrictWorkflowStep",
                table: "DistrictWorkflowStep",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DistrictWorkflow",
                table: "DistrictWorkflow",
                column: "Id");

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
                name: "FK_DistrictWorkflow_Districts_DistrictId",
                table: "DistrictWorkflow",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictWorkflow_Workflows_WorkflowId",
                table: "DistrictWorkflow",
                column: "WorkflowId",
                principalTable: "Workflows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictWorkflowStep_DistrictWorkflow_DistrictWorkflowId",
                table: "DistrictWorkflowStep",
                column: "DistrictWorkflowId",
                principalTable: "DistrictWorkflow",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictWorkflowStep_WorkflowSteps_WorkflowStepId",
                table: "DistrictWorkflowStep",
                column: "WorkflowStepId",
                principalTable: "WorkflowSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictWorkflowSubStep_DistrictWorkflowStep_DistrictWorkflowStepId",
                table: "DistrictWorkflowSubStep",
                column: "DistrictWorkflowStepId",
                principalTable: "DistrictWorkflowStep",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DistrictWorkflowSubStep_WorkflowSubSteps_WorkflowSubStepId",
                table: "DistrictWorkflowSubStep",
                column: "WorkflowSubStepId",
                principalTable: "WorkflowSubSteps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
        }
    }
}
