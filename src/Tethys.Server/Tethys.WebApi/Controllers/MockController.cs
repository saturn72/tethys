using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Tethys.WebApi.Controllers
{
    public class MockController
    {
        [Route("{routePart1}/{routePart2}")]
        public Task<IActionResult> Get(string routePart1, string routePart2)
        {
            throw new NotImplementedException();
                
        }
    }
}
