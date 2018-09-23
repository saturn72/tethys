
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tethys.Server.Services;

namespace Tethys.Server.Controllers
{
    [Route(Consts.LogControllerRoute)]
    public class LogController : Controller
    {
        #region fields
        private readonly IRequestResponseCoupleService _reqResCoupleService;
        #endregion
        #region ctor
        public LogController(IRequestResponseCoupleService reqResCoupeService)
        {
            _reqResCoupleService = reqResCoupeService;
        }
        #endregion

        /// <summary>
        /// Gets all log records
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var allReqResCouples = await _reqResCoupleService.GetAllAsync();
            return Ok(allReqResCouples);
        }
        /// <summary>
        /// Gets all log records
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(long id)
        {
            var requestResponseCouple = await _reqResCoupleService.GetById(id);
            return Ok(requestResponseCouple);
        }
    }
}