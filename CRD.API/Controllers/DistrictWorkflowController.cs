using CRD.Application.DistrictWorkflows.Commands;
using CRD.Application.DistrictWorkflows.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRD.API.Controllers
{
    [ApiController]
    [Route("api/district-workflow")]
    public class DistrictWorkflowController : BaseController
    {
   

        // ✅ Start a workflow for a district
        [HttpPost("start",Name = "StartWorkflow")]
        public async Task<IActionResult> StartWorkflow([FromBody] StartDistrictWorkflowCommand command)
        {
            var result = await Mediator.Send(command);
            return result ? Ok("Workflow started successfully.") : BadRequest("Failed to start workflow.");
        }

        // ✅ Mark a step as completed
        [HttpPost("step/complete",Name = "CompleteStep")]
        public async Task<IActionResult> CompleteStep([FromBody] CompleteDistrictStepCommand command)
        {
            var result = await Mediator.Send(command);
            return result ? Ok("Step completed.") : BadRequest("Step completion failed.");
        }

        // ✅ Mark a substep as completed
        [HttpPost("substep/complete",Name = "CompleteSubStep")]
        public async Task<IActionResult> CompleteSubStep([FromBody] CompleteDistrictSubStepCommand command)
        {
            var result = await Mediator.Send(command);
            return result ? Ok("Substep completed.") : BadRequest("Substep completion failed.");
        }

        // ✅ Add a comment
        [HttpPost("comment",Name = "AddComment")]
        public async Task<IActionResult> AddComment([FromBody] AddCommentCommand command)
        {
            var result = await Mediator.Send(command);
            return result ? Ok("Comment added.") : BadRequest("Comment failed.");
        }

        // ✅ Get the full progress for a district
        [HttpGet("status/{districtId}/{districtRateId}",Name = "GetDistrictWorkflowStatus")]
        public async Task<ActionResult<List<DistrictWorkflowStatusDto>>> GetStatus([FromRoute] int districtId, [FromRoute] int districtRateId)
        {
            var result = await Mediator.Send(new GetDistrictWorkflowStatusQuery { DistrictId = districtId, DistrictRateId=districtRateId });
            return Ok(result);
        }
        [HttpDelete("comment/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
           // await _mediator.Send(new DeleteCommentCommand { CommentId = id });
            return Ok();
        }
        [HttpPost("upload-document", Name = "UploadWorkflowDocument")]
        public async Task<IActionResult> UploadDocument( IFormFile file, int districtWorkflowSubStepId)
        {
            var command = new UploadSubStepDocumentCommand
            {
                File = file,
                
                DistrictWorkflowSubStepId = districtWorkflowSubStepId
            };

            var result = await Mediator.Send(command);
            return result ? Ok("Uploaded") : BadRequest("Upload failed.");
        }
        [HttpPost("step/revert/{stepId}", Name = "RevertStep")]
        public async Task<IActionResult> RevertStep(int stepId)
        {
            var result = await Mediator.Send(new RevertDistrictStepCommand { DistrictStepId = stepId });
            return result ? Ok() : BadRequest("Failed to revert step.");
        }
        [HttpPost("substep/revert/{subStepId}",Name = "RevertSubStep")]
        public async Task<IActionResult> RevertSubStep(int subStepId)
        {
            var result = await Mediator.Send(new RevertDistrictSubStepCommand { DistrictSubStepId = subStepId });
            return result ? Ok() : BadRequest("Failed to revert substep.");
        }


    }

}
