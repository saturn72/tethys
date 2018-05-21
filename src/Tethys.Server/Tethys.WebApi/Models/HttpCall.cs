using System.ComponentModel.DataAnnotations;

namespace Tethys.WebApi.Models
{
    public class HttpCall
    {
        [Required] public Request Request { get; set; }

        [Required] public Response Response { get; set; }
        public long Id { get; set; }
        public bool WasExecuted { get; set; }
    }
}