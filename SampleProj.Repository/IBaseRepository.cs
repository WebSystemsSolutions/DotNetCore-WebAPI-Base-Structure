using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SampleProj.Repository
{
    public interface IBaseRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> FindAllAsync(params Expression<Func<TEntity, object>>[] includes);
        Task<IEnumerable<TEntity>> FindAllPerPageAsync(int page, int pageSize);
        Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression);
        IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes);
        Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes);
        void Create(TEntity entity);
        void CreateMany(IEnumerable<TEntity> entity);
        void Update(TEntity entity);
        void UpdateMany(IEnumerable<TEntity> entity);
        Task UpdateEntryAsync(TEntity entity, params Expression<Func<TEntity, object>>[] properties);
        bool Delete(TEntity entity);
        bool DeleteMany(IEnumerable<TEntity> entity);
        Task SaveAsync();
    }
}
