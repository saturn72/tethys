namespace Tethys.Server.Models
{
    public class OriginalRequest : DomainModelBase
    {
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string HttpMethod { get; set; }
    }
}
