using System.Collections.Generic;
using System.Threading.Tasks;
using Tethys.Server.DbModel.Repositories;
using Tethys.Server.Models;

namespace Tethys.Server.Services
{
    public class RequestResponseCoupleService : IRequestResponseCoupleService
    {
        #region fields
        private readonly IRequestResponseCoupleRepository _reqResCoupleRepository;

        #endregion

        #region ctor
        public RequestResponseCoupleService(IRequestResponseCoupleRepository requestResponseCoupleRepository)
        {
            _reqResCoupleRepository = requestResponseCoupleRepository;
        }

        #endregion
        public async Task Create(RequestResponseCouple requestResponseCouple)
        {
            await Task.Run(() => _reqResCoupleRepository.Create(requestResponseCouple));
        }

        public async Task<IEnumerable<RequestResponseCouple>> GetAllAsync()
        {
            return await Task.Run(() => _reqResCoupleRepository.GetAll());
        }

        public async Task Update(RequestResponseCouple requestResponseCouple)
        {
            await Task.Run(() => _reqResCoupleRepository.Update(requestResponseCouple));
        }
    }
}