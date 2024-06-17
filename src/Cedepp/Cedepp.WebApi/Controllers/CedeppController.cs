using Cedepp.Application.Contracts.DTOs;
using Cedepp.Application.UseCases.Commands;
using Cedepp.Application.UseCases.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cedepp.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class CedeppController : ControllerBase
    {
        private readonly IMediator mediator;
        public CedeppController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("UploadProfilePhoto")]
        public async Task<ActionResult> UploadProfilePhoto([FromBody] ChangeProfilePhotoDTO data)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("UploadProfilePhoto called but user ID is missing.");
                return Unauthorized("User ID is required.");
            }

            try
            {
                Console.WriteLine("Attempting to upload profile photo for user");

                await mediator.Send(new ChangeProfilePhotoCommand(data, userId));

                Console.WriteLine("Profile photo updated successfully for user.");

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while uploading profile photo for user." + ex);
                return BadRequest("Something went wrong.");
            }
        }


        [HttpGet("GetUserProfile")]
        public async Task<ActionResult<GiveUserProfileFormDTO>> GetUserProfile()
        {

            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID is required.");
            }
            var result = await mediator.Send(new GetUserProfileQuery(userId));

            if (result == null)
            {
                return NotFound("There is no information.");
            }
            return Ok(result);
        }


        [HttpPost("FinishRegistration")]
        public async Task<ActionResult> FinishRegistration([FromBody] ChangeUserProfileDTO data)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User ID not found.");
            }

            var result = await mediator.Send(new FinishUserRegistrationCommand(data, userId));

            if (result == false)
            {
                return NotFound("Smth went wrong.");
            }
            return Ok();
        }

        [HttpPost("ChangeUserProfile")]
        public async Task<ActionResult> ChangeUserProfile([FromBody] ChangeUserProfileDTO data)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("User ID not found.");
            }

            var result = await mediator.Send(new ChangeUserProfileCommand(data, userId));

            if (result == false)
            {
                return NotFound("Smth went wrong.");
            }
            return Ok();
        }
    }
}
