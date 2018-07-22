namespace Tethys.Server.Models
{
    public class RequestResponseCouple : DomainModelBase
    {
        public Request Request { get; set; }
        public Response Response { get; set; }
    }
}