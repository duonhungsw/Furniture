namespace Furniture.Service;

public class StatusService(
	IMapper _mapper,
	IStatusRepository _repository) : IStatusService
{

	public async Task<List<StatusDto>> GetStatusAsync()
	{
		var statuses =  await _repository.GetAllAsync();
		var result = _mapper.Map<List<StatusDto>>(statuses);
		return result;
	}
}
