using Microsoft.AspNetCore.Mvc;
using MyMicroservice.Contracts.Requests;
using MyMicroservice.Contracts.Responses;

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
    [ProducesResponseType(typeof(SuccessResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Test([FromBody] TestJsonBodyRequest request)
    {
        return Ok(ApiResponse.Success($"Hello, {request.Name}!"));
    }

}

