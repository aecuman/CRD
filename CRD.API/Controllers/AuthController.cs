using CRD.Application.Auth.Command;
using CRD.Application.Users.Commands;
using CRD.Application.Users.Queries;
using CRD.Domain.Identity;
using CRD.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace CRD.API.Controllers
{

    public class AuthController : BaseController
    {
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginDto), 200)]
        public async Task<IActionResult> Login([FromBody] UserLoginCommand command)
        {
            var (isFirstTimeLogin, resetToken) = await Mediator.Send(new CheckIfTemporaryPasswordCommand() { Email = command.Email, Password = command.Password });
            if (isFirstTimeLogin)
            {
                return BadRequest(new
                {
                    requiresReset = true,
                    message = "You must reset your password before logging in.",
                    token = resetToken,
                    email = command.Email
                });
            }
            else
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
        [HttpPost("forgot-password", Name = "ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command)
        {
            var response = await Mediator.Send(command);
            if (response)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("users/list/{page}/{pageSize}", Name = "GetUsers")]
        public async Task<ActionResult<List<UserViewModel>>> GetUsers(int page, int pageSize)
        {
            return await Mediator.Send(new GetUsersQuery() { Page = page, PageSize = pageSize });

        }
        [HttpPost("new-user", Name = "NewUser")]
        public async Task<ActionResult<bool>> CreateUser(RegisterUserCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPut("edit-user", Name = "EditUser")]
        public async Task<ActionResult<(bool, string[])>> EditUser(UpdateUserCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Item1)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpPost("update-password", Name = "UpdatePassword")]
        public async Task<ActionResult<(bool, string[])>> UpdatePassword(UpdatePasswordCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Item1)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }

        }
        [HttpPost("reset-password",Name = "ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand model)
        {
            var result = await Mediator.Send(model);
            if (result == null)
                return BadRequest(new { message = "Invalid user" });
            if (!result.Succeeded)
                return BadRequest(new { message = result.Errors.Select(x => x.Description).ToList()  /*"Invalid or expired token"*/ });
            return Ok(new { message = "Password reset successful" });
        }
    }
}
