namespace Furniture.Service;

public interface IStatusService
{
	Task<List<StatusDto>> GetStatusAsync();
}
