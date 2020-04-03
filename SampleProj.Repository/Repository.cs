using System;
using System.Collections.Generic;
using System.Text;

namespace SampleProj.Repository
{
    public class Repository<TEntity> : BaseRepository<TEntity> where TEntity : class
    {
        public Repository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {

        }
    }
}
