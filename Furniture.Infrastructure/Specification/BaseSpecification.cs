using Furniture.Core.Interfaces;
using System.Linq.Expressions;

namespace Furniture.Infrastructure.Specification;

public class BaseSpecification<T>(Expression<Func<T, bool>>? criteria) : ISpecification<T>
{
    protected BaseSpecification() : this(null) { }

    public Expression<Func<T, bool>>? Criteria => criteria;

    public Expression<Func<T, object>>? OrderBy { get; private set; }

    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    public List<Expression<Func<T, object>>> Includes { get; } = [];

    public List<string> IncludeStrings { get; } = [];

    public bool IsDistinct { get; private set; }

    public int Take { get; private set; }

    public int Skip { get; private set; }

    public bool IsPagingEnabled { get; private set; }
    public IQueryable<T> ApplyCriteria(IQueryable<T> query)
    {
        if (Criteria != null)
        {
            query = query.Where(Criteria);
        }
        return query;
    } 

    protected void AppPaging(int take,  int skip) 
    { 
        Take = take;
        Skip = skip;
        IsPagingEnabled = true;
    }

    protected void AddOrderBy(Expression<Func<T, object>>? orderBy)
    {
        OrderBy = orderBy;
    }
    protected void AddOrderByDescending(Expression<Func<T, object>>? orderByDescending)
    {
        OrderByDescending = orderByDescending;
    }

    protected void Distinct()
    {
        IsDistinct = true;
    }
}
public class BaseSpecification<T, TResult> : BaseSpecification<T>, ISpecification<T, TResult>
{
    public BaseSpecification(Expression<Func<T, bool>> criteria) : base(criteria) { }

    protected BaseSpecification() : this(_ => true) { }

    public Expression<Func<T, TResult>>? Select { get; private set; }
    protected void AddSelect(Expression<Func<T, TResult>> selectExpression)
    {
        Select = selectExpression;
    }
}

