using Furniture.Core.Interfaces;
using Furniture.Core.Models;

namespace Furniture.Infrastructure.Specification;

public class SpecificationEvaluator<T> where T : BaseEntity
{
    public static IQueryable<T> GetQuery(IQueryable<T> query, ISpecification<T> spec)
    {
        if (spec != null)
        {
            query = query.Where(spec.Criteria!);
        }

        if(spec!.OrderBy != null)
        {
            query = query.OrderBy(spec.OrderBy);
        }

        if(spec.OrderByDescending != null)
        {
            query = query.OrderByDescending(spec.OrderByDescending);
        }
        if(spec.Skip != 0 && spec.Take != 0)
        {
            query = query.Skip(spec.Skip).Take(spec.Take);
        }
        return query;
    }
    public static IQueryable<TResult> GetQuery<TSpec, TResult>(IQueryable<T> query,
        ISpecification<T, TResult> spec)
    {
        if (spec.Criteria != null)
        {
            query = query.Where(spec.Criteria); // x => x.Brand == brand
        }
        var selectQuery = query as IQueryable<TResult>;

        return selectQuery ?? query.Cast<TResult>();

    }
}
