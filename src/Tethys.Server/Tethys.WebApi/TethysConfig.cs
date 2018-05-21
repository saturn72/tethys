namespace Tethys.WebApi
{
    public class TethysConfig
    {
        public short HttpPort { get; set; }
        public static TethysConfig Default => 
            new TethysConfig
            {
                HttpPort = 4880
            };
    }
}