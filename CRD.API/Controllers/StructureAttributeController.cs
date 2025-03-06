
using CRD.Application.Common.ViewModels;
using CRD.Application.StructureAttributes.Commands;
using CRD.Application.StructureAttributes.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRD.API.Controllers
{
    [Route("api/structure-attributes")]
    [ApiController]
    public class StructureAttributeController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<int>> Create(CreateStructureAttributeCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut]
        public async Task<ActionResult<bool>> Update(UpdateStructureAttributeCommand command)
        {
            return await Mediator.Send(command);
        }

        /*[HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            return await _mediator.Send(new DeleteStructureAttributeCommand { Id = id });
        }*/

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StructureAttributeViewModel>>> GetAll()
        {
            return Ok(await Mediator.Send(new GetStructureAttributesQuery()));
        }
    }
}
