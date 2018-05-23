namespace Tethys.WebApi.DbModel.Repositories.LiteDb
{
    public static class UnitOfWorkLiteDbExtensions
    {
        public static void Insert<TDomainModel>(this UnitOfWorkLiteDb unitOfWorkLiteDb, TDomainModel model)
        {

            unitOfWorkLiteDb.Command(db =>
            {
                var col = db.GetCollection<TDomainModel>();
                col.Insert(model);
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



            public static void Update<TDomainModel>(this UnitOfWorkLiteDb UnitOfWorkLiteDb, TDomainModel model)
                where TDomainModel : DomainModelBase<long>
            {

                UnitOfWorkLiteDb.Command(db => GetCollection<TDomainModel>(db).Update(model)
               //var dbModel = col.FindById(model.Id);
               //CopyAllProperties(source: model, destination: dbModel);
               //col.Update(dbModel);
               );
            }

            private static void CopyAllProperties<TDomainModel>(TDomainModel source, */
    }
}