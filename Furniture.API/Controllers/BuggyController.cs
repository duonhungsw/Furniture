using Furniture.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Furniture.API.Controllers;

public class BuggyController : BaseApiController
{
    [HttpGet("unauthorized")]
    public IActionResult GetUnauthorized()
    {
        return Unauthorized();
    }
    [HttpGet("badrequest")]
    public IActionResult GetBadRequest()
    {
        return BadRequest();
    }
    [HttpGet("notfound")]
    public IActionResult GetNotFound()
    {
        return NotFound();
    }
    [HttpGet("internalerror")]
    public IActionResult GetInternalError()
    {
        throw new Exception("This is a test exception");
    }
    [HttpPost("validationerror")]
    public IActionResult GetValidationError(BaseEntity entity)
    {
        return Ok();
    }
    [Authorize]
    [HttpGet("secret")]
    public IActionResult Secret()
    {
        var name = User.FindFirstValue(ClaimTypes.Name);
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Ok("Hello " + name + "with the id of " + id);
    }

}
