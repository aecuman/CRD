using CRD.Application.Auth.Command;
using CRD.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRD.API.Controllers
{

    public class AuthController : BaseController
    {
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginDto), 200)]
        public async Task<IActionResult> Login([FromBody] UserLoginCommand command)
        {
            var response = await Mediator.Send(command);
            if (response.Succeed)
            {
                return Ok(response);
            }
            else
            {
                var obj = new
                {
                    message = response.Message
                };
                return BadRequest(obj);
            }

        }
    }
}
