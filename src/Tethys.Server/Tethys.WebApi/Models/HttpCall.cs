using System.ComponentModel.DataAnnotations;

namespace Tethys.WebApi.Models
{
    public class HttpCall
    {
        [Required] public Request Request { get; set; }

        [Required] public Response Response { get; set; }
    }
}