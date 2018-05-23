using System;
using System.Collections.Generic;
using LiteDB;

namespace Tethys.WebApi.DbModel.Repositories.LiteDb
{
    public sealed class UnitOfWorkLiteDb
    {
        #region Fields

        private readonly string _dbName;

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
                command(db);
            }
        }

        public IEnumerable<TQueryResult> Query<TQueryResult>(ISpecification<TQueryResult> spec)
        {
            using (var db = new LiteDatabase(_dbName))
            {
                return db.GetCollection<TQueryResult>().Find(spec.Criteria);
            }
        }
    }
}