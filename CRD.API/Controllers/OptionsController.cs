using CRD.Application.Common;
using CRD.Application.Options.Commands;
using CRD.Application.Options.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRD.API.Controllers
{
   
    public class OptionsController : BaseController
    {
        [HttpGet("s")]
        public ActionResult GetOptionStruct()
        {

            return Ok(OptionName.Value);
        }
        [HttpGet]
        public async Task<ActionResult<OptionsListViewModel>> GetOptions()
        {
            var list = await Mediator.Send(new GetOptionsListQuery());
            return Ok(list);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateOptionCommand optionCommand)
        {
            try
            {
                var command = await Mediator.Send(optionCommand);
                return Ok(command);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public async Task<ActionResult> Edit([FromBody] UpdateOptionCommand optionCommand)
        {
            try
            {
                await Mediator.Send(optionCommand);
                return Ok();
            }
            catch (Exception ex) {
            return BadRequest(ex.Message);
            }
        }
    }
}
