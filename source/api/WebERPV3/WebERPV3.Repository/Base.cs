using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using WebERPV3.Entities;
using System.Threading.Tasks;

namespace WebERPV3.Repository
{
    public abstract class Base<T, U> : IBase<T>
          where T : class, IBaseEntity, new()
          where U : DbContext
    {
        protected readonly U _Context;
        private readonly DbSet<T> _DbSet;

        public Base(U context)
        {
            _Context = context;
            _DbSet = _Context.Set<T>();
        }

        public virtual async  Task<Result<T>> Add(T entity)
        {
            try
            {
                var translation = entity as IEntityTranslate;
                if (translation != null) 
                {
                  entity.TranslationData=  TranslateUtility.SaveTranslation(entity, translation.TranslationData);
                   
                }
            await    _Context.Set<T>().AddAsync(entity);

                await _Context.SaveChangesAsync();

                return new Result<T>(entity.Id, 0, "OK", new List<T>() { entity }); ;
            }

            catch (Exception ex)
            {
                return new Result<T>(-1, -1, ex.Message, null, ex);
            }
          
        }

        public virtual async Task AddRange(IEnumerable<T> entities)
        {
            await _Context.Set<T>().AddRangeAsync(entities);

            await _Context.SaveChangesAsync();
        }
        public virtual async Task RemoveRange(IEnumerable<T> entities)
        {
            _Context.Set<T>().RemoveRange(entities);

            await _Context.SaveChangesAsync();
        }



        public virtual async Task<Result<T>> Remove(T entity)
        {
            try
            {
                _DbSet.Attach(entity);
                _Context.Entry<T>(entity).State = EntityState.Deleted;

                await _Context.SaveChangesAsync();
                return new Result<T>(0, 0, "OK");
            }

            catch (Exception ex) 
            {
                return new Result<T>(-1, -1, ex.Message, null, ex);
            }
          


        }

        public virtual async  Task<Result<T>> Remove(long id)
        {
            try
            {
                T entity = _DbSet.Find(id);

                _Context.Entry<T>(entity).State = EntityState.Deleted;
                await _Context.SaveChangesAsync();
                return new Result<T>(0, 0, "OK");
            }

            catch (Exception ex)
            {
                return new Result<T>(-1, -1, ex.Message, null, ex);
            }

            
        }

        public virtual async  Task<Result<T>> Update(T entity)
        {
            try
            {
                var dbEntity = await _DbSet.FindAsync(entity.Id);
                _Context.Entry<T>(dbEntity).State = EntityState.Detached;
                var translation = dbEntity as IEntityTranslate;
                if (translation != null)
                {
                    entity.TranslationData = TranslateUtility.SaveTranslation(entity, translation.TranslationData);
                   
                }
                _DbSet.Attach(entity);
                _Context.Entry<T>(entity).State = EntityState.Modified;

                await _Context.SaveChangesAsync();

                return new Result<T>(entity.Id, 0, "OK", new List<T>() { entity });
            }

            catch (Exception ex)
            {
                return new Result<T>(-1, -1, ex.Message, null, ex);
            }
            
        }

        public virtual async  Task<Result<T>> GetAll(string sortExpression = null)
        {
            return new Result<T>(0,0,"OK", await _DbSet.AsNoTracking().OrderBy(sortExpression).ToListAsync());
        }

        public virtual  IPagedList<T> GetPaged(int startRowIndex, int pageSize, string sortExpression = null)
        {
            return new PagedList<T>(_DbSet.AsNoTracking().Where(x=>x.IsDeleted==false), startRowIndex, pageSize);
        }


       

        public virtual async  Task<Result<T>> GetAll(Func<IQueryable<T>, IQueryable<T>> transform, Expression<Func<T, bool>> filter = null, string sortExpression = null)
        {

            var query = filter == null ? _DbSet.AsNoTracking().OrderBy(sortExpression) : _DbSet.AsNoTracking().Where(filter).OrderBy(sortExpression);

            var notSortedResults = transform(query);

            var sortedResults = sortExpression == null ? notSortedResults : notSortedResults.OrderBy(sortExpression);

            return new Result<T>(0,0,"OK", await sortedResults.ToListAsync());
        }

        public virtual async Task<IEnumerable<TResult>> GetAll<TResult>(Func<IQueryable<T>, IQueryable<TResult>> transform, Expression<Func<T, bool>> filter = null, string sortExpression = null)
        {
            var query = filter == null ? _DbSet.AsNoTracking().OrderBy(sortExpression) : _DbSet.AsNoTracking().Where(filter).OrderBy(sortExpression);

            var notSortedResults = transform(query);

            var sortedResults = sortExpression == null ? notSortedResults : notSortedResults.OrderBy(sortExpression);

            return await sortedResults.ToListAsync();
        }

        public async Task<int> GetCount<TResult>(Func<IQueryable<T>, IQueryable<TResult>> transform, Expression<Func<T, bool>> filter = null)
        {
            var query = filter == null ? _DbSet.AsNoTracking() : _DbSet.AsNoTracking().Where(filter);

            return await transform(query).CountAsync();
        }

        public virtual IPagedList<T> GetPaged(Func<IQueryable<T>, IQueryable<T>> transform, Expression<Func<T, bool>> filter = null, int startRowIndex = -1, int pageSize = -1, string sortExpression = null)
        {
            var query = filter == null ? _DbSet.AsNoTracking().OrderBy(sortExpression) : _DbSet.AsNoTracking().Where(filter).OrderBy(sortExpression);

            var notSortedResults = transform(query);

            var sortedResults = sortExpression == null ? notSortedResults : notSortedResults.OrderBy(sortExpression);

            return new PagedList<T>(sortedResults, startRowIndex, pageSize);
        }

        public virtual  IPagedList<TResult> GetPaged<TResult>(Func<IQueryable<T>, IQueryable<TResult>> transform, Expression<Func<T, bool>> filter = null, int startRowIndex = -1, int pageSize = -1, string sortExpression = null)
        {
            var query = filter == null ? _DbSet.AsNoTracking() : _DbSet.AsNoTracking().Where(filter);

            var notSortedResults = transform(query);

            var sortedResults = sortExpression == null ? notSortedResults : notSortedResults.OrderBy(sortExpression);

            return new PagedList<TResult>(sortedResults, startRowIndex, pageSize);
        }

        public virtual async  Task<Result<T>> Get(long id)
        {
            try
            {
                var entity = await _DbSet.FindAsync(id);
                _Context.Entry<T>(entity).State = EntityState.Detached;
                //TranslateUtility.Translate(entity, entity.TranslationData);
                return new Result<T>(0,0,"OK",new List<T>() { entity });
            }

            catch (Exception ex)
            {
                return new Result<T>(-1, -1, ex.Message, null, ex);
            }
          
        }

        public virtual async  Task<Result<T>> Get(string id)
        {
            try
            {
                var entity = await _DbSet.FindAsync(id);
                _Context.Entry<T>(entity).State = EntityState.Detached;
                return new Result<T>(0, 0, "OK", new List<T>() { entity });
            }

            catch (Exception ex)
            {
                return new Result<T>(-1, -1, ex.Message, null, ex);
            }
          
        }

        public virtual async  Task<TResult> Get<TResult>(Func<IQueryable<T>, IQueryable<TResult>> transform, Expression<Func<T, bool>> filter = null, string sortExpression = null)
        {
            var query = filter == null ? _DbSet.AsNoTracking() : _DbSet.AsNoTracking().Where(filter);

            var notSortedResults = transform(query);

            var sortedResults = sortExpression == null ? notSortedResults : notSortedResults.OrderBy(sortExpression);

            return await sortedResults.FirstOrDefaultAsync();
        }
        public virtual async  Task<bool> Exists(long id)
        {
            return await _DbSet.AnyAsync(x=>x.Id==id && x.IsDeleted ==false);
        }

        public virtual async  Task<bool> Exists(Func<IQueryable<T>, IQueryable<T>> selector, Expression<Func<T, bool>> filter = null)
        {
            var query = filter == null ? _DbSet.AsNoTracking() : _DbSet.AsNoTracking().Where(filter);

            var result = selector(query);

            return await result.AnyAsync();
        }

       
    }
}
