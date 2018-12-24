using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Tethys.Server.Models
{
    public class Request : DomainModelBase
    {
        public string Resource { get; set; }
        public string Query { get; set; }
        public string HttpMethod { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public object Body { get; set; }
        public string BucketId { get; set; }
    }
}
