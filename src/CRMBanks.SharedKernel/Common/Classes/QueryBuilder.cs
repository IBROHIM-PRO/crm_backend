using System.Linq.Expressions;

namespace CRMBanks.SharedKernel.Common.Classes;

public interface IQueryBuilder<T>
{
    IQueryBuilder<T> Where(Expression<Func<T, bool>> expression);

    IQueryBuilder<T> Chain(Func<IQueryable<T>, IQueryable<T>> expression);

    void Reset();
}

public class QueryBuilder<T> : IQueryBuilder<T>
{
    protected List<Expression<Func<T, bool>>> WheresExpressions = new();
    private List<Func<IQueryable<T>, IQueryable<T>>> _queryableGetters = new();

    public IQueryBuilder<T> Where(Expression<Func<T, bool>> expression)
    {
        WheresExpressions.Add(expression);
        return this;
    }

    public IQueryBuilder<T> Chain(Func<IQueryable<T>, IQueryable<T>> expression)
    {
        _queryableGetters.Add(expression);
        return this;
    }

    public void Reset()
    {
        WheresExpressions = new List<Expression<Func<T, bool>>>();
        _queryableGetters = new List<Func<IQueryable<T>, IQueryable<T>>>();
    }

    private IQueryable<T> ApplyWheres(IQueryable<T> q)
        => WheresExpressions.Aggregate(q, (current, exp) => current.Where(exp));

    private IQueryable<T> ApplyQueryable(IQueryable<T> q)
        => _queryableGetters.Aggregate(q, (current, getter) => getter(current));

    public IQueryable<T> GetQuery(IQueryable<T> q) => ApplyQueryable(ApplyWheres(q));
}