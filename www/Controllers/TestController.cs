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
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Test([FromBody] TestJsonBodyRequest request)
    {
        if (request.Name == "error")
        {
            return BadRequest(ApiResponse<string>.Error("Some ErrorMessage"));
        }

        return Ok(ApiResponse<string>.Ok($"Hello, {request.Name}!"));
    }

}

