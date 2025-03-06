using CRD.Application.Options.Commands;
using CRD.Application.Options.Queries;
using CRD.Application.Plants.Commands;
using CRD.Application.Plants.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRD.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantsController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlantListViewModel>>> GetPlants()
        {
            var list = await Mediator.Send(new GetPlantListQuery());
            return Ok(list);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateCropCommand cropCommand)
        {
            try
            {
                var command = await Mediator.Send(cropCommand);
                return Ok(command);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public async Task<ActionResult> Edit([FromBody] UpdatePlantCommand plantCommand)
        {
            try
            {
                await Mediator.Send(plantCommand);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public async Task<ActionResult> Delete(int id, string planttype)
        {
            try
            {
                var response = await Mediator.Send(new DeletePlantCommand() { Id = id, PlantType = planttype });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
