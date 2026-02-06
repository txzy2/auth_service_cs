using Microsoft.AspNetCore.Mvc;
using MyMicroservice.Application.Common.Exceptions;
using MyMicroservice.Application.Services;
using MyMicroservice.Contracts.Requests;
using MyMicroservice.Contracts.Responses;
using MyMicroservice.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace MyMicroservice.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController(IUserService userService, ILogger<AuthController> logger) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly ILogger<AuthController> _logger = logger;

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">User registration data</param>
    /// <returns>Registration confirmation message</returns>
    [HttpPost("register")]
    [SwaggerOperation(
        Summary = "Register a new user",
        Description = "Creates a new user account with the provided credentials. Email must be unique.",
        OperationId = "Register",
        Tags = new[] { "Authentication" }
    )]
    [SwaggerResponse(200, "User successfully registered", typeof(SuccessResponse<string>))]
    [SwaggerResponse(400, "Invalid input data (validation error)", typeof(ErrorResponse))]
    [SwaggerResponse(409, "User with this email already exists", typeof(ErrorResponse))]
    public async Task<IActionResult> Register([FromBody] RegisterJsonRequest request)
    {
        _logger.LogInformation($"AuthController try REGISTER {request.Email}");
        var result = await _userService.RegisterAsync(request);
        _logger.LogInformation("User registered successfully: {Email}", request.Email);
        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// Authenticate user
    /// </summary>
    /// <param name="request">User login credentials</param>
    /// <returns>User data if authentication successful</returns>
    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "Authenticate user",
        Description = "Validates user credentials and returns user information. Use this endpoint to log in users.",
        OperationId = "Login",
        Tags = new[] { "Authentication" }
    )]
    [SwaggerResponse(200, "Successfully authenticated. Returns user data.", typeof(SuccessResponse<User>))]
    [SwaggerResponse(400, "Invalid credentials. Password does not match.", typeof(ErrorResponse))]
    [SwaggerResponse(404, "User with provided email not found", typeof(ErrorResponse))]
    public async Task<IActionResult> Login([FromBody] LoginJsonRequest request)
    {
        _logger.LogInformation($"AuthController try LOGIN {request.Login}");
        try
        {
            return Ok(ApiResponse.Success(await _userService.LoginAsync(request)));
        }
        catch (ApiException ex)
        {
            _logger.LogWarning("Login failed for {Login}: {Message}", request.Login, ex.Message);
            return BadRequest(ApiResponse.Error(ex.Message, ex.StatusCode));
        }
    }
}