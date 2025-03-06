using CRD.Application.Districts.Commands;
using CRD.Application.Districts.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRD.API.Controllers
{
    [Route("api/district-rates")]
    [ApiController]
    public class DistrictRatesController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await Mediator.Send(new GetDistrictRatesQuery()));

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
    }
}
