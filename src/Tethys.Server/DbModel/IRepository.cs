using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tethys.Server.Models;

namespace Tethys.Server.DbModel
{
    public interface IRepository<TEntity>
    {
        void Create(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        IEnumerable<TEntity> GetAll(Func<TEntity, bool> filter = null);
        void DeleteAll();
        TEntity GetById(long id);
    }
}
