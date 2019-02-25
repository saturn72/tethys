using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LiteDB;
using Tethys.Server.Models;
using Tethys.Server.Services;
using Tethys.Server.Services.HttpCalls;

namespace Tethys.Server.DbModel
{
    public class LiteDbRepository<TEntity> : IRepository<TEntity>
    {
        #region Fields

        private readonly string _dbName;

        #endregion

        #region CTOR

        public LiteDbRepository(string dbName)
        {
            _dbName = dbName;
        }

        #endregion

        #region Utilities
        public void Command(Action<LiteDatabase> command)
        {
            using (var db = new LiteDatabase(_dbName))
            {
                command(db);
            }
        }

        public IEnumerable<TQueryResult> Query<TQueryResult>(Func<TQueryResult, bool> filter)
        {
            IEnumerable<TQueryResult> results;
            using (var db = new LiteDatabase(_dbName))
            {
                var col = db.GetCollection<TQueryResult>();
                results = col.FindAll();
            }

            return filter == null ? results : results.Where(filter);
        }
        #endregion

        public void Create(IEnumerable<TEntity> entities)
        {
            Command(db => db.GetCollection<TEntity>().InsertBulk(entities));
        }

        public void Update(TEntity entity)
        {
            Command(db => db.GetCollection<TEntity>().Update(entity));
        }

        public IEnumerable<TEntity> GetAll(Func<TEntity, bool> filter = null)
        {
            return Query<TEntity>(filter);
        }

        public void DeleteAll()
        {
            Command(db => db.GetCollection<TEntity>().Delete(q => true));
        }

        public TEntity GetById(long id)
        {
            using (var db = new LiteDatabase(_dbName))
            {
                return db.GetCollection<TEntity>().FindById(id);
            }
        }


    }
}