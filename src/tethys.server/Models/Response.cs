namespace Tethys.Server.Models
{
    public class Response : DomainModelBase
    {
        public int HttpStatusCode { get; set; }

        public string Body { get; set; }

        public int Delay { get; set; }
    }
}