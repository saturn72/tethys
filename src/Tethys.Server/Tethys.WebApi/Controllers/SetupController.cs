using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tethys.WebApi.Models;

namespace Tethys.WebApi.Controllers
{
    [Route(Consts.SetupControllerRoute)]
    public class SetupController : Controller
    {
        private readonly HttpCallRepository _httpCallRepository;

        public SetupController(HttpCallRepository httpCallRepository)
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
                    data = httpCall
                });
            await Task.Run(() => _httpCallRepository.Insert(httpCall));
            return new ObjectResult(httpCall) {StatusCode = StatusCodes.Status201Created};
        }
    }
}