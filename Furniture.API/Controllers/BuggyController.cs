using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Furniture.API.Controllers;
[Route("buggy")]
public class BuggyController : BaseApiController
{
	[Authorize]
	[HttpGet("unauthorized")]
	public IActionResult GetUnauthorized()
	{
		throw new UnauthorizedException();
	}
	[HttpGet("badrequest")]
	public IActionResult GetBadRequest()
	{
		return BadRequest();
	}
	[HttpGet("notfound")]
	public IActionResult GetNotFound()
	{
		throw new NotFoundException("This item was not found in the database");
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
