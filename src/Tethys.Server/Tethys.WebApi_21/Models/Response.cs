namespace Tethys.WebApi.Models
{
    public class Response
    {
        public int HttpStatusCode { get; set; }

        public string Body { get; set; }

        public int Delay { get; set; }
    }
}