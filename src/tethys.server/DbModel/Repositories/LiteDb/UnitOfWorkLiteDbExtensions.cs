using System;
using System.Collections.Generic;
using System.Linq;
using Tethys.Server.Models;
using Tethys.Server.Services;

namespace Tethys.Server.DbModel.Repositories.LiteDb
{
    public static class UnitOfWorkLiteDbExtensions
    {

        public static IEnumerable<TQueryResult> Query<TQueryResult>(this UnitOfWorkLiteDb unitOfWorkLiteDb, ISpecification<TQueryResult> spec)
        {
            return unitOfWorkLiteDb.Query(spec.Criteria);
        }

        public static IEnumerable<TDomainModel> GetAll<TDomainModel>(this UnitOfWorkLiteDb unitOfWorkLiteDb)
       where TDomainModel : DomainModelBase
        {
            return unitOfWorkLiteDb.Query<TDomainModel>(db => true);
        }

        public static TDomainModel GetById<TDomainModel>(this UnitOfWorkLiteDb unitOfWorkLiteDb, long id)
        where TDomainModel : DomainModelBase
        {
            return unitOfWorkLiteDb.GetById<TDomainModel>(id);
        }

        public static void Create<TDomainModel>(this UnitOfWorkLiteDb unitOfWorkLiteDb,
            IEnumerable<TDomainModel> models)
      where TDomainModel : DomainModelBase
        {
            unitOfWorkLiteDb.Command(db =>
            {
                var col = db.GetCollection<TDomainModel>();
                col.InsertBulk(models);
            });
        }

        public static void Create<TDomainModel>(this UnitOfWorkLiteDb unitOfWorkLiteDb, TDomainModel model)
    where TDomainModel : DomainModelBase
        {
            unitOfWorkLiteDb.Command(db =>
            {
                var col = db.GetCollection<TDomainModel>();
                col.Insert(model);
            });
        }

        public static void Update<TDomainModel>(this UnitOfWorkLiteDb unitOfWorkLiteDb, TDomainModel model)
        where TDomainModel : DomainModelBase
        {
            unitOfWorkLiteDb.Command(db => db.GetCollection<TDomainModel>().Update(model));
        }
    }
}