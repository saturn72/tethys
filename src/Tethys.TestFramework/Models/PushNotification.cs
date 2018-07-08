namespace Tethys.TestFramework.Models
{
    public class PushNotification
    {
        public string Key { get; set; }
        public int Delay { get; set; }
        public object Body { get; set; }
    }

    public class HttpCall
    {
        public object Request { get; set; }
        public object Response { get; set; }
    }
}
