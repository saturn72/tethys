using System.Collections.Generic;

namespace Tethys.Server.Models
{
    public class Response : DomainModelBase
    {
        public int StatusCode { get; set; }
        public string Body { get; set; }

        public int Delay { get; set; }

        public virtual IDictionary<string, string> Headers { get; set; }
    }
}