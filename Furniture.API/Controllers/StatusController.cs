namespace Furniture.API.Controllers;

[Route("status")]
public class StatusController(IStatusService _statusService) : BaseApiController
{
	[HttpGet]
	public async Task<List<StatusDto>> GetStatuses()
	{
		var result = await _statusService.GetStatusAsync();
		return result;
	}
}
