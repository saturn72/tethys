using System.Collections.Generic;
using System.Linq;

namespace Tethys.Server.DbModel.Repositories.LiteDb
{
    public static class UnitOfWorkLiteDbExtensions
    {

        public static IEnumerable<TQueryResult> Query<TQueryResult>(this UnitOfWorkLiteDb unitOfWorkLiteDb, ISpecification<TQueryResult> spec)
        {
            return unitOfWorkLiteDb.Query(spec.Criteria);
        }

        public static IEnumerable<TDomainModel> GetAll<TDomainModel>(this UnitOfWorkLiteDb unitOfWorkLiteDb)
        {
            return unitOfWorkLiteDb.Query<TDomainModel>(db => true);
        }

        public static void Create<TDomainModel>(this UnitOfWorkLiteDb unitOfWorkLiteDb,
            IEnumerable<TDomainModel> models)
        {
            unitOfWorkLiteDb.Command(db =>
            {
                var col = db.GetCollection<TDomainModel>();
                col.InsertBulk(models);
            });
        }

        public static void Insert<TDomainModel>(this UnitOfWorkLiteDb unitOfWorkLiteDb, TDomainModel model)
        {
            unitOfWorkLiteDb.Command(db =>
            {
                var col = db.GetCollection<TDomainModel>();
                col.Insert(model);
            });
        }

        public static void Update<TDomainModel>(this UnitOfWorkLiteDb unitOfWorkLiteDb, TDomainModel model)
        {
            unitOfWorkLiteDb.Command(db =>
                {
                    var col = db.GetCollection<TDomainModel>();
                    col.Update(model);
                });
        }

        /*    public static void DeleteById<TDomainModel>(this UnitOfWorkLiteDb UnitOfWorkLiteDb, long id)
                where TDomainModel 
            {
                UnitOfWorkLiteDb.Command(db =>
                {
                    var col = GetCollection<TDomainModel>(db);
                    col.Delete(id);
                });
            }

            public static IEnumerable<TDomainModel> GetAll<TDomainModel>(this UnitOfWorkLiteDb UnitOfWorkLiteDb)
                where TDomainModel
            {
                return UnitOfWorkLiteDb.Query(db => GetCollection<TDomainModel>(db).FindAll().ToArray());
            }

            public static LiteCollection<TDomainModel> GetCollection<TDomainModel>(this LiteDatabase db)
                where TDomainModel
            {
                var key = LiteDbEntitiesNames.EntityToDocumentName[typeof(TDomainModel)];
                return db.GetCollection<TDomainModel>(key);
            }

            public static TDomainModel GetById<TDomainModel>(this UnitOfWorkLiteDb UnitOfWorkLiteDb, long id)
                where TDomainModel 
            {
                return UnitOfWorkLiteDb.Query(db => GetCollection<TDomainModel>(db).FindById(id));
            }
            public static IEnumerable<TDomainModel> GetBy<TDomainModel>(this UnitOfWorkLiteDb UnitOfWorkLiteDb, Func<TDomainModel, bool> query)
                where TDomainModel 
            {
                var all = UnitOfWorkLiteDb.GetAll<TDomainModel>();
                return all.Where(query).ToArray();
            }





            private static void CopyAllProperties<TDomainModel>(TDomainModel source, */
    }
}