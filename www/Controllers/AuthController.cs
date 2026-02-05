using Microsoft.AspNetCore.Mvc;
using MyMicroservice.Application.Services;
using MyMicroservice.Contracts.Requests;
using MyMicroservice.Contracts.Responses;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    /// <summary>
    /// Log incident endpoint
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("register")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterJsonRequest request)
    {
        Console.WriteLine($"Received register request: {request}");
        return Ok(
            ApiResponse<string>.Ok(
                await _userService.RegisterAsync(request)
            )
        );
    }

}

