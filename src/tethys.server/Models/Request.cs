using System.Collections.Generic;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace Tethys.Server.Models
{
    public class Request : DomainModelBase
    {
        public string Resource { get; set; }
        public string Query { get; set; }
        public string HttpMethod { get; set; }
        public string Body { get; set; }
        public IDictionary<string, IEnumerable<string>> Headers { get; set; }
    }
}