namespace Tethys.Server
{
    public class Consts
    {
        public const string SiteUrl = "http://www.github.com/saturn72/Tethys";
        public const string Ui = "/ui";
        public const string ApiBaseUrl = "/tethys";
        public const string SwaggerEndPointPrefix = "/swagger";
        public const string MockControllerRoute = ApiBaseUrl + "/api/mock";
        public const string MockUploadRoute = "upload";
        public const string LogControllerRoute = ApiBaseUrl + "/api/log";
        public const string TethysWebSocketPath = "/ws-tethys";
        public const string TethysWebSocketPathNegotiate = "/negotiate";
        public const string OriginalRequest = "original-request";
        public const string PushNotificationLog = "pushNotificationLog";
    }
}