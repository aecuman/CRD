using CRD.Application.Common.Dtos;
using CRD.Application.Plants.Commands;
using CRD.Application.Plants.Queries;
using CRD.Application.StructureRates.Commands;
using CRD.Application.StructureRates.Queries;
using CRD.Application.Structures.Commands;
using CRD.Application.Structures.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRD.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StructuresController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StructureViewDto>>> Get()
        {
            var list = await Mediator.Send(new GetStructuresListQuery());
            return Ok(list);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateStructureCommand structureCommand)
        {
            try
            {
                var command = await Mediator.Send(structureCommand);
                return Ok(command);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public async Task<ActionResult> Edit([FromBody] UpdateStructureCommand structureCommand)
        {
            try
            {
                await Mediator.Send(structureCommand);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var response = await Mediator.Send(new DeleteStructureCommand() { Id = id});
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("structure-rates/matrix", Name = "SubmitStructureRateMatrix")]
        public async Task<IActionResult> SubmitStructureMatrix([FromBody] SubmitStructureRateMatrixCommand command)
        {
            var result = await Mediator.Send(command);
            return result ? Ok("Matrix submitted.") : BadRequest("Failed to submit.");
        }
        [HttpPost("structure-rate",Name ="UpsertStructureRate")]
        public async Task<IActionResult> CreateOrUpdateStructureRate([FromBody] CreateOrUpdateStructureRateCommand command)
        {
            var id = await Mediator.Send(command);
            return Ok(id);
        }
        [HttpGet("structure-rates", Name = "GetStructureRates")]
        public async Task<ActionResult<List<StructureRatesListViewModel>>> GetStructureRates([FromQuery] GetStructureRatesQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
        [HttpDelete("structure-rate/{id}", Name = "DeleteStructureRate")]
        public async Task<IActionResult> DeleteStructureRate(int id)
        {
            var success = await Mediator.Send(new DeleteStructureRateCommand { Id = id });
            return success ? Ok() : NotFound();
        }

    }

}
