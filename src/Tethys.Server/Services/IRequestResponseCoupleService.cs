using System.Collections.Generic;
using System.Threading.Tasks;
using Tethys.Server.Models;

namespace Tethys.Server.Services
{
    public interface IRequestResponseCoupleService
    {
        Task<ServiceOperationResult> Create(RequestResponseCouple requestResponseCouple);
        Task Update(RequestResponseCouple requestResponseCouple);
        Task<IEnumerable<RequestResponseCouple>> GetAllAsync();
        Task<RequestResponseCouple> GetById(long id);
        Task DeleteAllAsync();
    }
}