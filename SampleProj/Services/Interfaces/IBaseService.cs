using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleProj.Services.Interfaces
{
    public interface IBaseService<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAllPerPageAsync(int page, int pageSize);
        Task<TEntity> GetByIdAsync(int id);
        Task<TEntity> CreateAsync(TEntity createObject);
        Task UpdateAsync(TEntity updateObject);
        Task<bool> DeleteAsync(int id);
    }
}
