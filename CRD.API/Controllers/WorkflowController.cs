using CRD.Application.Workflows;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRD.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowController : BaseController
    {
        [HttpPost("assign-workflow/{districtId}/{workflowId}")]
        public async Task<IActionResult> AssignWorkflow(int districtId, int workflowId)
        {
            var result = await Mediator.Send(new AssignWorkflowCommand(districtId, workflowId));
            return Ok(result);
        }

        [HttpPost("confirm-step/{stepId}")]
        public async Task<IActionResult> ConfirmStep(int stepId)
        {
            var result = await Mediator.Send(new ConfirmStepCommand(stepId));
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("districts/status")]
        public async Task<IActionResult> GetDistrictStatuses()
        {
            var result = await Mediator.Send(new GetDistrictStatusesQuery());
            return Ok(result);
        }
        /*
        [HttpPost("add-comment")]
        public async Task<IActionResult> AddComment([FromBody] AddCommentCommand command)
        {
            var result = await Mediator.Send(command);
            return result == null ? NotFound() : Ok(result);
        }
        */
        [HttpPost("create-workflow")]
        public async Task<IActionResult> CreateWorkflow([FromBody] CreateWorkflowCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("update-workflow/{workflowId}")]
        public async Task<IActionResult> UpdateWorkflow(int workflowId, [FromBody] UpdateWorkflowCommand command)
        {
            var result = await Mediator.Send(new UpdateWorkflowCommand(workflowId, command.Name, command.Description));
            return Ok(result);
        }

        [HttpDelete("delete-workflow/{workflowId}")]
        public async Task<IActionResult> DeleteWorkflow(int workflowId)
        {
            var result = await Mediator.Send(new DeleteWorkflowCommand(workflowId));
            return Ok(result);
        }

        [HttpPost("add-step/{workflowId}")]
        public async Task<IActionResult> AddStep(int workflowId, [FromBody] AddStepCommand command)
        {
            var result = await Mediator.Send(new AddStepCommand(workflowId, command.Name, command.AssignedToRoles));
            return Ok(result);
        }

        [HttpDelete("delete-step/{stepId}")]
        public async Task<IActionResult> DeleteStep(int stepId)
        {
            var result = await Mediator.Send(new DeleteStepCommand(stepId));
            return Ok(result);
        }
        [HttpPost("add-substep/{stepId}")]
        public async Task<IActionResult> AddSubStep(int stepId, [FromBody] AddSubStepCommand command)
        {
            var result = await Mediator.Send(new AddSubStepCommand(stepId, command.Name, command.AssignedToRoles));
            return Ok(result);
        }

        [HttpDelete("delete-substep/{subStepId}")]
        public async Task<IActionResult> DeleteSubStep(int subStepId)
        {
            var result = await Mediator.Send(new DeleteSubStepCommand(subStepId));
            return Ok(result);
        }
    }
}
