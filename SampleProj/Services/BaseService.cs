using SampleProj.Repository;
using SampleProj.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleProj.Services
{
    public abstract class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class
    {
        private Repository<TEntity> _repository;
        public BaseService(RepositoryContext repositoryContext)
        {
            _repository = new Repository<TEntity>(repositoryContext);
        }

        /// <summary>
        /// generic async method for create any object
        /// </summary>
        /// <param name="createObject"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> CreateAsync(TEntity createObject)
        {
            _repository.Create(createObject);
            await _repository.SaveAsync();

            return createObject;
        }

        /// <summary>
        /// generic async method for update any object
        /// </summary>
        /// <param name="updateObject"></param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(TEntity updateObject)
        {
            _repository.Update(updateObject);
            await _repository.SaveAsync();
        }

        /// <summary>
        /// generic async method for delete any object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(int id)
        {
            var item = await this.GetByIdAsync(id);
            if (item != null)
            {
                _repository.Delete(item);
                await _repository.SaveAsync();
                return true;
            }
            return false;
        }

        public abstract Task<TEntity> GetByIdAsync(int id);

        /// <summary>
        /// generic async method for get all objects
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _repository.FindAllAsync();
        }

        /// <summary>
        /// generic async method for get all objects per page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllPerPageAsync(int page, int pageSize)
        {
            return await _repository.FindAllPerPageAsync(page, pageSize);
        }
    }
}
