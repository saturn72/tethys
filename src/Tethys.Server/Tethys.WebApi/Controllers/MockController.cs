using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Tethys.WebApi.Controllers
{
    [Route(Consts.MockControllerRoute)]
    public class MockController:Controller
    {
        public Task<IActionResult> Get()
        {
            throw new NotImplementedException();
                
        }
    }
}
