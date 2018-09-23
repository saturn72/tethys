using System.Collections.Generic;
using System.Threading.Tasks;
using Tethys.Server.Models;

namespace Tethys.Server.DbModel.Repositories
{
    public interface IRequestResponseCoupleRepository
    {
        void Create(RequestResponseCouple requestResponseCouple);
        void Update(RequestResponseCouple requestResponseCouple);
        IEnumerable<RequestResponseCouple> GetAll();
        RequestResponseCouple GetById(long id);
        void DeleteAllAsync();

    }
}
