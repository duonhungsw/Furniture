namespace Furniture.Web.Services;

public interface IStatusApi
{
	[Get("/status")]
	Task<List<StatusDto>> GetStatuses();
}
