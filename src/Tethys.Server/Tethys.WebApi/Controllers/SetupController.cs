using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Tethys.WebApi.DbModel.Repositories;
using Tethys.WebApi.Models;

namespace Tethys.WebApi.Controllers
{
    [Route(Consts.SetupControllerRoute)]
    public class SetupController : Controller
    {
        private readonly IHttpCallRepository _httpCallRepository;

        public SetupController(IHttpCallRepository httpCallRepository)
        {
            _httpCallRepository = httpCallRepository;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] HttpCall httpCall)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    message = "Bad or missing data",
                    errors = ModelState.Values.Select(x => x.Errors),
                    data = httpCall
                });

            await Task.Run(() => _httpCallRepository.FlushUnhandled());
            await Task.Run(() => _httpCallRepository.Insert(httpCall));
            return new ObjectResult(httpCall) {StatusCode = StatusCodes.Status201Created};
        }
    }
}