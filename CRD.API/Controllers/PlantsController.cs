using CRD.Application.Options.Commands;
using CRD.Application.Options.Queries;
using CRD.Application.PlantRates.Commands;
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

        [HttpPost("bulk-upload")]
        public async Task<IActionResult> BulkUpload([FromBody] List<CreateCropCommand> crops)
        {
            if (crops == null || crops.Count == 0)
            {
                return BadRequest("No data provided for upload.");
            }

            var results = new List<int>();

            foreach (var crop in crops)
            {
                var result = await Mediator.Send(crop);
                results.Add(result);
            }

            return Ok(new { message = $"{crops.Count} records uploaded successfully.", ids = results });
        }


        // ✅ POST: Create Compensation Rate Matrix
        [HttpPost("matrix")]
        public async Task<IActionResult> CreateCompensationRateMatrix([FromBody] CreatePlantRatesCommand request)
        {
            //  var command = new CreatePlantRatesCommand { CompensationRateMatrix = request };
            var result = await Mediator.Send(request);
            return Ok(result); //result. ? Ok("Compensation rates created successfully.") : BadRequest("Failed to create compensation rates.");
        }

        // ✅ PUT: Update Compensation Rate Matrix
        [HttpPut("matrix")]
        public async Task<IActionResult> UpdateCompensationRateMatrix([FromBody] CompensationRateMatrixDto request)
        {
            var command = new UpdatePlantRatesCommand { CompensationRateMatrix = request };
            var result = await Mediator.Send(command);
            return result ? Ok("Compensation rates updated successfully.") : BadRequest("Failed to update compensation rates.");
        }
        // GET: api/groupedplants
        [HttpGet]
        [Route("group", Name = "GetGroupedPlants")]
        public async Task<ActionResult<List<GroupedPlantListViewModel>>> GetAll()
        {
            var result = await Mediator.Send(new GetGroupedPlantsQuery());
            return Ok(result);
        }

        // GET: api/groupedplants/{id}
        [HttpGet("group/{id}", Name = "GetGroupedPlantById")]
        public async Task<ActionResult<GroupedPlantListViewModel>> GetById(int id)
        {
            var result = await Mediator.Send(new GetGroupedPlantsQuery { Id = id });
            if (result == null || result.Count == 0)
                return NotFound();
            return Ok(result[0]);
        }

        // POST: api/groupedplants
        [HttpPost("group", Name = "CreateGroupedPlant")]
        public async Task<ActionResult<int>> Create(CreateGroupedPlantsCommand command)
        {
            var id = await Mediator.Send(command);
            return Ok(id);
        }

        // PUT: api/groupedplants/{id}
        [HttpPut("group/{id}",Name = "UpdateGroupedPlant")]
        public async Task<IActionResult> Update(int id, UpdateGroupedPlantsCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            var success = await Mediator.Send(command);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/groupedplants/{id}
        [HttpDelete("group/{id}", Name = "DeleteGroupedPlant")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await Mediator.Send(new DeleteGroupedPlantsCommand { Id = id });
            if (!result)
                return NotFound();

            return NoContent();

        }
            [HttpDelete("matrix/{id}",Name = "DeletePlantRate")]
            public async Task<IActionResult> DeletePlantRate(int id)
            {
                var success = await Mediator.Send(new DeletePlantRateCommand { Id = id });
                return success ? Ok() : NotFound();
            }
        }
}
