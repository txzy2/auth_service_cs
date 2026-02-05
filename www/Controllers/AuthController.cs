using Microsoft.AspNetCore.Mvc;
using MyMicroservice.Application.Common.Errors;
using MyMicroservice.Application.Common.Exceptions;
using MyMicroservice.Application.Services;
using MyMicroservice.Contracts.Requests;
using MyMicroservice.Contracts.Responses;
using MyMicroservice.Domain.Entities;

namespace MyMicroservice.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(SuccessResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterJsonRequest request)
    {
        var result = await _userService.RegisterAsync(request);
        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// Login endpoint
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(SuccessResponse<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginJsonRequest request)
    {
        try
        {
            var user = await _userService.LoginAsync(request);
            return Ok(ApiResponse.Success(user));
        }
        catch (ApiException ex)
        {
            return BadRequest(ApiResponse.Error(ex.Message, ex.StatusCode));
        }
    }
}