using System;
using System.ComponentModel.DataAnnotations;

namespace Tethys.Server.Models
{
    public class HttpCall : DomainModelBase
    {
        [Required] public Request Request { get; set; }
        [Required] public Response Response { get; set; }

        public int AllowedCallsNumber { get; set; }
        public int CallsCounter { get; set; }

        public bool WasFullyHandled { get; set; }
        public DateTime? HandledOnUtc { get; set; }

        public bool Flushed { get; set; }
        public DateTime? FlushedOnUtc { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}