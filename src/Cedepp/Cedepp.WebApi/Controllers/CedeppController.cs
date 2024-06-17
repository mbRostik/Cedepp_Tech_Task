using Cedepp.Application.Contracts.DTOs;
using Cedepp.Application.UseCases.Commands;
using Cedepp.Application.UseCases.Queries;
using Cedepp.Application.Validators;
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
        private readonly IMediator _mediator;
        private readonly Serilog.ILogger _logger;

        public CedeppController(IMediator mediator, Serilog.ILogger logger)
        {
            this._mediator = mediator;
            _logger = logger;
        }

        [HttpPost("UploadProfilePhoto")]
        public async Task<ActionResult<GiveUserProfileFormDTO>> UploadProfilePhoto([FromBody] ChangeProfilePhotoDTO data)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.Warning("UploadProfilePhoto called but user ID is missing.");
                return Unauthorized("User ID is required.");
            }

            try
            {
                _logger.Information("Attempting to upload profile photo for user with ID: {UserId}", userId);

                await _mediator.Send(new ChangeProfilePhotoCommand(data, userId));

                _logger.Information("Profile photo updated successfully for user with ID: {UserId}", userId);
                var result = await _mediator.Send(new GetUserProfileQuery(userId));

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while uploading profile photo for user with ID: {UserId}", userId);
                return BadRequest("Something went wrong.");
            }
        }

        [HttpGet("GetUserProfile")]
        public async Task<ActionResult<GiveUserProfileFormDTO>> GetUserProfile()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.Warning("GetUserProfile called but user ID is missing.");
                return Unauthorized("User ID is required.");
            }

            var result = await _mediator.Send(new GetUserProfileQuery(userId));

            if (result == null)
            {
                _logger.Information("No information found for user with ID: {UserId}", userId);
                return NotFound("There is no information.");
            }

            _logger.Information("User profile retrieved successfully for user with ID: {UserId}", userId);
            return Ok(result);
        }


        [HttpPost("FinishRegistration")]
        public async Task<ActionResult> FinishRegistration([FromBody] ChangeUserProfileDTO data)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.Warning("FinishRegistration called but user ID is missing.");
                return NotFound("User ID not found.");
            }

            var validator = new ChangeUserProfileDTOValidator();
            var validationResult = validator.Validate(data);

            if (!validationResult.IsValid)
            {
                _logger.Warning("FinishRegistration validation failed for user ID: {UserId}", userId);
                return BadRequest(validationResult.Errors.Select(e => new { error = e.ErrorMessage }));
            }

            var result = await _mediator.Send(new FinishUserRegistrationCommand(data, userId));

            if (!result)
            {
                _logger.Error("FinishRegistration failed for user ID: {UserId}", userId);
                return NotFound("Something went wrong.");
            }

            _logger.Information("FinishRegistration succeeded for user ID: {UserId}", userId);
            return Ok();
        }

        [HttpPost("ChangeUserProfile")]
        public async Task<ActionResult> ChangeUserProfile([FromBody] ChangeUserProfileDTO data)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.Warning("ChangeUserProfile called but user ID is missing.");
                return NotFound("User ID not found.");
            }

            var result = await _mediator.Send(new ChangeUserProfileCommand(data, userId));

            if (!result)
            {
                _logger.Error("ChangeUserProfile failed for user ID: {UserId}", userId);
                return NotFound("Something went wrong.");
            }

            _logger.Information("ChangeUserProfile succeeded for user ID: {UserId}", userId);
            return Ok();
        }

    }
}
