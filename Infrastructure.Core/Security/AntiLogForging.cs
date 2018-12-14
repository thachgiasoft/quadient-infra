using System.Linq;

namespace Infrastructure.Core.Security
{
    public static class AntiLogForging
    {
        private static readonly char[] BlackList = { '\n', '\r' };
        public static char SafeChar = '_';
        public static string Encoded = " (Encoded)";
        public static string PreventLogForging(string logInput)
        {
            if (string.IsNullOrWhiteSpace(logInput))
                return logInput;
            var clean = System.Web.Security.AntiXss.AntiXssEncoder.HtmlEncode(BlackList.Aggregate(logInput, (current, t) => current.Replace(t, SafeChar)), true);
            if (!logInput.Equals(clean))
            {
                clean += Encoded;
            }
            return clean;
        }
    }
}
