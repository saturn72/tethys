using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LiteDB;

namespace Tethys.Server.DbModel.Repositories.LiteDb
{
    public sealed class UnitOfWorkLiteDb
    {
        #region Fields

        private readonly string _dbName;
        private readonly object _lockObject = new object();

        #endregion

        #region ctor

        public UnitOfWorkLiteDb(string dbName)
        {
            _dbName = dbName;
        }

        #endregion

        public void Command(Action<LiteDatabase> command)
        {
            using (var db = new LiteDatabase(_dbName))
            {
                lock (_lockObject)
                {
                    command(db);
                }
            }
        }

        public IEnumerable<TQueryResult> Query<TQueryResult>(Expression<Func<TQueryResult, bool>> criteria)
        {
            using (var db = new LiteDatabase(_dbName))
            {
                return db.GetCollection<TQueryResult>().Find(criteria);
            }
        }
    }
}