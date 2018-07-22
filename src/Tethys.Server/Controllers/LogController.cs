
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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var allReqResCouples = await _reqResCoupleService.GetAllAsync();
            return Ok(allReqResCouples);
        }
    }
}