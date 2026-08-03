using CRD.Application.StructureTypes.Commands;
using CRD.Application.StructureTypes.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRD.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StructureTypesController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StructureTypeViewModel>>> GetAll()
        {
            var result = await Mediator.Send(new GetAllStructureTypesQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateStructureTypeCommand command)
        {
            try
            {
                var id = await Mediator.Send(command);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<bool>> Update([FromBody] UpdateStructureTypeCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                var result = await Mediator.Send(new DeleteStructureTypeCommand { Id = id });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
