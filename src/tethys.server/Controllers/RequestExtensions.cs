using System.Linq;
using System.Text;
using Tethys.Server.Models;

namespace Tethys.Server.Controllers
{
    public static class RequestExtensions
    {
        private const string LineFormat = "[{0}]";
        public static string ToReportFormat(this Request request)
        {
            var res = new StringBuilder();

            res.AppendLine(ToReportData("HTTP Method\tPath\tQuery"));
            res.AppendLine(ToReportData(
                request.HttpMethod.ToString().ToUpper() +
                '\t' + request.Resource
                + '\t' + request.Query));

            var headerLines = request.Headers?.Select(h => ToReportData(h.Key + "," + h.Value ?? ""))
                              ?? new string[] { };
            var headers = string.Join("\n\t", headerLines);
            res.AppendLine("HEADERS:\n" + ToReportData(headers));
            res.AppendLine("BODY:\n" + request.Body);

            return res.ToString();

            string ToReportData(string data)
            {
                return string.Format(LineFormat, data);
            }
        }
    }
}
