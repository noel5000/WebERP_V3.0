using WebERPV3.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
  public interface  IBase
    {
    }

    public interface IBase<T> : IBase
      where T : class, IBaseEntity, new()
    {
        Task<Result<T>> Add(T entity);
        Task AddRange(IEnumerable<T> entities);

        Task<Result<T>> Remove(T entity);

        Task<Result<T>> Remove(long id);
        Task RemoveRange(IEnumerable<T> entities);

        Task<Result<T>> Update(T entity);

        Task<Result<T>> GetAll(string sortExpression = null);

        IPagedList<T> GetPaged(int startRowIndex, int pageSize, string sortExpression = null);

        Task<Result<T>> GetAll(Func<IQueryable<T>, IQueryable<T>> transform, Expression<Func<T, bool>> filter = null, string sortExpression = null);

        Task<IEnumerable<TResult>> GetAll<TResult>(Func<IQueryable<T>, IQueryable<TResult>> transform, Expression<Func<T, bool>> filter = null, string sortExpression = null);

        Task<int> GetCount<TResult>(Func<IQueryable<T>, IQueryable<TResult>> transform, Expression<Func<T, bool>> filter = null);

        IPagedList<T> GetPaged(Func<IQueryable<T>, IQueryable<T>> transform, Expression<Func<T, bool>> filter = null, int startRowIndex = -1, int pageSize = -1, string sortExpression = null);

        IPagedList<TResult> GetPaged<TResult>(Func<IQueryable<T>, IQueryable<TResult>> transform, Expression<Func<T, bool>> filter = null, int startRowIndex = -1, int pageSize = -1, string sortExpression = null);

        Task<Result<T>> Get(long id);

       Task<Result<T>> Get(string id);

       Task<TResult> Get<TResult>(Func<IQueryable<T>, IQueryable<TResult>> transform, Expression<Func<T, bool>> filter = null, string sortExpression = null);

        Task<bool> Exists(long id);

        Task<bool> Exists(Func<IQueryable<T>, IQueryable<T>> query, Expression<Func<T, bool>> filter = null);
     
    }
}
