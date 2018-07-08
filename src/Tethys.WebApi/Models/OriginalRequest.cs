namespace Tethys.WebApi.Models
{
    public class OriginalRequest
    {
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string HttpMethod { get; set; }
    }
}
