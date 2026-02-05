using Microsoft.AspNetCore.Mvc;
using MyMicroservice.Contracts.Requests;
using MyMicroservice.Contracts.Responses;
using Swashbuckle.AspNetCore.Annotations;

[ApiController]
[Route("api/v1/test")]
public class TestController : ControllerBase
{

    /// <summary>
    /// Test endpoint
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Test endpoint",
        Description = "A simple test endpoint to verify API functionality. Accepts a name and returns a greeting message.",
        OperationId = "Test",
        Tags = new[] { "Test" }
    )]
    [SwaggerResponse(200, "Returns a greeting message", typeof(SuccessResponse<string>))]
    public async Task<IActionResult> Test([FromBody] TestJsonBodyRequest request)
    {
        return Ok(ApiResponse.Success($"Hello, {request.Name}!"));
    }

}

