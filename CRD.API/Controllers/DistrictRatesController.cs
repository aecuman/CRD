using CRD.Application.Districts.Commands;
using CRD.Application.Districts.Queries;
using CRD.Application.PlantRates.Commands;
using CRD.Application.PlantRates.Queries;
using CRD.Application.Plants.Commands;
using CRD.Application.Rates.Commands;
using CRD.Application.Rates.Queries;
using CRD.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace CRD.API.Controllers
{
    [Route("api/district-rates")]
    [ApiController]
    public class DistrictRatesController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<List<DistrictRateDto>>> GetAllDistrictRates(
                [FromQuery] int? year,
                [FromQuery] string? status,
                [FromQuery] int? districtId)
        {
            var query = new GetDistrictRatesQuery
            {
                Year = year,
                Status = status,
                DistrictId = districtId
            };

            var districtRates = await Mediator.Send(query);
            return Ok(districtRates);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<District>>> GetAllDistricts() => Ok(await Mediator.Send(new GetAllDistrictsQuery()));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDistrictRateCommand command)
            => Ok(await Mediator.Send(command));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDistrictRateCommand command)
        {
            if (id != command.Id) return BadRequest("Mismatched Id");
            var result = await Mediator.Send(command);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await Mediator.Send(new DeleteDistrictRateCommand(id));
            return result ? Ok() : NotFound();
        }
        [HttpPost("upload")]
        public async Task<ActionResult<int>> UploadFile(IFormFile file, [FromForm] int refId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file.");
            var fileId = await Mediator.Send(new UploadFileCommand(file, refId));
            return Ok(fileId);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<DistrictRateDto>> GetDistrictRateById(int id)
        {
            return Ok( await Mediator.Send(new GetDistrictRateByIdQuery() { Id = id }));  
        }
        [HttpGet]
        [Route("plants/{districtRateId}",Name = "GetPlantsRatesByDistrict")]
        [Route("plants/{districtRateId}/{plantId:int?}", Name = "GetPlantsRatesByDistrictPlant")]
        public async Task<ActionResult<List<PlantRateViewModel>>> GetPlantsRates(int districtRateId, int? plantId=null)
        {
            var cropRates = await Mediator.Send(new GetPlantsRatesQuery() { DistrictRateId = districtRateId, PlantId = plantId });

            return Ok(cropRates);
        }

        [HttpPut]
        [Route("plants/update",Name = "UpdateDistrictPlantRate")]
        public async Task<IActionResult> UpdateDistrictPlantRate(UpdatePlantRatesCommand command)
        {          
            var result = await Mediator.Send(command);
            return result ? Ok() : NotFound();           
        }

        [HttpPut("plant/update",Name ="UpdateSingleRate")]
        public async Task<IActionResult> UpdatePlantRate(int id, [FromBody] UpdateSinglePlantRateCommand command)
        {
            if (id != command.Id) return BadRequest("Mismatched Id");
            var result = await Mediator.Send(command);
            return result ? Ok() : NotFound();
        }


        // ✅ Get Moderation Report for a District with Filters
        [HttpGet("moderation/report/{districtRateId}",Name = "GetModerationReport")]
        public async Task<ActionResult<ModerationReportViewModel>> GetModerationReport(int districtRateId, [FromQuery] string? filter)
        {
            var query = new GetModerationReportQuery { DistrictRateId = districtRateId, Filter = filter };
            var report = await Mediator.Send(query);
            return Ok(report);
        }
        // ✅ Moderate a Compensation Rate
        [HttpPut("moderate/{id}")]
        public async Task<IActionResult> ModerateRate(int id, [FromBody] ModerateCompensationRateCommand command)
        {
            if (id != command.RateId)
                return BadRequest("ID mismatch.");

            var success = await Mediator.Send(command);
            if (!success)
                return NotFound("Compensation rate not found.");

            return NoContent();
        }

        // ✅ Get Moderation History
        [HttpGet("moderation-history/{rateId}")]
        public async Task<ActionResult<List<CompensationRateModerationViewModel>>> GetModerationHistory(int rateId)
        {
            var query = new GetModerationHistoryQuery { RateId = rateId };
            var history = await Mediator.Send(query);
            return Ok(history);
        }

        /// <summary>
        /// Gets all moderated plant rates with optional DistrictRateId filter.
        /// </summary>
        [HttpGet("moderation/plant-rates/{districtRateId}",Name = "GetModeratedPlantRates")]
        public async Task<ActionResult<List<ModeratedPlantRateViewModel>>> GetModeratedPlantRates(int? districtRateId, [FromQuery] bool includeUnmoderated = false,
        CancellationToken cancellationToken = default)
        {
            var query = new GetModeratedPlantRateQuery
            {
                DistrictRateId = districtRateId,
                IncludeUnmoderated = includeUnmoderated
            };

            var result = await Mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("moderation/structure-rates/{districtRateId}", Name = "GetModeratedStructureRates")]
        public async Task<ActionResult<List<ModeratedStructureRateViewModel>>> GetModeratedStructureRates(int? districtRateId, [FromQuery] bool includeUnmoderated = false)
        {
            var result = await Mediator.Send(new GetModeratedStructureRateQuery
            {
                DistrictRateId = districtRateId,
                IncludeUnmoderated = includeUnmoderated
            });

            return Ok(result);
        }
        [HttpGet("latest-excluding",Name = "GetLatestDistrictRates")]
        public async Task<ActionResult<List<DistrictRateDto>>> GetLatestDistrictRates([FromQuery] int excludeId, [FromQuery] string? name, [FromQuery] string? year)
        {
            var result = await Mediator.Send(new GetLatestDistrictRatesQuery
            {
                ExcludeId = excludeId,
                Name = name,
                Year = year
            });

            return Ok(result);
        }
        [HttpPost("{id}/update-comparables",Name = "UpdateComparables")]
        public async Task<IActionResult> UpdateComparables(int id, [FromBody] List<int> comparableIds)
        {
            await Mediator.Send(new UpdateComparableDistrictRatesCommand
            {
                DistrictRateId = id,
                ComparableDistrictRateIds = comparableIds
            });

            return NoContent();
        }
        [HttpGet("{id}/comparables",Name = "GetComparablesByDistrictRateId")]
        public async Task<ActionResult<ComparableDistrictRateDetailDto>> GetComparablesByDistrictRateId(int id, [FromQuery] string context = "both")
        {
            var result = await Mediator.Send(new GetComparableDistrictRateDetailByIdQuery
            {
                Id = id,
                Context = context
            });

            return Ok(result);
        }
        [HttpGet("published",Name = "GetPublishedRates")]
        public async Task<ActionResult<List<PublishedRateSummaryDto>>> GetPublishedRates([FromQuery] string? filter = "all")
        {
            var result = await Mediator.Send(new GetPublishedRatesQuery
            {
                Filter = filter
            });

            return Ok(result);
        }
        [HttpGet("published/{districtId}",Name = "GetDistrictPublishedRates")]
        public async Task<ActionResult<PublishedRateSummaryDto>> GetDistrictPublishedRates(int districtId)
        {
            var all = await Mediator.Send(new GetPublishedRatesQuery());
            var record = all.FirstOrDefault(x => x.DistrictId == districtId);

            if (record == null)
                return NotFound("District not found or has no published rates.");

            return Ok(record);
        }
    }
}

