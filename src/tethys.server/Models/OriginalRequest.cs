using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Tethys.Server.Models
{
    public class OriginalRequest : DomainModelBase
    {
        public string Path { get; internal set; }
        public string QueryString { get; internal set; }
        public string HttpMethod { get; internal set; }
        public IDictionary<string, string> Headers { get; internal set; }
        public object Body { get; internal set; }
    }
}
