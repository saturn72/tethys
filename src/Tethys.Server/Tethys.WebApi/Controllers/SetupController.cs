using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tethys.WebApi.Models;

namespace Tethys.WebApi.Controllers
{
    [Route("[controller]/http-call")]
    public class SetupController : Controller
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public void Post([FromBody]HttpCall httpCall)
        {
            throw new NotImplementedException();
        }
    }
}
