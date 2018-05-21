using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace Tethys.WebApi.Models
{
    public class Request
    {
        public string Resource { get; set; }
        public string Query { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public string Body { get; set; }
    }
}