using AutoMapper;
using Furniture.Core.Models;
using Furniture.Infrastructure.Repositories.Interfaces;

namespace Furniture.API.Controllers;
public class ProductController(IUnitOfWork unit, IMapper mapper) : BaseApiController
{
	private bool ProductExists(Guid id)
	{
		return unit.Repository<Product>().Exist(id);
	}
}
