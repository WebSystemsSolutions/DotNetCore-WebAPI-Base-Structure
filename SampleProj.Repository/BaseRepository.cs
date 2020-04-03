using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SampleProj.Repository
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected DbContext EntitiesContext { get; set; }

        public BaseRepository(DbContext entitiesContext)
        {
            this.EntitiesContext = entitiesContext;
        }

        public void Create(TEntity entity)
        {
            this.EntitiesContext.Set<TEntity>().Add(entity);
        }

        public void CreateMany(IEnumerable<TEntity> entity)
        {
            this.EntitiesContext.Set<TEntity>().AddRange(entity);
        }

        public async Task CreateManyAsync(IEnumerable<TEntity> entity)
        {
            await this.EntitiesContext.Set<TEntity>().AddRangeAsync(entity);
        }

        public void Update(TEntity entity)
        {
            this.EntitiesContext.Set<TEntity>().Update(entity);
        }

        public void UpdateMany(IEnumerable<TEntity> entity)
        {
            this.EntitiesContext.Set<TEntity>().UpdateRange(entity);
        }

        public async Task UpdateEntryAsync(TEntity entity, params Expression<Func<TEntity, object>>[] properties)
        {
            var entry = EntitiesContext.Entry(entity);

            EntitiesContext.Set<TEntity>().Attach(entity);

            foreach (var property in properties)
                entry.Property(property).IsModified = true;

            await EntitiesContext.SaveChangesAsync();
        }

        public bool Delete(TEntity entity)
        {
            this.EntitiesContext.Set<TEntity>().Remove(entity);
            return true;
        }

        public bool DeleteMany(IEnumerable<TEntity> entity)
        {
            this.EntitiesContext.Set<TEntity>().RemoveRange(entity);
            return true;
        }

        public async Task SaveAsync()
        {
            await this.EntitiesContext.SaveChangesAsync();
        }

        /// <summary>
        /// Find all objects and can include related tables. Includes are not required
        /// </summary>
        /// <param name="includes"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> FindAllAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            var query = this.EntitiesContext.Set<TEntity>().AsQueryable<TEntity>();

            if (includes != null)
                return await includes.Aggregate(query, (current, include) =>
                {
                    if (!String.IsNullOrEmpty(include.AsPath()))
                        return current.Include(include.AsPath());
                    else
                        return current;
                }).ToListAsync<TEntity>();
            else
                return await this.EntitiesContext.Set<TEntity>().ToListAsync();
        }

        /// <summary>
        /// Find all objects for page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> FindAllPerPageAsync(int page, int pageSize)
        {
            return await this.EntitiesContext.Set<TEntity>().Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        /// <summary>
        /// Sync method to find objects by expression and include related tables. Expression and includes are not required
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="includes"></param>
        /// <returns>Query</returns>
        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = null;

            if (expression != null)
                query = EntitiesContext.Set<TEntity>().Where(expression);
            else
                query = EntitiesContext.Set<TEntity>();

            if (includes != null)
                return includes.Aggregate(query, (current, include) =>
                {
                    if (!String.IsNullOrEmpty(include.AsPath()))
                        return current.Include(include.AsPath());
                    else
                        return current;
                });
            else
                return query;
        }

        /// <summary>
        /// Method to find few expression
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="includes"></param>
        /// <returns>Query</returns>
        public IQueryable<TEntity> FindByСhain(IQueryable<TEntity> querysource, Expression<Func<TEntity, bool>> expression)
        {
            if (querysource == null)
                querysource = EntitiesContext.Set<TEntity>();

            if (expression != null)
                querysource = querysource.AddExpressionСhain(expression);

            return querysource;
        }

        /// <summary>
        /// Async method to find objects by expression and include related tables. Expression and includes are not required
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="includes"></param>
        /// <returns>List<TEntity></returns>
        public async Task<IEnumerable<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = null;

            if (expression != null)
                query = EntitiesContext.Set<TEntity>().Where(expression);
            else
                query = EntitiesContext.Set<TEntity>();

            if (includes != null)
                return await includes.Aggregate(query, (current, include) =>
                {
                    if (!String.IsNullOrEmpty(include.AsPath()))
                        return current.Include(include.AsPath());
                    else
                        return current;
                })
                    .AsNoTracking()
                    .ToListAsync();
            else
                return await query
                    .AsNoTracking()
                    .ToListAsync();
        }

        /// <summary>
        /// Find objects by expression. Expression is required
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await this.EntitiesContext.Set<TEntity>().Where(expression).AsNoTracking().ToListAsync();
        }
    }
}
