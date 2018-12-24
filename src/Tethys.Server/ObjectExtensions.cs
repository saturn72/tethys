namespace Tethys.Server
{
    public static class ObjectExtensions
    {
        public static bool HasValue(this string source)
        {
            return source != null && source.Trim().Length > 0;
        }
    }
}