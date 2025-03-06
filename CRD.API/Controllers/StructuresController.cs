using CRD.Application.Common.Dtos;
using CRD.Application.Plants.Commands;
using CRD.Application.Plants.Queries;
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
    }

}
