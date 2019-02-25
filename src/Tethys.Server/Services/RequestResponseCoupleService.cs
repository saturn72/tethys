using System.Collections.Generic;
using System.Threading.Tasks;
using Tethys.Server.DbModel;
using Tethys.Server.Models;

namespace Tethys.Server.Services
{
    public class RequestResponseCoupleService : IRequestResponseCoupleService
    {
        #region fields
        private readonly IRepository<RequestResponseCouple> _reqResCoupleRepository;

        #endregion

        #region ctor
        public RequestResponseCoupleService(IRepository<RequestResponseCouple> reqResCoupleRepository)
        {
            _reqResCoupleRepository = reqResCoupleRepository;
        }

        #endregion
        public async Task Create(RequestResponseCouple requestResponseCouple)
        {
            await Task.Run(() => _reqResCoupleRepository.Create(new[] { requestResponseCouple }));
        }

        public async Task DeleteAllAsync()
        {
            await Task.Run(() => _reqResCoupleRepository.DeleteAll());
        }

        public async Task<IEnumerable<RequestResponseCouple>> GetAllAsync()
        {
            return await Task.Run(() => _reqResCoupleRepository.GetAll());
        }

        public async Task<RequestResponseCouple> GetById(long id)
        {
            return await Task.Run(() => _reqResCoupleRepository.GetById(id));
        }

        public async Task Update(RequestResponseCouple requestResponseCouple)
        {
            await Task.Run(() => _reqResCoupleRepository.Update(requestResponseCouple));
        }
    }
}