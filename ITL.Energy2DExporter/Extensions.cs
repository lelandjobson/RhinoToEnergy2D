using System.Linq;

namespace ITL.Energy2DExporter
{
    public static class Extensions
    {
        public static string Copy(this string s)
        {
            return new string(s.ToArray());
        }
    }
}
