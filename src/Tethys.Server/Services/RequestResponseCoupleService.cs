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
        public async Task<ServiceOperationResult> Create(RequestResponseCouple requestResponseCouple)
        {
            await Task.Run(() => _reqResCoupleRepository.Create(requestResponseCouple));
            return new ServiceOperationResult
            {
                Status = requestResponseCouple.Id > 0 ? ServiceOperationStatus.Success : ServiceOperationStatus.Fail
            };
        }

        public async Task DeleteAllAsync()
        {
            await Task.Run(() => _reqResCoupleRepository.DeleteAllAsync());
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