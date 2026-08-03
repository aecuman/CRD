using CRD.Application.Templates;
using CRD.Application.Templates.Commands;
using CRD.Application.Templates.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CRD.API.Controllers
{
    [Route("api/rate-templates")]
    [ApiController]
    public class RateTemplateController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<List<RateTemplateViewModel>>> GetAll()
            => Ok(await Mediator.Send(new GetRateTemplatesQuery()));

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateRateTemplateCommand command)
            => Ok(await Mediator.Send(command));

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
            => Ok(await Mediator.Send(new DeleteRateTemplateCommand { Id = id }));

        [HttpPost("apply")]
        public async Task<ActionResult<ApplyRateTemplateResult>> Apply([FromBody] ApplyRateTemplateCommand command)
            => Ok(await Mediator.Send(command));
    }
}
