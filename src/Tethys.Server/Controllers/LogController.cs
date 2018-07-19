
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tethys.Server.DbModel.Repositories;

namespace Tethys.Server.Controllers
{
    [Route(Consts.LogControllerRoute)]
    public class LogController : Controller
    {
        #region fields
        private readonly IHttpCallRepository _httpCallRepository;
        #endregion
        #region ctor
        public LogController(IHttpCallRepository httpCallRepository)
        {
            _httpCallRepository = httpCallRepository;
        }
        #endregion

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var allHttpCalls = await Task.Run(() => _httpCallRepository.GetAll());
            return Ok(allHttpCalls);
        }
    }
}